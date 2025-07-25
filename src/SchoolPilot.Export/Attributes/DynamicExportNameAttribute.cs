

namespace SchoolPilot.Export.Attributes
{
    public class DynamicExportNameAttribute : ExportNameAttribute
    {
        public string PropertyName { get; set; }

        public DynamicExportNameAttribute(string name, string shortName, string propertyName)
            : base(name, shortName)
        {
            PropertyName = propertyName;
        }

        public DynamicExportNameAttribute(string name, string propertyName)
            : this(name, null, propertyName)
        {
        }
    }
}
