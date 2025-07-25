
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Infrastructure.Interfaces
{
    public interface IHasAsset
    {
        string FileName { get; set; }
        byte[] FileData { get; set; } // Added for raw file data
        string AssetToken { get; set; } // Now will store URL
        Guid AssetId { get; set; }
    }
}
