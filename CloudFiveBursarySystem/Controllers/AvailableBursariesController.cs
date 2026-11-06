using CloudFiveBursarySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Allows registered applicants to browse available bursary programmes.
    // Applicants can view programme information but cannot modify programmes.
    [Authorize(Roles = "Applicant")]
    public class AvailableBursariesController : Controller
    {
        // Provides access to the bursary programme records in SQL Server.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public AvailableBursariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays bursary programmes that are open for applications.
        public async Task<IActionResult> Index()
        {
            // Gets today's date to check the application period.
            var today = DateTime.Today;

            // Retrieves programmes that are marked Open and within
            // their application opening and closing dates.
            var availableProgrammes = await _context.BursaryProgrammes
                .Where(p =>
                    p.ProgrammeStatus == "Open" &&
                    p.ApplicationOpeningDate.Date <= today &&
                    p.ApplicationClosingDate.Date >= today)
                .OrderBy(p => p.ApplicationClosingDate)
                .ToListAsync();

            // Sends the available programmes to the applicant's view.
            return View(availableProgrammes);
        }

        // Displays detailed information about a selected bursary programme.
        public async Task<IActionResult> Details(int? id)
        {
            // Checks whether a programme ID was provided.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieves the selected programme from the database.
            var programme = await _context.BursaryProgrammes
                .FirstOrDefaultAsync(p =>
                    p.BursaryProgrammeId == id &&
                    p.ProgrammeStatus == "Open" &&
                    p.ApplicationOpeningDate.Date <= DateTime.Today &&
                    p.ApplicationClosingDate.Date >= DateTime.Today);

            // Returns an error if the programme is unavailable or does not exist.
            if (programme == null)
            {
                return NotFound();
            }

            // Displays the selected programme's details.
            return View(programme);
        }
    }
}