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
            await GetDBContext().Notifications
                .Where(n => !n.IsAdminDeleted)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsAdminDeleted, true), cancellationToken);

            return Result.Success();
        }
    }
}
