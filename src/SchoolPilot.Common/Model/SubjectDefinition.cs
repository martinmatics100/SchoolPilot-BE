

using SchoolPilot.Common.Enums;

namespace SchoolPilot.Common.Model
{
    public sealed class SubjectDefinition
    {
        public string Name { get; set; } 
        public string Code { get; set; }
        public SchoolLevel[] Levels { get; set; }
        public SubjectCategory[] Category { get; set; }
    }
}
