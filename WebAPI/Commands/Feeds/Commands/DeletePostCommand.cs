using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;
namespace WebAPI.Commands.Feeds.Commands
{
    public record DeletePostCommand(Guid NewsFeedId) : IRequest<Result>;
    public class DeletePostCommandHandler : AppDatabaseBase, IRequestHandler<DeletePostCommand, Result>
    {
        public DeletePostCommandHandler(AppDbContext context) : base(context) { }
        public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var existing = await GetDBContext().NewsFeeds
                .Include(n => n.Images)
                .Include(n => n.Comments)
                .Include(n => n.Likes)
                .FirstOrDefaultAsync(n => n.NewsFeedId == request.NewsFeedId, cancellationToken);
            if (existing is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Post not found."));
            if (existing.Likes is { Count: > 0 })
                GetDBContext().Likes.RemoveRange(existing.Likes);
            if (existing.Comments is { Count: > 0 })
                GetDBContext().NewsFeedComments.RemoveRange(existing.Comments);
            if (existing.Images is { Count: > 0 })
                GetDBContext().NewsFeedImages.RemoveRange(existing.Images);
            GetDBContext().NewsFeeds.Remove(existing);
            await GetDBContext().SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
