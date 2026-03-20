using CamCon.Shared;
using Domain.Models;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task<NotifyModel> GetByIdAsync(Guid notifyId);
        Task<List<NotifyModel>> GetAll();
        Task<List<NotifyModel>> GetByRecipientAsync(string recipientUserId);
        Task<Result> DeleteNotificationAsync(Guid notifyId);
        Task<Result> ClearAllNotificationsAsync(string recipientUserId);
        Task<Result> ClearAllAsync();
    }
}
