using CamCon.Shared;
using Domain.Models;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task<NotifyModel> GetByIdAsync(Guid notifyId);
        Task<List<NotifyModel>> GetAll();
        Task<List<NotifyModel>> GetByRecipientAsync(string recipientUserId);
        Task<Result> DeleteNotificationAsync(Guid notifyId, bool isAdmin = false);
        Task<Result> ClearAllNotificationsAsync(string recipientUserId);
        Task<Result> ClearAllAsync();
        Task RegisterFcm(string fcm, string userId);
    }
}
