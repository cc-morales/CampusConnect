using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamCon.Domain.Entity
{
    [Table("NewsFeeds")]
    [PrimaryKey("NewsFeedId")]
    public class NewsFeedModel
    {
        public Guid NewsFeedId { get; set; }

        public string Message { get; set; } = string.Empty;

        public Guid MyOrganizationId { get; set; }

        [ForeignKey("MyOrganizationId")]
        public MyOrganizationModel? MyOrganization { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("NewsFeedId")]
        public List<NewsFeedImageModel>? Images { get; set; }

        [ForeignKey("NewsFeedId")]
        public List<NewsFeedCommentModel>? Comments { get; set; }

        [ForeignKey("NewsFeedId")]
        public List<LikeModel>? Likes { get; set; }

        public bool IsPublic { get; set; }
    }
}
