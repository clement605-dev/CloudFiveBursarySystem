using CloudFiveBursarySystem.Models;

namespace CloudFiveBursarySystem.ViewModels
{
    // Holds the information displayed in the navigation notification bell.
    public class NotificationBellViewModel
    {
        public int UnreadCount { get; set; }

        public List<Notification> RecentNotifications { get; set; } = new();
    }
}