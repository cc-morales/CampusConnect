using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CamCon.Domain.Entity
{
    [Table("Likes")]
    [PrimaryKey("LikeId")]
    public class LikeModel
    {
        public Guid LikeId { get; set; }

        public Enums.LikeType LikeType { get; set; }

        public string? UserId { get; set; }

        public string Emoji { get; set; } = string.Empty;

        public Guid NewsFeedId { get; set; }
    }
}
