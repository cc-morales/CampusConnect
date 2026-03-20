using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Queries
{
    public record GetByIdReciepientNotificationQuery(string RecipientId) : IRequest<Result<List<NotifyModel>>>;

    public class GetByIdReciepientNotificationQueryHandler : AppDatabaseBase, IRequestHandler<GetByIdReciepientNotificationQuery, Result<List<NotifyModel>>>
    {
        public GetByIdReciepientNotificationQueryHandler(AppDbContext context) : base(context) { }

        public async Task<Result<List<NotifyModel>>> Handle(GetByIdReciepientNotificationQuery request, CancellationToken cancellationToken)
        {
            var notifications = await GetDBContext().Notifications
                .Where(c => c.RecipientUserId == request.RecipientId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            if (notifications is null)
                return Result.Failure<List<NotifyModel>>(new Error(404, "Notification not found."));

            return Result.Success(notifications);
        }
    }
}
