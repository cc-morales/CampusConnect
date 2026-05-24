using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CamCon.Domain.Entity;

[Table("DeviceFCMs")]
[PrimaryKey("FcmId")]
public class DeviceFcm
{
    public Guid FcmId { get; set; }

    public string FcmToken { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
}