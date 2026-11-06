using System.Security.Claims;
using CloudFiveBursarySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // This controller manages the applicant's dashboard.
    // Only users with the Applicant role are allowed to access it.
    [Authorize(Roles = "Applicant")]
    public class ApplicantDashboardController : Controller
    {
        // Provides access to the application's SQL Server database.
        private readonly ApplicationDbContext _context;

        // Constructor: receives the database context through dependency injection.
        public ApplicantDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApplicantDashboard/Index
        // Displays the dashboard for the currently logged-in applicant.
        public async Task<IActionResult> Index()
        {
            // Gets the unique Identity user ID of the logged-in account.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Checks whether the logged-in account has a valid user ID.
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Searches the Applicants table for the profile linked to this account.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            // Returns an error if no applicant profile is linked to the account.
            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Counts the applications belonging to the logged-in applicant.
            ViewBag.ApplicationCount = await _context.BursaryApplications
                .CountAsync(a => a.ApplicantId == applicant.ApplicantId);

            // Counts bursary programmes whose status is currently Open.
            ViewBag.OpenProgrammeCount = await _context.BursaryProgrammes
                .CountAsync(p => p.ProgrammeStatus == "Open");

            // Sends the applicant's information to the dashboard view.
            return View(applicant);
        }
    }
}