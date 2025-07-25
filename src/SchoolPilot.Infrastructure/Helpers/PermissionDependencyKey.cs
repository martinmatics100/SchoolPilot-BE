

namespace SchoolPilot.Infrastructure.Helpers
{
    public class PermissionDependencyKey
    {
        public int ParentPermission { get; set; }
        public int PermissionAction { get; set; }

        // Needed for dictionary key comparisons
        public override bool Equals(object? obj)
        {
            return obj is PermissionDependencyKey other &&
                   ParentPermission == other.ParentPermission &&
                   PermissionAction == other.PermissionAction;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParentPermission, PermissionAction);
        }
    }
}
