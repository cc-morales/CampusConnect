using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    /// <summary>
    /// Represents an admin account with role-based access control permissions.
    /// Used by the Presentation layer for managing admin users and their page-level access.
    /// </summary>
    public class AdminAccountModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Password { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// The pages/features this admin is allowed to access.
        /// Values should correspond to <see cref="Constants.AdminPermissions"/> constants.
        /// </summary>
        public string[] AccessControl { get; set; } = [];
    }
}

