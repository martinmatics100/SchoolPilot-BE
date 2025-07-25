

using AutoMapper;
using SchoolPilot.Common.Attributes;
using System.ComponentModel;

namespace SchoolPilot.Infrastructure.AutoMapper
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreNotAutoMappedAttributes<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties())
            {
                var descriptor = TypeDescriptor.GetProperties(desType)[property.Name];
                var attribute = (NotAutoMappedAttribute)descriptor.Attributes[typeof(NotAutoMappedAttribute)];
                if (attribute != null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            var interfaces = desType.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                foreach (var property in @interface.GetProperties())
                {
                    var descriptor = TypeDescriptor.GetProperties(@interface)[property.Name];
                    var attribute = (NotAutoMappedAttribute)descriptor.Attributes[typeof(NotAutoMappedAttribute)];
                    if (attribute != null)
                    {
                        expression.ForMember(property.Name, opt => opt.Ignore());
                    }
                }
            }
            return expression;
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllDestinationVirtual<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }

        public static IMappingExpression<TSource, TDestination> IgnorePropertiesInOtherSource<TSource, TDestination, TOtherSource>(this IMappingExpression<TSource, TDestination> expression)
        {
            var otherSourceTypeProperties = typeof(TOtherSource).GetProperties();
            var desType = typeof(TDestination);

            foreach (var property in desType.GetProperties())
            {
                foreach (var otherProperty in otherSourceTypeProperties)
                {
                    if (property.Name == otherProperty.Name)
                    {
                        expression.ForMember(property.Name, opt => opt.Ignore());
                    }
                }
            }

            return expression;
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllSourceVirtual<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var srcType = typeof(TSource);
            foreach (var property in srcType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForSourceMember(property.Name, opt => opt.DoNotValidate());
            }

            return expression;
        }
    }
}
