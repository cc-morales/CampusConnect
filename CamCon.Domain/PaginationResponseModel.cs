using CamCon.Domain.Entity;

namespace CamCon.Domain
{
    public class PaginationResponseModel
    {
        public int Count { get; set; }
        public List<NewsFeedModel> Records { get; set; } = [];
    }
}
