

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Attributes;
using System.Collections.Concurrent;

namespace SchoolPilot.Data.Helpers
{
    public static class SaveActionHelper
    {
        // TODO
        // [ ] Take type from property, not of current type. Need to handle nullables as the cast looses that information.

        private static readonly ConcurrentDictionary<Type, IList<PropertyAttribute<SaveActionAttribute>>> ActionCache
            = new ConcurrentDictionary<Type, IList<PropertyAttribute<SaveActionAttribute>>>();

        private static IEnumerable<PropertyAttribute<SaveActionAttribute>> ActionsForEntityType(Type type)
        {
            Func<Type, IList<PropertyAttribute<SaveActionAttribute>>> actionFactory = PropertyAttributeHelper.GetPropertiesAndAttributes<SaveActionAttribute>;

            return ActionCache.GetOrAdd(type, actionFactory);
        }

        public static void ApplySaveActions(object entity, EntityState state, EntityEntry entryEntry)
        {
            var type = entity.GetType();
            var actionsForType = ActionsForEntityType(type);

            foreach (var savableProperty in actionsForType)
            {
                var saveActionAttributes = savableProperty
                    .Attributes.Where(x =>
                    {
                        var compatible = x.CanPerform(savableProperty.PropertyType);
                        if (!compatible)
                        {
                            throw new NotSupportedException(
                                $"Detected incompatible save action on property {savableProperty.Name} of type {savableProperty.Name}. "
                                + $"Consider checking both the returning type of {nameof(SaveActionAttribute.Perform)} "
                                + $"and the result of {nameof(SaveActionAttribute.CanPerform)}.");
                        }
                        return true;
                    });

                foreach (var action in saveActionAttributes)
                {
                    object originalValue = null;
                    if (state == EntityState.Modified && entryEntry != null && entryEntry.OriginalValues.Properties.Select(s => s.Name).Contains(savableProperty.Name))
                    {
                        originalValue = entryEntry.OriginalValues[savableProperty.Name];
                    }

                    var value = savableProperty.Get(entity);
                    var newValue = action.Perform(value, state, originalValue, entity, savableProperty.Name);
                    if (newValue != null && value != null && value.GetType() != newValue.GetType())
                    {
                        throw new NotSupportedException(
                            $"Detected incompatible save action on property {savableProperty.Name} of type {savableProperty.ParentType} - got {newValue.GetType()}, expected {savableProperty.PropertyType}. "
                            + $"Consider checking both the returning type of {nameof(SaveActionAttribute.Perform)} "
                            + $"and the result of {nameof(SaveActionAttribute.CanPerform)}.");
                    }
                    if (value?.Equals(newValue) != true)
                    {
                        savableProperty.Set(entity, newValue);
                    }
                }
            }
        }

    }
}
