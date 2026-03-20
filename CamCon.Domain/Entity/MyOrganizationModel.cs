using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{
    [Table("MyOrganizations")]
    [PrimaryKey("MyOrganizationId")]
    public class MyOrganizationModel
    {
        public Guid MyOrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Enums.OrganizationType OrganizationType { get; set; }

        //Profile
        public string? Id { get; set; }
        [ForeignKey("Id")]
        public ApplicationUserModel? User { get; set; }

        public Guid? OrganizationDepartmentId { get; set; }
        [ForeignKey("OrganizationDepartmentId")]
        public OrganizationDepartmentModel? OrganizationDepartment { get; set; }

        public byte[]? Photo { get; set; }

        public byte[]? CoverPhoto { get; set; }

        [ForeignKey("MyOrganizationId")]
        public List<Contributors> Contributors { get; set; } = [];
    }
}
