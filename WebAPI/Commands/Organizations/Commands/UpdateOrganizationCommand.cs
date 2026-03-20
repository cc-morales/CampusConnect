using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record class UpdateOrganizationCommand(MyOrganizationModel Request) : IRequest<Result>;

    public class UpdateOrganizationCommandHandler : AppDatabaseBase, IRequestHandler<UpdateOrganizationCommand, Result>
    {
        public UpdateOrganizationCommandHandler(AppDbContext context)  : base(context) { }
        public async Task<Result> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var existingOrg = await GetDBContext().MyOrganizations.FirstOrDefaultAsync(o => o.MyOrganizationId == request.Request.MyOrganizationId, cancellationToken: cancellationToken);

            if(existingOrg is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound,"Organization not found"));

            existingOrg.OrganizationName = request.Request.OrganizationName;
            existingOrg.OrganizationType = request.Request.OrganizationType;
            existingOrg.Description = request.Request.Description;
            existingOrg.Photo = request.Request.Photo;
            existingOrg.CoverPhoto = request.Request.CoverPhoto;

            if (request.Request.User is not null)
            {
                existingOrg.User = request.Request.User;
                existingOrg.Id = request.Request.Id;
            }

            if(request.Request.OrganizationDepartment is not null)
            {
                existingOrg.OrganizationDepartmentId = request.Request.OrganizationDepartment.OrganizationDepartmentId;
                existingOrg.OrganizationDepartment = request.Request.OrganizationDepartment;
            }


            if (existingOrg.User is not null)
            {
                GetDBContext().Entry(existingOrg.User).State = EntityState.Unchanged;
            }

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
