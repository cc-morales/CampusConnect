using CamCon.Domain.Entity;
using Microsoft.AspNetCore.SignalR;
using WebAPI.NotifyHub;

namespace WebAPI.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToAllAsync(Guid id)
        {
            await _hubContext.Clients.All.SendAsync("UserNotification", id);
        }
        public async Task SendToAdmin(Guid id)
        {
            await _hubContext.Clients.All.SendAsync("AdminNotification", id);
        }
    }
}
