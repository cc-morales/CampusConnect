using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class NewsFeedModel
    {
        public Guid NewsFeedId { get; set; }

        public string Message { get; set; } = string.Empty;

        public Guid MyOrganizationId { get; set; }

        public MyOrganizationModel? MyOrganization { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<NewsFeedImageModel>? Images { get; set; }

        public List<NewsFeedCommentModel>? Comments { get; set; }

        public List<LikeModel>? Likes { get; set; }

        public bool IsPublic { get; set; }

        [JsonIgnore]
        public bool openEmoji { get; set; }
    }
}
