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
            await GetDBContext().Notifications
                .Where(n => n.RecipientUserId == request.RecipientUserId && !n.IsUserDeleted)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsUserDeleted, true), cancellationToken);

            return Result.Success();
        }
    }
}
