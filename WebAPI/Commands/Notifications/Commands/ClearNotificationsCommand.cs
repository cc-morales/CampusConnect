using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Commands
{
    public record ClearNotificationsCommand(string RecipientUserId) : IRequest<Result>;

    public class ClearNotificationsCommandHandler : AppDatabaseBase, IRequestHandler<ClearNotificationsCommand, Result>
    {
        public ClearNotificationsCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(ClearNotificationsCommand request, CancellationToken cancellationToken)
        {
            var notifications = await GetDBContext().Notifications
                .Where(n => n.RecipientUserId == request.RecipientUserId)
                .ToListAsync(cancellationToken);

            if (notifications.Count == 0)
                return Result.Success();

            GetDBContext().Notifications.RemoveRange(notifications);
            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

