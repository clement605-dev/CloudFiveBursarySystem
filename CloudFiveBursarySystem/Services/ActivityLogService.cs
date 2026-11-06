using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CloudFiveBursarySystem.Services
{
    // Handles the recording of activities performed by applicants and administrators.
    public class ActivityLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityLogService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Records an activity performed by the currently logged-in user.
        public async Task LogActivityAsync(
            string action,
            string description,
            string? entityType = null,
            int? entityId = null)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // Do not record an activity if no authenticated user exists.
            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                return;
            }

            // Get the user's account information.
            string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;

            string email = user.FindFirstValue(ClaimTypes.Email)
                ?? user.Identity.Name
                ?? "Unknown user";

            // Identify whether the user is an Administrator or Applicant.
            string userRole = user.IsInRole("Administrator")
                ? "Administrator"
                : user.IsInRole("Applicant")
                    ? "Applicant"
                    : "Unknown";

            // Create a new activity log record.
            var activity = new ActivityLog
            {
                UserId = userId,
                UserEmail = email,
                UserRole = userRole,
                Action = action,
                Description = description,
                EntityType = entityType,
                EntityId = entityId,
                DateCreated = DateTime.Now
            };

            // Save the activity to SQL Server.
            _context.ActivityLogs.Add(activity);

            await _context.SaveChangesAsync();
        }
    }
}