    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace CamCon.Domain.Entity
    {
        [Table("AdminPageRequest")]
        [PrimaryKey("AdminPageRequestId")]
        public class AdminPageRequestModel
        {
            public Guid AdminPageRequestId { get; set; }
        
            public string? Id { get; set; }

            [ForeignKey("Id")]
            public ApplicationUserModel? User { get; set; }

            public Guid? MyOrganizationId { get; set; }

            [ForeignKey("MyOrganizationId")]
            public MyOrganizationModel? MyOrganization { get; set; }

            public Enums.PageRequestStatus PageRequestStatus { get; set; } = Enums.PageRequestStatus.Pending;

            [ForeignKey("AdminPageRequestId")]
            public List<PageRequestImageModel>? PageRequestImages { get; set; } = [];

            [NotMapped]
            public OrganizationDepartmentModel? Department { get; set; }
            
            public string OrganizationName { get; set; } = string.Empty;
            
            public string Reason { get; set; } = string.Empty;
            
            public Enums.PageRequestType PageRequestType { get; set; }
    }
}
