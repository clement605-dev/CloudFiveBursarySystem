using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Services
{
    // Central service responsible for managing applicant notifications.
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Creates a new notification for a specific applicant.
        public async Task CreateNotificationAsync(
            int applicantId,
            string title,
            string message,
            string notificationType,
            string? actionUrl = null)
        {
            var notification = new Notification
            {
                ApplicantId = applicantId,
                Title = title,
                Message = message,
                NotificationType = notificationType,
                IsRead = false,
                DateCreated = DateTime.Now,
                ActionUrl = actionUrl
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        // Retrieves the latest notifications belonging to an applicant.
        public async Task<List<Notification>> GetRecentNotificationsAsync(
            int applicantId,
            int count = 5)
        {
            return await _context.Notifications
                .Where(n => n.ApplicantId == applicantId)
                .OrderByDescending(n => n.DateCreated)
                .Take(count)
                .ToListAsync();
        }

        // Counts unread notifications for a specific applicant.
        public async Task<int> GetUnreadCountAsync(int applicantId)
        {
            return await _context.Notifications
                .CountAsync(n =>
                    n.ApplicantId == applicantId &&
                    !n.IsRead);
        }

        // Marks a notification as read.
        // The applicant ID check prevents users from modifying
        // notifications belonging to other applicants.
        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int applicantId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.ApplicantId == applicantId);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // Marks all notifications belonging to an applicant as read.
        public async Task MarkAllAsReadAsync(int applicantId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n =>
                    n.ApplicantId == applicantId &&
                    !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}