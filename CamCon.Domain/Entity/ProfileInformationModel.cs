using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CamCon.Domain.Entity
{
    [Table("ProfileInformation")]
    [PrimaryKey("ProfileInformationId")]
    public class ProfileInfo
    {
        public Guid ProfileInformationId { get; set; }

        public bool IsAdmin { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public byte[]? CoverPicture { get; set; }

        //Department
        public Guid? MyOrganizationId { get; set; }
        [ForeignKey("MyOrganizationId")]
        public MyOrganizationModel? MyOrganization { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Course { get; set; } = string.Empty;
    }
}
