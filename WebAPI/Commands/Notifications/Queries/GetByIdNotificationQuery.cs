using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Queries
{
    public record GetByIdNotificationQuery(Guid notifyId) : IRequest<Result<NotifyModel>>;

    public class GetByIdNotificationQueryHandler : AppDatabaseBase, IRequestHandler<GetByIdNotificationQuery, Result<NotifyModel>>
    {
        public GetByIdNotificationQueryHandler(AppDbContext context) : base(context) { }

        public async Task<Result<NotifyModel>> Handle(GetByIdNotificationQuery request, CancellationToken cancellationToken)
        {
            var notification = await GetDBContext().Notifications.FirstOrDefaultAsync(c => c.NotifyId == request.notifyId);

            if (notification is null)
                return Result.Failure<NotifyModel>(new Error(404, "Notification not found."));

            return Result.Success(notification);
        }
    }
}
