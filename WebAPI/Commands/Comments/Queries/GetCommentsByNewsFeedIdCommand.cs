using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Comments.Queries
{
    public record GetCommentsByNewsFeedIdCommand(Guid guid) : IRequest<Result<List<NewsFeedCommentModel>>>;
   
    public class GetCommentsByNewsFeedIdCommandHandler : AppDatabaseBase, IRequestHandler<GetCommentsByNewsFeedIdCommand, Result<List<NewsFeedCommentModel>>>
    {
        public GetCommentsByNewsFeedIdCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<List<NewsFeedCommentModel>>> Handle(GetCommentsByNewsFeedIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetDBContext().NewsFeedComments
                .Where(x => x.NewsFeedId == request.guid)
                .ToListAsync(cancellationToken: cancellationToken);

            return Result.Success(result);
        }
    }
}
