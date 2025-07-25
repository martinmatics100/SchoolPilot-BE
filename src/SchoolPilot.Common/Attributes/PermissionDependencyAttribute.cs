

using SchoolPilot.Common.Enums;

namespace SchoolPilot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PermissionDependencyAttribute : Attribute
    {
        public PermissionActions Key;

        public List<PermissionActions> Dependencies;

        public PermissionDependencyAttribute(params PermissionActions[] actions)
        {
            Dependencies = actions.ToList();
            Key = Dependencies[0];
            Dependencies.RemoveAt(0);
        }
    }
}
