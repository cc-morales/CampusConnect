using CamCon.Shared;
using Domain;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface INewsFeedService
    {
        Task CreatePost(NewsFeedModel newsFeed);
        Task<NewsFeedModel> GetPostById(Guid guid);
        Task<PaginationResponseModel> GetNewsFeeds(PaginationRequestModel model);
        Task<List<NewsFeedModel>> GetNewsFeedById(Guid guid);

        Task<LikeModel> UpdateLike(LikeModel like);

        Task<Result> UpdatePostAsync(NewsFeedModel newsFeed, List<Guid>? removedImageIds = null, List<NewsFeedImageModel>? newImages = null);
        Task<Result> DeletePostAsync(Guid newsFeedId);
    }
}
