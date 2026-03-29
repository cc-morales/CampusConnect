using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record ToggleFollowCommand(Guid MyOrganizationId, string UserId) : IRequest<Result>;

    public class ToggleFollowCommandHandler : AppDatabaseBase, IRequestHandler<ToggleFollowCommand, Result>
    {
        public ToggleFollowCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(ToggleFollowCommand request, CancellationToken cancellationToken)
        {
            var organization = await GetDBContext().MyOrganizations
                .Include(o => o.Followers)
                .FirstOrDefaultAsync(o => o.MyOrganizationId == request.MyOrganizationId, cancellationToken);

            if (organization is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Organization not found."));

            var existing = organization.Followers.FirstOrDefault(f => f.Id == request.UserId);

            if (existing is not null)
                organization.Followers.Remove(existing);
            else
                organization.Followers.Add(new Follower
                {
                    MyOrganizationId = request.MyOrganizationId,
                    Id = request.UserId
                });

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}


