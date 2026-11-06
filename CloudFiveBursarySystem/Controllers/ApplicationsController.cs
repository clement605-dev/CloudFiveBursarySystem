using System.Security.Claims;
using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using CloudFiveBursarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Controls the application submission and tracking process.
    // Only registered applicants can access these features.
    [Authorize(Roles = "Applicant")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLogService;

        // Receives the database context and activity logging service.
        public ApplicationsController(
            ApplicationDbContext context,
            ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        // Displays all applications belonging to the logged-in applicant.
        public async Task<IActionResult> Index()
        {
            // Retrieves the ID of the currently logged-in account.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Finds the applicant profile connected to the account.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Retrieves the applicant's applications and their programme details.
            var applications = await _context.BursaryApplications
                .Include(a => a.BursaryProgramme)
                .Where(a => a.ApplicantId == applicant.ApplicantId)
                .OrderByDescending(a => a.DateSubmitted)
                .ToListAsync();

            return View(applications);
        }

        // Displays the application form for the selected bursary programme.
        [HttpGet]
        public async Task<IActionResult> Create(int? programmeId)
        {
            if (programmeId == null)
            {
                return NotFound();
            }

            // Finds the selected bursary programme.
            var programme = await _context.BursaryProgrammes
                .FirstOrDefaultAsync(p =>
                    p.BursaryProgrammeId == programmeId);

            if (programme == null)
            {
                return NotFound();
            }

            // Checks whether the programme is open and within its application period.
            if (programme.ProgrammeStatus != "Open" ||
                programme.ApplicationOpeningDate.Date > DateTime.Today ||
                programme.ApplicationClosingDate.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] =
                    "This bursary programme is not currently accepting applications.";

                return RedirectToAction("Index", "AvailableBursaries");
            }

            // Passes the selected programme to the Create view.
            ViewBag.Programme = programme;

            return View();
        }

        // Receives and saves a new application submitted by an applicant.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bursaryProgrammeId)
        {
            // Retrieves the logged-in user's Identity account ID.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Finds the applicant profile belonging to the logged-in user.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Finds the bursary programme selected by the applicant.
            var programme = await _context.BursaryProgrammes
                .FirstOrDefaultAsync(p =>
                    p.BursaryProgrammeId == bursaryProgrammeId);

            if (programme == null)
            {
                return NotFound("Bursary programme could not be found.");
            }

            // Confirms that the programme is currently accepting applications.
            if (programme.ProgrammeStatus != "Open" ||
                programme.ApplicationOpeningDate.Date > DateTime.Today ||
                programme.ApplicationClosingDate.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] =
                    "This bursary programme is not currently accepting applications.";

                return RedirectToAction("Index", "AvailableBursaries");
            }

            // Checks whether this applicant has already applied for this programme.
            bool alreadyApplied = await _context.BursaryApplications
                .AnyAsync(a =>
                    a.ApplicantId == applicant.ApplicantId &&
                    a.BursaryProgrammeId == bursaryProgrammeId);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] =
                    "You have already submitted an application for this programme.";

                return RedirectToAction(nameof(Index));
            }

            // Generates a unique reference number for the new application.
            string referenceNumber =
                $"CFB-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            // Creates the application record.
            var application = new BursaryApplication
            {
                ReferenceNumber = referenceNumber,
                ApplicantId = applicant.ApplicantId,
                BursaryProgrammeId = programme.BursaryProgrammeId,
                DateSubmitted = DateTime.Now,
                ApplicationStatus = "Pending"
            };

            // Adds the application to the database.
            _context.BursaryApplications.Add(application);

            // Saves the application permanently in SQL Server.
            await _context.SaveChangesAsync();

            // Records the applicant's successful application submission.
            await _activityLogService.LogActivityAsync(
                "Application Submitted",
                $"Applicant submitted a bursary application. Reference number: {referenceNumber}",
                "BursaryApplication",
                application.BursaryApplicationId);

            TempData["SuccessMessage"] =
                "Your bursary application was submitted successfully.";

            // Opens the details page for the newly submitted application.
            return RedirectToAction(
                nameof(Details),
                new { id = application.BursaryApplicationId });
        }

        // Displays the details of a specific application.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieves the logged-in user's ID.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Finds the applicant profile connected to the account.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Retrieves the selected application only if it belongs to this applicant.
            var application = await _context.BursaryApplications
                .Include(a => a.BursaryProgramme)
                .FirstOrDefaultAsync(a =>
                    a.BursaryApplicationId == id &&
                    a.ApplicantId == applicant.ApplicantId);

            if (application == null)
            {
                return NotFound("Application could not be found.");
            }

            return View(application);
        }
    }
}
