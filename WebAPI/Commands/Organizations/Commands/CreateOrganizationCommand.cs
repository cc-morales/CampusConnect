using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record class CreateOrganizationCommand(MyOrganizationModel Organization) : IRequest<Result>;

    public class CreateCommandHandler : AppDatabaseBase, IRequestHandler<CreateOrganizationCommand, Result>
    {
        public CreateCommandHandler(AppDbContext context)  : base(context) { }
        public async Task<Result> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            if (request.Organization.OrganizationDepartment is not null)
            {
                GetDBContext().Entry(request.Organization.OrganizationDepartment).State = EntityState.Added;

                request.Organization.OrganizationDepartmentId = request.Organization.OrganizationDepartment.OrganizationDepartmentId;
            }

            GetDBContext().MyOrganizations.Add(request.Organization);

            if (request.Organization.User is not null)
            {
                GetDBContext().Entry(request.Organization.User).State = EntityState.Unchanged;
            }


            await GetDBContext().SaveChangesAsync();

            return Result.Success();
        }
    }
}
