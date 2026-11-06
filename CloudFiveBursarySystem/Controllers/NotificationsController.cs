using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CloudFiveBursarySystem.Controllers
{
    // Only authenticated applicants can access their notifications.
    [Authorize(Roles = "Applicant")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public NotificationsController(
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Displays all notifications belonging to the logged-in applicant.
        public async Task<IActionResult> Index()
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            var notifications = await _notificationService
                .GetRecentNotificationsAsync(applicantId.Value, 100);

            // Show the unread notification count on the page.
            ViewBag.UnreadCount = await _notificationService
                .GetUnreadCountAsync(applicantId.Value);

            return View(notifications);
        }

        // Marks one notification as read.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
            {
                return NotFound();
            }

            await _notificationService.MarkAsReadAsync(
                id,
                applicantId.Value);

            return RedirectToAction(nameof(Index));
        }

        // Marks all notifications belonging to the applicant as read.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
            {
                return NotFound();
            }

            await _notificationService.MarkAllAsReadAsync(
                applicantId.Value);

            TempData["SuccessMessage"] =
                "All notifications have been marked as read.";

            return RedirectToAction(nameof(Index));
        }

        // Marks a notification as read and opens its related page.
        public async Task<IActionResult> Open(int id)
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == id &&
                    n.ApplicantId == applicantId.Value);

            if (notification == null)
            {
                return NotFound();
            }

            await _notificationService.MarkAsReadAsync(
                id,
                applicantId.Value);

            // Only allow internal application links.
            if (!string.IsNullOrWhiteSpace(notification.ActionUrl) &&
                Url.IsLocalUrl(notification.ActionUrl))
            {
                return LocalRedirect(notification.ActionUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        // Finds the applicant profile linked to the logged-in account.
        private async Task<int?> GetCurrentApplicantIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return applicant?.ApplicantId;
        }
    }
}
