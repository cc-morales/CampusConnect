using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record DeleteOrganizationCommand(Guid OrganizationId) : IRequest<Result>;

    public class DeleteOrganizationCommandHandler(AppDbContext context)
        : AppDatabaseBase(context), IRequestHandler<DeleteOrganizationCommand, Result>
    {
        public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var db = GetDBContext();

            var org = await db.MyOrganizations
                .Include(o => o.Followers)
                .Include(o => o.Contributors)
                .FirstOrDefaultAsync(o => o.MyOrganizationId == request.OrganizationId, cancellationToken);

            if (org is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Organization not found"));

            using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (org.Contributors is not null && org.Contributors.Any())
                {
                    db.Contributors.RemoveRange(org.Contributors);
                }

                if (org.Followers is not null && org.Followers.Any())
                {
                    db.Followers.RemoveRange(org.Followers);
                }

                var feeds = await db.NewsFeeds.Where(f => f.MyOrganizationId == request.OrganizationId).ToListAsync(cancellationToken);
                if (feeds.Any())
                {
                    var feedIds = feeds.Select(f => f.NewsFeedId).ToList();
                    var images = await db.NewsFeedImages.Where(i => feedIds.Contains(i.NewsFeedId)).ToListAsync(cancellationToken);
                    if (images.Any()) db.NewsFeedImages.RemoveRange(images);

                    var comments = await db.NewsFeedComments.Where(c => feedIds.Contains(c.NewsFeedId)).ToListAsync(cancellationToken);
                    if (comments.Any()) db.NewsFeedComments.RemoveRange(comments);

                    db.NewsFeeds.RemoveRange(feeds);
                }

                db.MyOrganizations.Remove(org);

                await db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result.Failure(new Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}

