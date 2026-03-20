namespace Domain.Models;

public class Contributors
{
    public Guid ContributorsId { get; set; }
    public Guid MyOrganizationId { get; set; }
    public string Id { get; set; } = string.Empty;
}