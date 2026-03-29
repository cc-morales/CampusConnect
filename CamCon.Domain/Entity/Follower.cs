using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CamCon.Domain.Entity;

[Table("Followers")]
[PrimaryKey("FollowerId")]
public class Follower
{
    public Guid FollowerId { get; set; }
    public Guid MyOrganizationId { get; set; }
    public string Id { get; set; } = string.Empty;
}

