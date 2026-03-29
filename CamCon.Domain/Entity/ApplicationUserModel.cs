using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{   
    public class ApplicationUserModel : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public Guid? ProfileInformationId { get; set; }
        [ForeignKey("ProfileInformationId")]
        public ProfileInfo? ProfileInformation { get; set; }

        public bool IsWebCreated { get; set; }

        /// <summary>
        /// Comma-separated list of page-level permissions for admin users.
        /// </summary>
        public string? AccessControl { get; set; }

        public bool IsGuest { get; set; } = false;
        public DateTime? GuestExpiresAt { get; set; }
    }
}
