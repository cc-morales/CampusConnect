using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CamCon.Domain;
using CamCon.Domain.Entity;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Google;
using WebAPI.Commands.Notifications.Events;
using WebAPI.Interfaces;
using WebAPI.NotifyHub;
using WebAPI.Services.NotificationService;

namespace WebAPI.Commands.Notifications.Handlers
{
    public class AdminNotificationHandler(
        IHubContext<NotificationHub> hubContext,
        ILogger<AdminNotificationHandler> logger)
        : INotificationHandler<AdminNotificationEvent>
    {
        public async Task Handle(AdminNotificationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Sending admin notification: {NotificationId}", notification.NotificationId);

                await hubContext.Clients.All.SendAsync("AdminNotification", notification.NotificationId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending admin notification: {NotificationId}", notification.NotificationId);
            }
        }
    }

    public class AllNotificationHandler : INotificationHandler<AllNotificationEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<AllNotificationHandler> _logger;

        public AllNotificationHandler(
            IHubContext<NotificationHub> hubContext,
            ILogger<AllNotificationHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(AllNotificationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Sending notification: {NotificationId}", notification.NotificationId);

                await _hubContext.Clients.All.SendAsync("AllNotification", notification.NotificationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending user notification: {NotificationId}", notification.NotificationId);
            }
        }
    }

    public class UserNotificationHandler(
        IHubContext<NotificationHub> hubContext,
        ILogger<UserNotificationHandler> logger,
        AppDbContext context,
        IMediator mediator)
        : AppDatabaseBase(context), INotificationHandler<UserNotificationEvent>
    {
        public async Task Handle(UserNotificationEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Sending user notification: {NotificationId}", notification.NotificationId);

                if (notification.Fcm is not null)
                {
                    var notify = GetDBContext().Notifications.FirstOrDefault(c => c.NotifyId == notification.NotificationId);

                    if (notify is not null)
                    {
                        if (notify.NotificationType == Enums.NotificationType.PageRequest)
                        {
                            var model = JsonSerializer.Deserialize<AdminPageRequestModel>(notify.DataJson);
                            var pageRequest = GetDBContext().RequestPages.FirstOrDefault(rp => rp.AdminPageRequestId == model.AdminPageRequestId);
                            var message = $"Your request as {(pageRequest.PageRequestType == Enums.PageRequestType.PageRequest? "page admin" : "contributor")} has been {pageRequest.PageRequestStatus.Humanize()}{(pageRequest.PageRequestStatus is Enums.PageRequestStatus.Rejected ? $"\nReason: {pageRequest.Reason}" : "")}";
                        
                            JsonSerializerOptions options = new()
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            };
                            using var httpClient = new HttpClient();

                            string jsonPayload = JsonSerializer.Serialize(new FcmMessage
                            {
                                Message = new MessageContent
                                {
                                    Token = notification.Fcm,
                                    Notification = new NotificationContent
                                    {
                                        Title = "Campus Connect",
                                        Body = message
                                    }
                                }
                            }, options);

                                var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/v1/projects/push-notification-a02c9/messages:send")
                            {
                                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                            };

                            var googleToken = await mediator.Send(new GetGoogleTokenQuery(), cancellationToken);
                             
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", googleToken.Value);

                            var response = await httpClient.SendAsync(request, cancellationToken);
                        }
                    }
                }

                await hubContext.Clients.User(notification.UserId).SendAsync("UserNotification", notification.NotificationId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending user notification: {NotificationId}", notification.NotificationId);
            }
        }
    }
}
