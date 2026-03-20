using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CamCon.Domain.Entity;

[Table("Contributors")]
[PrimaryKey("ContributorsId")]
public class Contributors
{
    public Guid ContributorsId { get; set; }
    public Guid MyOrganizationId { get; set; }
    public string Id { get; set; } = string.Empty;
}