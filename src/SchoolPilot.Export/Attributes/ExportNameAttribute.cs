

namespace SchoolPilot.Export.Attributes
{
    public class ExportNameAttribute : Attribute
    {
        public string Name { get; }

        public string ShortName { get; }

        public ExportNameAttribute(string name, string shortName = null)
        {
            Name = name;
            ShortName = shortName;
        }
    }
}
