using CamCon.Domain;
using CamCon.Domain.Entity;
using CamCon.Shared;
using CloneExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Notifications.Events;
using WebAPI.Commands.Organizations.Commands;
using WebAPI.Commands.Users.Commands.ProfileCommands;
using WebAPI.Interfaces;

namespace WebAPI.Commands.AdminPageRequests.Commands
{
    public record UpdateRequestAdminCommand(AdminPageRequestModel Request, Guid NotificationId) : IRequest<Result>;

    public class UpdateRequestAdminCommandHandler(AppDbContext context, IMediator mediator)
        : AppDatabaseBase(context), IRequestHandler<UpdateRequestAdminCommand, Result>
    {
        public async Task<Result> Handle(UpdateRequestAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pageRequest = await GetDBContext().RequestPages.FirstOrDefaultAsync(rp => rp.AdminPageRequestId == request.Request.AdminPageRequestId);

                var notify = await GetDBContext().Notifications.FirstOrDefaultAsync(rp => rp.NotifyId == request.NotificationId);

                if (pageRequest is null || notify is null)
                    return Result.Failure(new Error(StatusCodes.Status404NotFound, "Page request not found."));

                if (request.Request.MyOrganization is not null)
                    GetDBContext().Entry(request.Request.MyOrganization).State = EntityState.Unchanged;
                if(request.Request.User is not null)
                    GetDBContext().Entry(request.Request.User).State = EntityState.Unchanged;

                pageRequest.PageRequestStatus = request.Request.PageRequestStatus;
                pageRequest.Reason = request.Request.Reason;

                notify.UpdatedAt = DateTime.UtcNow;
                notify.DataJson = JsonSerializer.Serialize(pageRequest);

                await GetDBContext().SaveChangesAsync(cancellationToken);

                //Create org
                if (pageRequest.PageRequestStatus is Enums.PageRequestStatus.Approved)
                {
                    if (pageRequest.PageRequestType is Enums.PageRequestType.PageRequest)
                    {
                        var cloneRequest = pageRequest.GetClone();
                        var newOrg = new MyOrganizationModel
                        {
                            OrganizationName = cloneRequest.OrganizationName,
                            OrganizationType = Enums.OrganizationType.Organization,
                            Id = cloneRequest.Id,
                            User = request.Request.User,
                            OrganizationDepartment = new OrganizationDepartmentModel() { MyOrganizationId = request.Request.Department.MyOrganizationId }
                        };

                        await mediator.Send(new CreateOrganizationCommand(newOrg), cancellationToken);
                    }
                    else if (pageRequest.PageRequestType is Enums.PageRequestType.Contributor)
                    {
                        var contributor = new Contributors
                        {
                            ContributorsId = Guid.NewGuid(),
                            MyOrganizationId = pageRequest.MyOrganizationId ?? Guid.Empty,
                            Id = pageRequest.Id ?? string.Empty
                        };

                        await mediator.Send(new AddContributorCommand(contributor), cancellationToken);
                    }
                }
                
                var fcm = GetDBContext().DeviceFcms.FirstOrDefault(c => c.UserId == request.Request.User.Id);

                await mediator.Publish(new UserNotificationEvent(notify.NotifyId, request.Request.Id, fcm.FcmToken), cancellationToken);

                var profile = await GetDBContext().ProfileInformations.FirstOrDefaultAsync(p => p.ProfileInformationId == request.Request.User.ProfileInformationId, cancellationToken: cancellationToken);

                if(profile is not null)
                    await mediator.Send(new UserUpdateProfileCommand(profile, true), cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
