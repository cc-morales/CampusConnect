using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Feeds.Commands
{
    public record UpdatePostCommand(NewsFeedModel NewsFeed, List<Guid>? RemovedImageIds = null, List<NewsFeedImageModel>? NewImages = null) : IRequest<Result>;

    public class UpdatePostCommandHandler : AppDatabaseBase, IRequestHandler<UpdatePostCommand, Result>
    {
        public UpdatePostCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var existing = await GetDBContext().NewsFeeds
                .FirstOrDefaultAsync(n => n.NewsFeedId == request.NewsFeed.NewsFeedId, cancellationToken);

            if (existing is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Post not found."));

            existing.Message = request.NewsFeed.Message;
            existing.IsPublic = request.NewsFeed.IsPublic;

            if (request.RemovedImageIds is { Count: > 0 })
            {
                var imagesToRemove = await GetDBContext().NewsFeedImages
                    .Where(i => request.RemovedImageIds.Contains(i.NewsFeedImageId)
                             && i.NewsFeedId == request.NewsFeed.NewsFeedId)
                    .ToListAsync(cancellationToken);

                GetDBContext().NewsFeedImages.RemoveRange(imagesToRemove);
            }

            if (request.NewImages is { Count: > 0 })
            {
                foreach (var image in request.NewImages)
                {
                    image.NewsFeedId = existing.NewsFeedId;
                    GetDBContext().NewsFeedImages.Add(image);
                }
            }

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
