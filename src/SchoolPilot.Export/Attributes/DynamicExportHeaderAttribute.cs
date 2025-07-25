

using SchoolPilot.Export.Enums;

namespace SchoolPilot.Export.Attributes
{
    public class DynamicExportHeaderAttribute : ExportHeaderAttribute
    {
        public DynamicExportHeaderAttribute(string header, Type filterType, int index = -1,
            ExportDataFormat dataFormat = ExportDataFormat.PropertyType)
            : base(header, dataFormat)
        {
            Index = index;
            FilterType = filterType;
        }

        public string[] FilterPropertyNames { get; set; }
    }
}
