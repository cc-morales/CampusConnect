using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Feeds.Commands
{
    public record UpdatePostVisibilityCommand(Guid NewsFeedId, bool IsPublic) : IRequest<Result>;

    public class UpdatePostVisibilityCommandHandler : AppDatabaseBase, IRequestHandler<UpdatePostVisibilityCommand, Result>
    {
        public UpdatePostVisibilityCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(UpdatePostVisibilityCommand request, CancellationToken cancellationToken)
        {
            var existing = await GetDBContext().NewsFeeds
                .FirstOrDefaultAsync(n => n.NewsFeedId == request.NewsFeedId, cancellationToken);

            if (existing is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Post not found."));

            existing.IsPublic = request.IsPublic;

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

