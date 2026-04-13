using System.ComponentModel.DataAnnotations;

namespace NZWalks.UI.Models
{
    public class AddRegionViewModel
    {
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Code must be exactly 3 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        public string? RegionImageUrl { get; set; }
    }
}
