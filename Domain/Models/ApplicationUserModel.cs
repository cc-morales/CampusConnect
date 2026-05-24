using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{   
    public class ApplicationUserModel
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public Guid? ProfileInformationId { get; set; }
        public ProfileInfo? ProfileInformation { get; set; }

        public bool IsWebCreated { get; set; }

        public bool IsGuest { get; set; }
        
        public DateTime? GuestExpiresAt { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
