

namespace SchoolPilot.Export.Attributes
{
    public class ExportTableHeaderAttribute : Attribute
    {
        public string TableHeader { get; }

        public ExportTableHeaderAttribute(string tableHeader)
        {
            TableHeader = tableHeader;
        }
    }
}
