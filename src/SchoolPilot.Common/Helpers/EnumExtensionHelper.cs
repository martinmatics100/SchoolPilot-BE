

using SchoolPilot.Common.Attributes;
using SchoolPilot.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;

namespace SchoolPilot.Common.Helpers
{
    internal static class EnumExtensionHelper
    {
        internal static (Dictionary<Enum, DescriptionAttribute> Descriptions, HashSet<Enum> ExcludedEnums)
           GetEnumDescriptions()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var enums = assemblies
                .Where(assembly => assembly.FullName.StartsWith("Proximify"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsEnum)
                .SelectMany(GetEnumValues)
                .ToList();

            var descriptions = enums
                .Where(ExcludeEnumValue)
                .ToDictionary(tuple => tuple.Value, elementSelector: GetEnumDescription);
            var excluded = enums.Where(w => !ExcludeEnumValue(w)).Select(s => s.Value).ToHashset();


            return (descriptions, excluded);
        }

        private struct EnumValueTuple
        {
            public Enum Value { get; set; }
            public FieldInfo FieldInfo { get; set; }
        }

        private static IEnumerable<EnumValueTuple> GetEnumValues(Type enumType)
        {
            return enumType.GetEnumValues()
                           .Cast<Enum>()
                           .Select(enumValue => new EnumValueTuple
                           {
                               Value = enumValue,
                               FieldInfo = enumType.GetField(enumValue.ToString())
                           });
        }

        private static bool ExcludeEnumValue(EnumValueTuple tuple)
        {
            var excludeEnumValueAttribute = tuple.FieldInfo.GetCustomAttribute<ExcludeEnumValueAttribute>();
            return excludeEnumValueAttribute == null;
        }

        private static DescriptionAttribute GetEnumDescription(EnumValueTuple tuple)
        {
            var defaultDescription = tuple.Value.ToString().ConvertToNormalText();

            foreach (var attribute in tuple.FieldInfo.GetCustomAttributes())
            {
                if (TryGetDescriptionFromAttribute(attribute, out string description))
                {
                    return new DescriptionAttribute(description);
                }
            }

            return new DescriptionAttribute(defaultDescription);
        }

        private static bool TryGetDescriptionFromAttribute(Attribute attribute, out string description)
        {
            var descriptionAttribute = attribute as DescriptionAttribute;
            if (descriptionAttribute != null)
            {
                description = descriptionAttribute.Description;
                return true;
            }
            else if (attribute is DisplayAttribute)
            {
                var displayAttribute = (DisplayAttribute)attribute;
                return TryGetDescriptionFromDisplayAttribute(displayAttribute, out description);
            }
            else if (attribute is DisplayNameAttribute)
            {
                description = ((DisplayNameAttribute)attribute).DisplayName;
                return true;
            }
            else
            {
                description = null;
                return false;
            }
        }

        private static bool TryGetDescriptionFromDisplayAttribute(DisplayAttribute displayAttribute, out string description)
        {
            description = displayAttribute.GetDescription();
            if (!string.IsNullOrWhiteSpace(description))
            {
                return true;
            }

            description = displayAttribute.GetName();
            if (!string.IsNullOrWhiteSpace(description))
            {
                return true;
            }

            return false;
        }

    }
}
