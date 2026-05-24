namespace Domain.Models;

public class DeviceFcm
{
    public Guid FcmId { get; set; }

    public string FcmToken { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
}