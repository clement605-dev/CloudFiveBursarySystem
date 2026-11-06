using CloudFiveBursarySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("AdminActivityLogs")]
    public class AdminActivityLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminActivityLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? entityType,
            string? userRole)
        {
            var activities = _context.ActivityLogs.AsQueryable();

            // Search by user email, role, action or description.
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                activities = activities.Where(a =>
                    a.UserEmail.Contains(searchTerm) ||
                    a.UserRole.Contains(searchTerm) ||
                    a.Action.Contains(searchTerm) ||
                    a.Description.Contains(searchTerm));
            }

            // Filter activities by system module.
            if (!string.IsNullOrWhiteSpace(entityType))
            {
                activities = activities.Where(a =>
                    a.EntityType == entityType);
            }

            // Filter activities by user role.
            if (!string.IsNullOrWhiteSpace(userRole))
            {
                activities = activities.Where(a =>
                    a.UserRole == userRole);
            }

            var activityList = await activities
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.EntityType = entityType;
            ViewBag.UserRole = userRole;

            // Load available modules for the filter dropdown.
            ViewBag.EntityTypes = await _context.ActivityLogs
                .Where(a => a.EntityType != null && a.EntityType != "")
                .Select(a => a.EntityType!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Load available user roles for the filter dropdown.
            ViewBag.UserRoles = await _context.ActivityLogs
                .Where(a => a.UserRole != "")
                .Select(a => a.UserRole)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            return View("~/Views/AdminActivityLogs/Index.cshtml", activityList);
        }
    }
}