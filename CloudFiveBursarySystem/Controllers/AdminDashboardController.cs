using CloudFiveBursarySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Restricts access to users assigned the Administrator role.
    [Authorize(Roles = "Administrator")]
    public class AdminDashboardController : Controller
    {
        // Provides access to the application's database tables.
        private readonly ApplicationDbContext _context;

        // Manages user accounts and their assigned roles.
        private readonly UserManager<IdentityUser> _userManager;

        // Receives the database context and user manager through dependency injection.
        public AdminDashboardController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Retrieves the account IDs of all registered administrators.
        private async Task<List<string>> GetAdministratorIdsAsync()
        {
            var administrators = await _userManager
                .GetUsersInRoleAsync("Administrator");

            return administrators.Select(user => user.Id).ToList();
        }

        // Displays the administrator dashboard and its summary information.
        public async Task<IActionResult> Index()
        {
            var administratorIds = await GetAdministratorIdsAsync();

            // Counts applicant profiles without including administrator accounts.
            ViewBag.TotalApplicants = await _context.Applicants
                .CountAsync(a => !administratorIds.Contains(a.UserId));

            // Counts all bursary programmes.
            ViewBag.TotalProgrammes = await _context.BursaryProgrammes
                .CountAsync();

            // Counts applications awaiting review.
            ViewBag.PendingApplications = await _context.BursaryApplications
                .CountAsync(a => a.ApplicationStatus == "Pending");

            // Counts approved applications.
            ViewBag.ApprovedApplications = await _context.BursaryApplications
                .CountAsync(a => a.ApplicationStatus == "Approved");


            // ==========================================
            // RECENT APPLICATIONS
            // ==========================================

            // Retrieves the five most recently submitted applications.
            // Applicant and programme details are included for display.
            ViewBag.RecentApplications = await _context.BursaryApplications
                .Include(a => a.Applicant)
                .Include(a => a.BursaryProgramme)
                .OrderByDescending(a => a.DateSubmitted)
                .Take(5)
                .Select(a => new
                {
                    ReferenceNumber = a.ReferenceNumber,
                    ApplicantName = a.Applicant!.FirstName + " " +
                                    a.Applicant.LastName,
                    ProgrammeName = a.BursaryProgramme!.ProgrammeName,
                    DateSubmitted = a.DateSubmitted,
                    ApplicationStatus = a.ApplicationStatus
                })
                .ToListAsync();


            // ==========================================
            // RECENT BENEFICIARIES
            // ==========================================

            // Retrieves the five most recently registered beneficiaries.
            // Related applicant and programme information is included.
            ViewBag.RecentBeneficiaries = await _context.Beneficiaries
                .Include(b => b.BursaryApplication)
                    .ThenInclude(a => a!.Applicant)
                .Include(b => b.BursaryApplication)
                    .ThenInclude(a => a!.BursaryProgramme)
                .OrderByDescending(b => b.DateAdded)
                .Take(5)
                .Select(b => new
                {
                    ApplicantName =
                        b.BursaryApplication!.Applicant!.FirstName + " " +
                        b.BursaryApplication.Applicant.LastName,

                    ProgrammeName =
                        b.BursaryApplication.BursaryProgramme!.ProgrammeName,

                    FundingAmount = b.FundingAmount,
                    DateAdded = b.DateAdded,
                    BeneficiaryStatus = b.BeneficiaryStatus
                })
                .ToListAsync();


            // ==========================================
            // RECENT PAYMENTS
            // ==========================================

            // Retrieves the five most recently recorded payments.
            // Payment information is connected to its beneficiary,
            // applicant and bursary programme.
            ViewBag.RecentPayments = await _context.Payments
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.Applicant)
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.BursaryProgramme)
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .Select(p => new
                {
                    PaymentReference = p.PaymentReference,

                    ApplicantName =
                        p.Beneficiary!.BursaryApplication!.Applicant!.FirstName + " " +
                        p.Beneficiary.BursaryApplication.Applicant.LastName,

                    ProgrammeName =
                        p.Beneficiary.BursaryApplication.BursaryProgramme!.ProgrammeName,

                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus
                })
                .ToListAsync();


            // Displays the dashboard with its statistics and recent records.
            return View();
        }


        // Displays all registered applicants to the administrator.
        public async Task<IActionResult> Applicants()
        {
            var administratorIds = await GetAdministratorIdsAsync();

            var applicants = await _context.Applicants
                .Where(a => !administratorIds.Contains(a.UserId))
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();

            return View(applicants);
        }


        // Displays all beneficiaries registered in the system.
        public async Task<IActionResult> Beneficiaries()
        {
            // Retrieves beneficiary records together with their related
            // application, applicant and bursary programme information.
            var beneficiaries = await _context.Beneficiaries
                .Include(b => b.BursaryApplication)
                    .ThenInclude(a => a!.Applicant)
                .Include(b => b.BursaryApplication)
                    .ThenInclude(a => a!.BursaryProgramme)
                .OrderByDescending(b => b.DateAdded)
                .ToListAsync();

            // Sends the beneficiary records to the Beneficiaries view.
            return View(beneficiaries);
        }
    }
}