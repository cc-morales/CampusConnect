using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(string UserId) : IRequest<Result>;

    public class DeleteUserCommandHandler : AppDatabaseBase, IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            ILogger<DeleteUserCommandHandler> logger) : base(context)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var db = GetDBContext();

                var user = await _userManager.FindByIdAsync(command.UserId);
                if (user is null)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status404NotFound, "User not found");

                // Get all organizations owned by this user
                var userOrganizationIds = await db.MyOrganizations
                    .Where(o => o.Id == command.UserId)
                    .Select(o => o.MyOrganizationId)
                    .ToListAsync(cancellationToken);

                if (userOrganizationIds.Count > 0)
                {
                    // 1a. Get all NewsFeeds belonging to the user's organizations
                    var newsFeedIds = await db.NewsFeeds
                        .Where(nf => userOrganizationIds.Contains(nf.MyOrganizationId))
                        .Select(nf => nf.NewsFeedId)
                        .ToListAsync(cancellationToken);

                    if (newsFeedIds.Count > 0)
                    {
                        // Delete likes on those news feeds
                        var feedLikes = await db.Likes
                            .Where(l => newsFeedIds.Contains(l.NewsFeedId))
                            .ToListAsync(cancellationToken);
                        if (feedLikes.Count > 0)
                            db.Likes.RemoveRange(feedLikes);

                        // Delete comments on those news feeds (include soft-deleted via IgnoreQueryFilters)
                        var feedComments = await db.NewsFeedComments
                            .IgnoreQueryFilters()
                            .Where(c => newsFeedIds.Contains(c.NewsFeedId))
                            .ToListAsync(cancellationToken);
                        if (feedComments.Count > 0)
                            db.NewsFeedComments.RemoveRange(feedComments);

                        // Delete images on those news feeds
                        var feedImages = await db.NewsFeedImages
                            .Where(i => newsFeedIds.Contains(i.NewsFeedId))
                            .ToListAsync(cancellationToken);
                        if (feedImages.Count > 0)
                            db.NewsFeedImages.RemoveRange(feedImages);

                        // Delete the news feeds themselves
                        var newsFeeds = await db.NewsFeeds
                            .Where(nf => userOrganizationIds.Contains(nf.MyOrganizationId))
                            .ToListAsync(cancellationToken);
                        db.NewsFeeds.RemoveRange(newsFeeds);
                    }

                    // 1b. Delete PageRequest images, then PageRequests referencing the user's organizations
                    var pageRequestIds = await db.RequestPages
                        .Where(pr => userOrganizationIds.Contains(pr.MyOrganizationId!.Value))
                        .Select(pr => pr.AdminPageRequestId)
                        .ToListAsync(cancellationToken);

                    if (pageRequestIds.Count > 0)
                    {
                        var prImages = await db.PageRequestImages
                            .Where(pi => pageRequestIds.Contains(pi.AdminPageRequestId))
                            .ToListAsync(cancellationToken);
                        if (prImages.Count > 0)
                            db.PageRequestImages.RemoveRange(prImages);

                        var pageRequests = await db.RequestPages
                            .Where(pr => pageRequestIds.Contains(pr.AdminPageRequestId))
                            .ToListAsync(cancellationToken);
                        db.RequestPages.RemoveRange(pageRequests);
                    }

                    // 1c. Delete OrganizationDepartments belonging to those organizations
                    var orgDepartments = await db.OrganizationDepartments
                        .Where(od => userOrganizationIds.Contains(od.MyOrganizationId))
                        .ToListAsync(cancellationToken);
                    if (orgDepartments.Count > 0)
                        db.OrganizationDepartments.RemoveRange(orgDepartments);

                    // 1d. Nullify ProfileInformation.MyOrganizationId references to these organizations
                    var profilesReferencingOrgs = await db.ProfileInformations
                        .Where(p => p.MyOrganizationId.HasValue && userOrganizationIds.Contains(p.MyOrganizationId.Value))
                        .ToListAsync(cancellationToken);
                    foreach (var p in profilesReferencingOrgs)
                        p.MyOrganizationId = null;

                    // 1e. Delete the organizations themselves
                    var userOrganizations = await db.MyOrganizations
                        .Where(o => o.Id == command.UserId)
                        .ToListAsync(cancellationToken);
                    db.MyOrganizations.RemoveRange(userOrganizations);

                    _logger.LogInformation("Removing {Count} organization(s) and related data for user {UserId}",
                        userOrganizations.Count, command.UserId);
                }

                // 2. Delete PageRequests owned by this user (not linked to an organization)
                var userPageRequestIds = await db.RequestPages
                    .Where(pr => pr.Id == command.UserId)
                    .Select(pr => pr.AdminPageRequestId)
                    .ToListAsync(cancellationToken);

                if (userPageRequestIds.Count > 0)
                {
                    var prImages = await db.PageRequestImages
                        .Where(pi => userPageRequestIds.Contains(pi.AdminPageRequestId))
                        .ToListAsync(cancellationToken);
                    if (prImages.Count > 0)
                        db.PageRequestImages.RemoveRange(prImages);

                    var pageRequests = await db.RequestPages
                        .Where(pr => userPageRequestIds.Contains(pr.AdminPageRequestId))
                        .ToListAsync(cancellationToken);
                    db.RequestPages.RemoveRange(pageRequests);
                }

                // 3. Delete or nullify comments made by this user on other feeds
                var userComments = await db.NewsFeedComments
                    .IgnoreQueryFilters()
                    .Where(c => c.Id == command.UserId)
                    .ToListAsync(cancellationToken);
                if (userComments.Count > 0)
                    db.NewsFeedComments.RemoveRange(userComments);

                // 4. Delete likes made by this user on other feeds
                var userLikes = await db.Likes
                    .Where(l => l.UserId == command.UserId)
                    .ToListAsync(cancellationToken);
                if (userLikes.Count > 0)
                    db.Likes.RemoveRange(userLikes);

                // 5. Delete ProfileInformation if exists
                if (user.ProfileInformationId.HasValue)
                {
                    var profile = await db.ProfileInformations
                        .FirstOrDefaultAsync(p => p.ProfileInformationId == user.ProfileInformationId.Value, cancellationToken);

                    if (profile is not null)
                        db.ProfileInformations.Remove(profile);
                }

                // 6. Delete TokenInfo for this user
                var tokenInfo = await db.TokenInfos
                    .FirstOrDefaultAsync(t => t.Username == user.UserName, cancellationToken);

                if (tokenInfo is not null)
                    db.TokenInfos.Remove(tokenInfo);

                // 7. Delete notifications for this user
                var notifications = await db.Notifications
                    .Where(n => n.RecipientUserId == command.UserId)
                    .ToListAsync(cancellationToken);

                if (notifications.Count > 0)
                    db.Notifications.RemoveRange(notifications);

                await db.SaveChangesAsync(cancellationToken);

                // 8. Delete the user (Identity handles UserRoles, UserClaims, UserLogins, UserTokens)
                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to delete user {UserId}. Errors: {Errors}", command.UserId, errors);
                    return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, errors);
                }

                _logger.LogInformation("User {UserId} deleted successfully", command.UserId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", command.UserId);
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

