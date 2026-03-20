using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Commands
{
    public record ClearAllNotificationsCommand : IRequest<Result>;

    public class ClearAllNotificationsCommandHandler : AppDatabaseBase, IRequestHandler<ClearAllNotificationsCommand, Result>
    {
        public ClearAllNotificationsCommandHandler(AppDbContext context) : base(context) { }

        public async Task<Result> Handle(ClearAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            var notifications = await GetDBContext().Notifications.ToListAsync(cancellationToken);

            if (notifications.Count == 0)
                return Result.Success();

            GetDBContext().Notifications.RemoveRange(notifications);
            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

