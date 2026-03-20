using CamCon.Domain;
using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using System.Text.Json;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Notifications.Events;
using WebAPI.Commands.Notifications.Handlers;
using WebAPI.Interfaces;
using WebAPI.Services.NotificationService;

namespace WebAPI.Commands.Notifications.Commands
{
    public record CreateNotificationCommand(NotifyModel Notify) : IRequest<Result<Guid>>;

    public class CreateNotificationCommandHandler : AppDatabaseBase, IRequestHandler<CreateNotificationCommand, Result<Guid>>
    {
        private readonly INotificationService notificationService;
        private readonly IMediator mediator;

        public CreateNotificationCommandHandler(AppDbContext context, INotificationService notificationService, IMediator mediator) : base(context)
        {
            this.notificationService = notificationService;
            this.mediator = mediator;

        }
        public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await GetDBContext().Notifications.AddAsync(request.Notify, cancellationToken);
                
                await GetDBContext().SaveChangesAsync(cancellationToken);

                await mediator.Publish(new AdminNotificationEvent(request.Notify.NotifyId), cancellationToken);

                return Result.Success(request.Notify.NotifyId);
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(new Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
