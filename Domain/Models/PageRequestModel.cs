using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class AdminPageRequestModel
    {
        public Guid AdminPageRequestId { get; set; }
        
        public string? Id { get; set; }

        public ApplicationUserModel? User { get; set; }

        public Guid? MyOrganizationId { get; set; }

        public MyOrganizationModel? MyOrganization { get; set; }

        public Enums.PageRequestStatus PageRequestStatus { get; set; }

        public List<PageRequestImageModel>? PageRequestImages { get; set; }

        public string OrganizationName { get; set; } = string.Empty;

        public OrganizationDepartmentModel? Department { get; set; }
        
        public string Reason { get; set; } = string.Empty;
    }
}
