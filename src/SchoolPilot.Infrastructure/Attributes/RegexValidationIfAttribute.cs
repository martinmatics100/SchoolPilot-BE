

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace SchoolPilot.Infrastructure.Attributes
{
    /// <summary>
    /// Provides conditional validation based on related property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RegexValidationIfAttribute : RegularExpressionAttribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the other property name that will be used during validation.
        /// </summary>
        /// <value>
        /// The other property name.
        /// </value>
        public string OtherProperty { get; private set; }

        /// <summary>
        /// Gets or sets the display name of the other property.
        /// </summary>
        /// <value>
        /// The display name of the other property.
        /// </value>
        public string OtherPropertyDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the other property value that will be relevant for validation.
        /// </summary>
        /// <value>
        /// The other property value.
        /// </value>
        public object OtherPropertyValue { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether other property's value should match or differ from provided other property's value (default is <c>false</c>).
        /// </summary>
        /// <value>
        ///   <c>true</c> if other property's value validation should be inverted; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// How this works
        /// - true: validated property is required when other property doesn't equal provided value
        /// - false: validated property is required when other property matches provided value
        /// </remarks>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        /// <returns><c>true</c> if the attribute requires validation context; otherwise, <c>false</c>.</returns>
        public override bool RequiresValidationContext => true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="otherProperty">The other property.</param>
        /// <param name="otherPropertyValue">The other property value.</param>
        /// <param name="pattern">Regular expression pattern</param>
        public RegexValidationIfAttribute(string otherProperty, object otherPropertyValue, string pattern)
            : base(pattern)
        {
            OtherProperty = otherProperty;
            OtherPropertyValue = otherPropertyValue;
            IsInverted = false;
        }

        #endregion

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }

            PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherProperty == null)
            {
                return new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, "Could not find a property named '{0}'.", OtherProperty));
            }

            object otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

            // check if this value is actually required and validate it
            if (!IsInverted && Equals(otherValue, OtherPropertyValue) ||
                IsInverted && !Equals(otherValue, OtherPropertyValue))
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
