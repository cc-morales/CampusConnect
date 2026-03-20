using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record AddContributorCommand(Contributors Contributor) : IRequest<Result>;

    public class AddContributorCommandHandler : AppDatabaseBase, IRequestHandler<AddContributorCommand, Result>
    {
        public AddContributorCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(AddContributorCommand request, CancellationToken cancellationToken)
        {
            var organization = await GetDBContext().MyOrganizations
                .AsNoTracking()
                .AnyAsync(o => o.MyOrganizationId == request.Contributor.MyOrganizationId, cancellationToken);

            if (!organization)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Organization not found."));

            var alreadyExists = await GetDBContext().Contributors
                .AsNoTracking()
                .AnyAsync(c => c.MyOrganizationId == request.Contributor.MyOrganizationId
                            && c.Id == request.Contributor.Id, cancellationToken);

            if (alreadyExists)
                return Result.Failure(new Error(StatusCodes.Status409Conflict, "User is already a contributor of this organization."));

            GetDBContext().Contributors.Add(request.Contributor);
            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

