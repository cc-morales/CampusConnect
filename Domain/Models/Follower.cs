namespace Domain.Models;

public class Follower
{
    public Guid FollowerId { get; set; }
    public Guid MyOrganizationId { get; set; }
    public string Id { get; set; } = string.Empty;
}

