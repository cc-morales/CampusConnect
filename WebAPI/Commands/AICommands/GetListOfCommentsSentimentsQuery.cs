using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;
using WebAPI.Services.GeminiServices;

namespace WebAPI.Commands.AICommands
{
    public record GetListOfCommentsSentimentsQuery(List<string> Sentiments) : IRequest<Result<List<NewsFeedCommentModel>>>;

    public class GetListOfCommentsSentimentsQueryHandler : AppDatabaseBase, IRequestHandler<GetListOfCommentsSentimentsQuery, Result<List<NewsFeedCommentModel>>>
    {
        private readonly IGeminiService _geminiService;

        public GetListOfCommentsSentimentsQueryHandler(AppDbContext context, IGeminiService geminiService) : base(context)
        {
            _geminiService = geminiService;
        }

        public IGeminiService GeminiService { get; }

        public async Task<Result<List<NewsFeedCommentModel>>> Handle(GetListOfCommentsSentimentsQuery request, CancellationToken cancellationToken)
        {
            var comments = GetDBContext().NewsFeedComments.AsQueryable();

            var currentComments = comments.Where(c => !c.IsFlagged && !c.IsDeleted);

            //AI for sentiment analysis simulation
            var sentimentsAI = await _geminiService.ModerateCommentsAsync(currentComments, request.Sentiments);

            var results = await currentComments
                .Include(c => c.User).ThenInclude(c => c.ProfileInformation)
                .Where(c => sentimentsAI.Any(s => s == c.NewsFeedCommentId))
                .Select(c => new NewsFeedCommentModel
                {
                    NewsFeedCommentId = c.NewsFeedCommentId,
                    NewsFeedId = c.NewsFeedId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt,
                    IsFlagged = c.IsFlagged,
                    IsDeleted = c.IsDeleted,
                    User = new ApplicationUserModel
                    {
                        UserName = c.User.UserName,
                        Email = c.User.Email,
                        Name = c.User.Name,
                        ProfileInformation = new ProfileInfo()
                        {
                            FullName = c.User.ProfileInformation.FullName,
                            ProfilePicture = c.User.ProfileInformation.ProfilePicture,
                        }
                    }
                })
                .ToListAsync(cancellationToken);
           
            return Result.Success(results);
        }
    }
}
