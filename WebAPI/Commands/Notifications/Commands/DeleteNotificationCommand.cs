using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Commands
{
    public record DeleteNotificationCommand(Guid NotifyId) : IRequest<Result>;

    public class DeleteNotificationCommandHandler : AppDatabaseBase, IRequestHandler<DeleteNotificationCommand, Result>
    {
        public DeleteNotificationCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await GetDBContext().Notifications
                .FirstOrDefaultAsync(n => n.NotifyId == request.NotifyId, cancellationToken);

            if (notification is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Notification not found."));

            GetDBContext().Notifications.Remove(notification);
            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

