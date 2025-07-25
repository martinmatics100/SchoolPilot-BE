

using System.Reflection;

namespace SchoolPilot.Data.Helpers
{
    public static class PropertyAttributeHelper
    {
        public static IList<PropertyAttribute<T>> GetPropertiesAndAttributes<T>(Type parentType)
        {
            const BindingFlags bindingFlags = BindingFlags.GetProperty
                                              | BindingFlags.SetProperty
                                              | BindingFlags.Public
                                              | BindingFlags.Instance;

            return parentType.GetProperties(bindingFlags)
                              .Union(parentType.GetInterfaces().SelectMany(x => x.GetProperties(bindingFlags)))
                              .Select(info => CreateSaveActionProperty<T>(parentType, info))
                              .Where(x => x.Attributes.Any())
                              .ToList();
        }

        private static PropertyAttribute<T> CreateSaveActionProperty<T>(Type parentType, PropertyInfo property)
        {
            var attributes = Attribute.GetCustomAttributes(property, typeof(T), true)
                                      .Cast<T>()
                                      .ToList();

            return new PropertyAttribute<T>
            {
                Get = entity => property.GetValue(entity),
                Set = (entity, value) => property.SetValue(entity, value),
                Attributes = attributes,
                PropertyType = property.PropertyType,
                ParentType = parentType,
                Name = property.Name
            };
        }
    }

    public class PropertyAttribute<T>
    {
        public Type ParentType { get; set; }

        public Type PropertyType { get; set; }

        public Action<object, object> Set { get; set; }

        public Func<object, object> Get { get; set; }

        public IList<T> Attributes { get; set; }
        public string Name { get; set; }
    }
}
