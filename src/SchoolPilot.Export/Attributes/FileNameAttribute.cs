

namespace SchoolPilot.Export.Attributes
{
    public class FileNameAttribute : Attribute
    {
        public string FileName { get; }

        public FileNameAttribute(string name)
        {
            FileName = name;
        }
    }
}
