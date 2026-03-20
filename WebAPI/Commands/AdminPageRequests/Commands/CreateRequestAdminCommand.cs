using CamCon.Domain;
using CamCon.Domain.Entity;
using CamCon.Shared;
using CloneExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Notifications.Commands;
using WebAPI.Commands.Organizations.Commands;
using WebAPI.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Commands.AdminPageRequests.Commands
{
    public record CreateRequestAdminCommand(AdminPageRequestModel Request) : IRequest<Result>;

    public class CreateRequestAdminCommandHandler(AppDbContext context, IMediator mediator)
        : AppDatabaseBase(context), IRequestHandler<CreateRequestAdminCommand, Result>
    {
        public async Task<Result> Handle(CreateRequestAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if(request.Request.User is not null)
                    GetDBContext().Entry(request.Request.User).State = EntityState.Unchanged;
                if (request.Request.MyOrganization is not null)
                    GetDBContext().Entry(request.Request.MyOrganization).State = EntityState.Unchanged;
                if(request.Request.Department is not null)
                    GetDBContext().Entry(request.Request.Department).State = EntityState.Added;

                if (request.Request.PageRequestImages is not null)
                {
                    foreach (var image in request.Request.PageRequestImages)
                    {
                        //image.AdminPageRequestId = request.Request.AdminPageRequestId;
                        GetDBContext().Entry(image).State = EntityState.Added;
                    }
                }

                await GetDBContext().RequestPages.AddAsync(request.Request, cancellationToken);

                await GetDBContext().SaveChangesAsync(cancellationToken);

                var cloneRequest = request.Request.GetClone();

                if (cloneRequest.User?.ProfileInformation is not null)
                {
                    cloneRequest.User.ProfileInformation.ProfilePicture = null;
                    cloneRequest.User.ProfileInformation.CoverPicture = null;
                    cloneRequest.PageRequestImages = null;
                }

                //Notify
                var notification = new NotifyModel<AdminPageRequestModel>
                {
                    NotificationType = Enums.NotificationType.PageRequest,
                    DataJson = JsonSerializer.Serialize(cloneRequest),
                    RecipientUserId = cloneRequest.Id!,
                };
                await mediator.Send(new CreateNotificationCommand(notification), cancellationToken);

                return Result.Success("Admin request created successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
