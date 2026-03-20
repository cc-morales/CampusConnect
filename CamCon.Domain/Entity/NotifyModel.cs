using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using CamCon.Shared;

namespace CamCon.Domain.Entity
{
    [Table("Notifications")]
    [PrimaryKey("NotifyId")]
    public class NotifyModel
    {
        public Guid NotifyId { get; set; }

        public Enums.NotificationType NotificationType { get; set; }

        public string DataJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        public string RecipientUserId { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        public bool IsUserDeleted { get; set; }

        public bool IsAdminDeleted { get; set; }
    }

    public class NotifyModel<T> : NotifyModel
    {
        [NotMapped]
        public T? Data
        {
            get => string.IsNullOrWhiteSpace(DataJson)
                ? default
                : JsonSerializer.Deserialize<T>(DataJson);
            set => DataJson = JsonSerializer.Serialize(value);
        }
    }

}
