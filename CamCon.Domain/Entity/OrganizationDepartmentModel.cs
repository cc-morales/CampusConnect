using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{
    [Table("OrganizationDepartments")]
    [PrimaryKey("OrganizationDepartmentId")]
    public class OrganizationDepartmentModel
    {
        public Guid OrganizationDepartmentId { get; set; }
        public Guid MyOrganizationId { get; set; }
    }
}
