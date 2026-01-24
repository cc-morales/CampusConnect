using CamCon.Domain.Enitity;

namespace WebAPI.Services.GeminiServices
{
    public interface IGeminiService
    {
        Task<List<Guid>> ModerateCommentsAsync(IQueryable<NewsFeedCommentModel> comments, List<string> badWords);
    }
}
