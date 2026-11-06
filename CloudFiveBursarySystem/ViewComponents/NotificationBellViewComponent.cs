using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Services;
using CloudFiveBursarySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CloudFiveBursarySystem.ViewComponents
{
    // Displays the applicant's notification bell in the navigation bar.
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public NotificationBellViewComponent(
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Only applicants should see the notification bell.
            if (User.Identity?.IsAuthenticated != true ||
                !User.IsInRole("Applicant"))
            {
                return Content(string.Empty);
            }

            // Gets the ID of the currently logged-in user.
            var userId = UserClaimsPrincipal.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Content(string.Empty);
            }

            // Finds the applicant's profile in the database.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return Content(string.Empty);
            }

            // Retrieves unread count and recent notifications.
            var viewModel = new NotificationBellViewModel
            {
                UnreadCount = await _notificationService
                    .GetUnreadCountAsync(applicant.ApplicantId),

                RecentNotifications = await _notificationService
                    .GetRecentNotificationsAsync(applicant.ApplicantId, 5)
            };

            return View(viewModel);
        }
    }
}