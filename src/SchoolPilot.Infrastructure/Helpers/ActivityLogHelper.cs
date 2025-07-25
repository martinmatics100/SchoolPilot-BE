

using Newtonsoft.Json.Linq;
using NLog;
using SchoolPilot.Data.Entities.Logs;
using SchoolPilot.Infrastructure.Attributes;
using SchoolPilot.Infrastructure.SimpleHelpers;
using SchoolPilot.Common.Extensions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections;
using SchoolPilot.Data.Helpers;

namespace SchoolPilot.Infrastructure.Helpers
{
    public interface IActivityLogHelper
    {
        List<ActivityLogDetails> GenerateActivityLogDetails<T>(Guid activityLogId, Guid accountId, T oldObject, T newObject, params KeyValuePair<string, Func<CustomFormattingData<T>, (string DisplayName, string NewValue, string OldValue)?>>[] customFormattings)
            where T : class;

        //TODO: Add a method to display activity log details with a certain format. Needed until elastic search is implemented
    }

    public class ActivityLogHelper : IActivityLogHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<Type, Dictionary<string, PropertyDetails>> ClassPropertiesTypeMappings;

        public ActivityLogHelper()
        {
            ClassPropertiesTypeMappings = new ConcurrentDictionary<Type, Dictionary<string, PropertyDetails>>();
        }

        /// <summary>
        ///     This will generate what changes were made between the passed in objects for the Activity Log.
        ///     List properties cannot be sent as null, must at least be an empty list.
        ///     In order for a SubEntity to get an Id, the "Id" property must be given, it is the only Guid which will be checked.
        ///     Nested lists are not currently handled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityLogId">The Id of the activity log created in the calling method.</param>
        /// <param name="accountId">The accountId of the activity log created in the calling method.</param>
        /// <param name="oldObject">The object state before the changes were made.</param>
        /// <param name="newObject">The object state after the changes were made.</param>
        /// <param name="customFormattings">
        ///     Optional: Any custom functions to be used for specific properties which need special formatting.
        ///             The Key should be the name of the property the custom function should be used for. If it is
        ///             a nested property, it should be {ParentName}.{Name}.
        /// </param>
        /// <returns>List of created ActivityLogDetails</returns>
        ///     --For nested lists to be handled -> If a fieldChange is a JObject, check the pulled object for its type instead of the property mapping which
        ///         uses the parent information. Will also need to determine how that will be displayed since only the root property will have formatting data

        public List<ActivityLogDetails> GenerateActivityLogDetails<T>(Guid activityLogId, Guid accountId, T oldObject, T newObject, params KeyValuePair<string, Func<CustomFormattingData<T>, (string DisplayName, string NewValue, string OldValue)?>>[] customFormattings)
            where T : class
        {
            var propertiesTypeMapping = GetOrAddPropertiesTypeMapping<T>();
            var activityLogDetailsList = new List<ActivityLogDetails>();
            var dataChanges = ObjectDiffPatch.GenerateDiff(oldObject, newObject);

            var fieldChanges = new List<ActivityLogDetailsInfo>();
            if (!dataChanges.AreEqual)
            {
                //If the new object is given, use it as the base for the changes and pull the old values tied to it, if they exist
                if (newObject != null)
                {
                    var oldValues = dataChanges.OldValues?.Properties()?.ToDictionary(k => k.Name, v => v.Value) ?? new Dictionary<string, JToken>();

                    //For the base Object, only newObject properties are considered
                    fieldChanges = dataChanges.NewValues.Properties()
                        .Select(s => new ActivityLogDetailsInfo
                        {
                            Name = s.Name,
                            Key = s.Name,
                            ParentId = activityLogId,
                            ParentNewObject = newObject,
                            ParentOldObject = oldObject,
                            NewValue = s.Value,
                            OldValue = oldValues.TryGetValue(s.Name, out var oldToken) ? oldToken : null
                        })
                        .ToList();
                }
                else
                {
                    //If no new object was given this must be a delete call, in which case use the old object as the base for changes
                    fieldChanges = dataChanges.OldValues.Properties()
                        .Select(s => new ActivityLogDetailsInfo
                        {
                            Name = s.Name,
                            Key = s.Name,
                            ParentId = activityLogId,
                            ParentNewObject = null,
                            ParentOldObject = oldObject,
                            OldValue = s.Value
                        })
                        .ToList();
                }
            }

            while (fieldChanges.Any())
            {
                var fieldChange = fieldChanges.First();
                fieldChanges.RemoveAt(0);
                if (!propertiesTypeMapping.TryGetValue(fieldChange.Key, out var info))
                {
                    //This should not happen unless the dictionary creation is invalid.
                    continue;
                }

                var propertyInformation = info;
                var activityLogDetails = new ActivityLogDetails
                {
                    Id = SequentialGuid.Create(),
                    AccountId = accountId,
                    ActivityLogId = activityLogId,
                    EntryType = fieldChange.IsListElement ? EntryType.Field | EntryType.List : EntryType.Field,
                    ParentId = fieldChange.ParentId,
                    SubEntityType = propertyInformation.Attribute?.SubEntityType
                };

                if (propertyInformation.Attribute?.IsHidden ?? false)
                {
                    activityLogDetails.EntryType |= EntryType.Hidden;
                }

                //Display name should be what is defined by the PropertyMapping. If this is part of a list, append the element number to the name
                var displayName = propertyInformation.Attribute?.DisplayName + $"{(fieldChange.IsListElement ? " #" + fieldChange.Name : "")}";
                string newValueString = null;
                string oldValueString = null;
                var propertyType = propertyInformation.Info.PropertyType;

                if (fieldChange.NewValue is JObject || fieldChange.OldValue is JObject)
                {
                    var prefix = fieldChange.Key + ".";

                    //If this is a list element, its object to pull from was already passed down as the parentObject. If not, pull this field's object from the parent
                    var thisNewObject = fieldChange.IsListElement
                        ? fieldChange.ParentNewObject
                        : fieldChange.ParentNewObject == null
                            ? null
                            : propertyInformation.Info.GetValue(fieldChange.ParentNewObject);

                    var thisOldObject = fieldChange.IsListElement
                        ? fieldChange.ParentOldObject
                        : fieldChange.ParentOldObject == null
                            ? null
                            : propertyInformation.Info.GetValue(fieldChange.ParentOldObject);

                    //If the nested property object is a list or array and not an element of the list. (List elements use the main list property's type info)
                    if ((propertyType.GetInterfaces().Contains(typeof(ICollection)) || propertyType.IsArray) && !fieldChange.IsListElement)
                    {
                        activityLogDetails.EntryType = EntryType.List;

                        //Convert this object into a list of objects, since this property is of a list/array this should always work
                        var thisNewObjectList = (IList)thisNewObject;
                        var thisOldObjectList = (IList)thisOldObject;

                        //Lists always have the action of being updated, each element has a more specific action
                        activityLogDetails.ActionType = ActionType.Updated;

                        //Get the lists elements which are represented as properties in an object
                        var newSubProperties = (fieldChange.NewValue as JObject)?.Properties().ToList() ?? new List<JProperty>();
                        var oldSubProperties = (fieldChange.OldValue as JObject)?.Properties().ToList() ?? new List<JProperty>();

                        //Get the old and new list counts which are the values for the parent list entry
                        var newListCount = newSubProperties.FirstOrDefault(f => f.Name == ObjectDiffPatch.PREFIX_ARRAY_SIZE);
                        var oldListCount = oldSubProperties.FirstOrDefault(f => f.Name == ObjectDiffPatch.PREFIX_ARRAY_SIZE);

                        //Remove the count from the list so that the actual elements can be compared
                        newSubProperties.Remove(newListCount);
                        oldSubProperties.Remove(oldListCount);

                        newValueString = newListCount?.Value?.ToString();
                        oldValueString = oldListCount?.Value?.ToString();

                        var listFieldChanges = new List<ActivityLogDetailsInfo>();
                        foreach (var newSubProperty in newSubProperties)
                        {
                            //Get the old version of this element if it exists.
                            //Remove the element from the list so that the unmatched old properties can be added later.
                            var oldSubProperty = oldSubProperties.FirstOrDefault(f => f.Name == newSubProperty.Name);
                            oldSubProperties.Remove(oldSubProperty);

                            var index = int.Parse(newSubProperty.Name);
                            listFieldChanges.Add(new ActivityLogDetailsInfo
                            {
                                Name = $"{index + 1}",
                                Key = fieldChange.Key,
                                ParentId = activityLogDetails.Id,
                                ParentNewObject = thisNewObjectList[index],
                                ParentOldObject = oldSubProperty != null ? thisOldObjectList[index] : null,
                                NewValue = newSubProperty.Value,
                                OldValue = oldSubProperty?.Value,
                                IsListElement = true
                            });
                        }

                        foreach (var oldSubProperty in oldSubProperties)
                        {
                            var index = int.Parse(oldSubProperty.Name);
                            listFieldChanges.Add(new ActivityLogDetailsInfo
                            {
                                Name = $"{index + 1}",
                                Key = fieldChange.Key,
                                ParentId = activityLogDetails.Id,
                                ParentNewObject = null,
                                ParentOldObject = thisOldObjectList[index],
                                NewValue = null,
                                OldValue = oldSubProperty.Value,
                                IsListElement = true
                            });
                        }

                        fieldChanges.AddRange(listFieldChanges);
                    }
                    //Check if the nested object is a class
                    else if (propertyType.IsClass)
                    {
                        activityLogDetails.EntryType = fieldChange.IsListElement ? EntryType.SubEntity | EntryType.List : EntryType.SubEntity;

                        //Get the subProperties from the parent object.
                        var newSubProperties = (fieldChange.NewValue as JObject)?.Properties().ToList() ?? new List<JProperty>();
                        var oldSubProperties = (fieldChange.OldValue as JObject)?.Properties().ToList() ?? new List<JProperty>();

                        //Get the Id of this subEntity which should have the name Id, if it exists
                        var subEntityIdValue = newSubProperties?.FirstOrDefault(f => f.Name == "Id")?.Value?.ToString()
                                               ?? oldSubProperties?.FirstOrDefault(f => f.Name == "Id")?.Value?.ToString();

                        if (subEntityIdValue == null)
                        {
                            var objectToCheck = thisNewObject ?? thisOldObject;
                            var objectSubProperty = objectToCheck.GetType().GetProperty("Id");
                            if (objectSubProperty != null)
                            {
                                subEntityIdValue = objectSubProperty.GetValue(objectToCheck)?.ToString();
                            }
                        }
                        else
                        {
                            //Remove the Id property from both old and new property lists so that it is not considered as a change.
                            newSubProperties = newSubProperties.Where(w => w.Name != "Id").ToList();
                            oldSubProperties = oldSubProperties.Where(w => w.Name != "Id").ToList();
                        }

                        if (Guid.TryParse(subEntityIdValue, out var subEntityId))
                        {
                            activityLogDetails.SubEntityId = subEntityId;
                        }

                        //If neither subEntity has any data, skip it
                        if (!newSubProperties.Any() && !oldSubProperties.Any())
                        {
                            continue;
                        }

                        //If this subEntity is marked as a single field, it should only have one property (also a single field) whose value is used.
                        //This accounts for when a subEntity has only one field displayed to the user but still should be tracked with an EntityId.
                        if (propertyInformation.Attribute?.IsSingleField ?? true)
                        {
                            activityLogDetails.EntryType |= EntryType.Field;

                            if (newSubProperties.Any())
                            {
                                //Get the first property, which should be the only one, and take its Key and Value.
                                var singleField = newSubProperties.First();
                                fieldChange.Key = prefix + singleField.Name;
                                fieldChange.NewValue = singleField.Value;
                            }

                            if (oldSubProperties.Any())
                            {
                                //Get the first property, which should be the only one, and take its Key and Value.
                                var singleField = oldSubProperties.First();
                                fieldChange.Key = prefix + singleField.Name;
                                fieldChange.OldValue = singleField.Value;
                            }

                            if (!propertiesTypeMapping.TryGetValue(fieldChange.Key, out var newFieldPropertyInfo))
                            {
                                //Again, should not happen unless the dictionary creation is invalid or the subProperties get an incorrect key.
                                continue;
                            }
                        }
                        else
                        {
                            var subFieldChanges = new List<ActivityLogDetailsInfo>();
                            if (newSubProperties.Any())
                            {
                                //TODO: Test if JToken.IsNullOrEmpty is working as intended
                                var hasChange = false;

                                //Add the subProperties back into the fieldChanges list so that their changes can be determined later
                                foreach (var newSubProperty in newSubProperties)
                                {
                                    var oldValue = oldSubProperties.FirstOrDefault(f => f.Name == newSubProperty.Name)?.Value;
                                    hasChange |= !oldValue.IsNullOrEmpty();
                                    subFieldChanges.Add(new ActivityLogDetailsInfo
                                    {
                                        Name = newSubProperty.Name,
                                        Key = prefix + newSubProperty.Name,
                                        ParentId = activityLogDetails.Id,
                                        ParentNewObject = thisNewObject,
                                        ParentOldObject = thisOldObject,
                                        NewValue = newSubProperty.Value,
                                        OldValue = oldValue
                                    });
                                }

                                activityLogDetails.ActionType = hasChange ? ActionType.Updated : ActionType.Added;
                            }
                            else
                            {
                                foreach (var oldSubProperty in oldSubProperties)
                                {
                                    subFieldChanges.Add(new ActivityLogDetailsInfo
                                    {
                                        Name = oldSubProperty.Name,
                                        Key = prefix + oldSubProperty.Name,
                                        ParentId = activityLogDetails.Id,
                                        ParentNewObject = thisNewObject,
                                        ParentOldObject = thisOldObject,
                                        NewValue = null,
                                        OldValue = oldSubProperty.Value
                                    });
                                }

                                activityLogDetails.ActionType = ActionType.Removed;
                            }

                            fieldChanges.AddRange(subFieldChanges);
                        }
                    }
                    else
                    {
                        //If this is an object that is not a class or a list, it is not handled
                        Logger.Warn($"{typeof(T)} {fieldChange.Key} is an Object which is not a Class nor a List.");
                    }
                }

                //Single field entity or a list element
                if (activityLogDetails.EntryType.ContainsFlag(EntryType.Field))
                {
                    newValueString = fieldChange.NewValue?.ToString();
                    oldValueString = fieldChange.OldValue?.ToString();

                    //Should only happen if no OldObject was passed in and the NewValue was also empty,
                    //meaning on creation the NewValue was not entered and thus should not be recorded as a change in the ActivityLog
                    if (newValueString.IsNullOrEmpty() && oldValueString.IsNullOrEmpty())
                    {
                        continue;
                    }

                    //If a customFormatting was provided for this field's key, use it instead.
                    var matchingCustomFormattings = customFormattings.FirstOrDefault(f => f.Key == fieldChange.Key);
                    if (!matchingCustomFormattings.Key.IsNullOrEmpty())
                    {
                        var customResults = matchingCustomFormattings.Value(new CustomFormattingData<T>
                        {
                            OldValue = oldValueString,
                            NewValue = newValueString,
                            NewObject = newObject,
                            OldObject = oldObject
                        });

                        if (!customResults.HasValue)
                        {
                            continue;
                        }

                        displayName = customResults.Value.DisplayName ?? displayName;
                        newValueString = customResults.Value.NewValue ?? newValueString;
                        oldValueString = customResults.Value.OldValue ?? oldValueString;
                    }
                    else
                    {
                        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                        if (underlyingType.IsEnum)
                        {
                            //Check if this is a flagged enum
                            if (underlyingType.GetCustomAttributes<FlagsAttribute>().Any())
                            {
                                activityLogDetails.EntryType |= EntryType.FlaggedEnum;

                                //Gets the default value of the enum, the check below for flags will always pull the default
                                if (!newValueString.IsNullOrEmpty() && Enum.Parse(underlyingType, newValueString) is Enum newEnumValue)
                                {
                                    newValueString = newEnumValue.GetFlaggedDescription(underlyingType);
                                }

                                //Gets the default value of the enum, the check below for flags will always pull the default
                                if (!oldValueString.IsNullOrEmpty() && Enum.Parse(underlyingType, oldValueString) is Enum oldEnumValue)
                                {
                                    oldValueString = oldEnumValue.GetFlaggedDescription(underlyingType);
                                }
                            }
                            else
                            {
                                //Get the enum value's description
                                if (!newValueString.IsNullOrEmpty())
                                {
                                    newValueString = (Enum.Parse(underlyingType, newValueString) as Enum).GetEnumDescription();
                                }

                                if (!oldValueString.IsNullOrEmpty())
                                {
                                    oldValueString = (Enum.Parse(underlyingType, oldValueString) as Enum).GetEnumDescription();
                                }
                            }
                        }
                        else if (underlyingType == typeof(bool))
                        {
                            //Turn True/False into Yes/No
                            if (!newValueString.IsNullOrEmpty())
                            {
                                newValueString = newValueString == "True" ? "Yes" : "No";
                            }

                            if (!oldValueString.IsNullOrEmpty())
                            {
                                oldValueString = oldValueString == "True" ? "Yes" : "No";
                            }
                        }
                        else if (underlyingType == typeof(DateTime))
                        {
                            //Turn the string into a date
                            if (!newValueString.IsNullOrEmpty())
                            {
                                newValueString = $"{DateTime.Parse(newValueString):d}";
                            }

                            if (!oldValueString.IsNullOrEmpty())
                            {
                                oldValueString = $"{DateTime.Parse(oldValueString):d}";
                            }
                        }
                        else if (underlyingType == typeof(TimeSpan))
                        {
                            //Turn the string into a time
                            if (!newValueString.IsNullOrEmpty())
                            {
                                //Need to add the timespan to a date in order to get AM/PM
                                newValueString = $"{DateTime.Today.Add(TimeSpan.Parse(newValueString)):hh:mm tt}";
                            }

                            if (!oldValueString.IsNullOrEmpty())
                            {
                                //Need to add the timespan to a date in order to get AM/PM
                                oldValueString = $"{DateTime.Today.Add(TimeSpan.Parse(oldValueString)):hh:mm tt}";
                            }
                        }
                        else if (underlyingType == typeof(Guid))
                        {
                            //Guid should only be present to give sub-entity a reference Id.
                            continue;
                        }
                    }

                    //If the value is still null after converting for the type, check to see if there is a default null value defined	
                    //If the parent object is null, then this field was not given, and should remain null	
                    if (newValueString == null && fieldChange.ParentNewObject != null)
                    {
                        newValueString = propertyInformation.Attribute?.NullDefaultValue;
                    }
                    if (oldValueString == null && fieldChange.ParentOldObject != null)
                    {
                        oldValueString = propertyInformation.Attribute?.NullDefaultValue;
                    }

                    //Check to see what type of action was performed based on the new/old results
                    activityLogDetails.ActionType = !oldValueString.IsNullOrEmpty() && !newValueString.IsNullOrEmpty() ? ActionType.Updated :
                        !newValueString.IsNullOrEmpty() ? ActionType.Added : ActionType.Removed;
                }

                activityLogDetails.PropertyDisplayName = displayName;
                activityLogDetails.OldValue = oldValueString;
                activityLogDetails.NewValue = newValueString;
                activityLogDetailsList.Add(activityLogDetails);
            };

            return activityLogDetailsList;
        }

        /// <summary>
        /// Gets a dictionary mapping the provided class's properties to their type and ActivityLogDetails attribute info,
        /// or Adds one if it has not already been created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        private static Dictionary<string, PropertyDetails> GetOrAddPropertiesTypeMapping<T>() where T : class
        {
            var type = typeof(T);
            if (ClassPropertiesTypeMappings.TryGetValue(type, out var propertyMappings))
            {
                return propertyMappings;
            }

            var mapping = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(s => new
                {
                    s.Name,
                    PropertyDetails = new PropertyDetails
                    {
                        Info = s,
                        Attribute = s.GetCustomAttribute<ActivityLogDetailsAttribute>() ?? new ActivityLogDetailsAttribute
                        {
                            IsSingleField = true,
                            DisplayName = s.Name
                        }
                    }
                })
                .ToList();

            var subClassProperties = mapping.Where(w => w.PropertyDetails.Info.PropertyType.IsClass && w.PropertyDetails.Info.PropertyType != typeof(string)).ToList();

            while (subClassProperties.Any())
            {
                var subClassProperty = subClassProperties.First();
                subClassProperties.RemoveAt(0);

                type = subClassProperty.PropertyDetails.Info.PropertyType;
                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof(ICollection)) || interfaces.Contains(typeof(IEnumerable)) || type.IsArray)
                {
                    type = type.GetGenericArguments().Single();
                }

                var subProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(s => new
                    {
                        Name = subClassProperty.Name + "." + s.Name,
                        PropertyDetails = new PropertyDetails
                        {
                            Info = s,
                            Attribute = s.GetCustomAttribute<ActivityLogDetailsAttribute>() ?? new ActivityLogDetailsAttribute
                            {
                                IsSingleField = true,
                                DisplayName = s.Name
                            }
                        }
                    })
                    .ToList();

                mapping.AddRange(subProperties);
                var furtherNestedClasses = subProperties.Where(w => w.PropertyDetails.Info.PropertyType.IsClass && w.PropertyDetails.Info.PropertyType != typeof(string)).ToList();
                if (furtherNestedClasses.Any())
                {
                    subClassProperties.AddRange(furtherNestedClasses);
                }
            }

            propertyMappings = mapping.ToDictionary(k => k.Name, v => v.PropertyDetails);
            //Do a TryAdd in case another thread happened to have already added this type's data	
            ClassPropertiesTypeMappings.TryAdd(type, propertyMappings);
            return propertyMappings;
        }

    }

    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return token.IsNull()
                   || token.Type == JTokenType.Array && !token.HasValues
                   || token.Type == JTokenType.Object && !token.HasValues
                   || token.Type == JTokenType.String && token.ToString() == string.Empty;
        }

        public static bool IsNull(this JToken token)
        {
            return token == null || token.Type == JTokenType.Null;
        }
    }

    public class CustomFormattingData<T>
    {
        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public T OldObject { get; set; }

        public T NewObject { get; set; }
    }

    public class PropertyDetails
    {
        public PropertyInfo Info { get; set; }

        public ActivityLogDetailsAttribute? Attribute { get; set; }
    }

    public class ActivityLogDetailsInfo
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public Guid ParentId { get; set; }

        //Needed for new objects that have fields that are the same as the old object.
        //May need the field value (e.g. Id) but the diffResults will drop it since it didn't change.
        public object ParentNewObject { get; set; }

        public object ParentOldObject { get; set; }

        public JToken NewValue { get; set; }

        public JToken OldValue { get; set; }

        public bool IsListElement { get; set; }
    }

}
