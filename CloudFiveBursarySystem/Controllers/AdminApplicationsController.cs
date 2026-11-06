using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using CloudFiveBursarySystem.Services;
using CloudFiveBursarySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Only users assigned the Administrator role can manage applications.
    [Authorize(Roles = "Administrator")]
    public class AdminApplicationsController : Controller
    {
        // Provides access to application, applicant, programme and beneficiary records.
        private readonly ApplicationDbContext _context;

        // Handles automatic notifications sent to applicants.
        private readonly NotificationService _notificationService;

        // Receives required services through dependency injection.
        public AdminApplicationsController(
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Displays all bursary applications submitted by applicants.
        public async Task<IActionResult> Index()
        {
            // Retrieves applications together with their related applicant
            // and bursary programme information.
            var applications = await _context.BursaryApplications
                .Include(a => a.Applicant)
                .Include(a => a.BursaryProgramme)
                .OrderByDescending(a => a.DateSubmitted)
                .ToListAsync();

            // Retrieves the IDs of applications already registered as beneficiaries.
            var registeredApplicationIds = await _context.Beneficiaries
                .Select(b => b.BursaryApplicationId)
                .ToListAsync();

            // Makes these IDs available to the application list view.
            ViewBag.RegisteredApplicationIds = registeredApplicationIds;

            // Displays the application records in the administrator's view.
            return View(applications);
        }

        // Displays the details of a selected application for review.
        public async Task<IActionResult> Review(int? id)
        {
            // Checks whether an application ID was supplied.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieves the selected application and its related records.
            var application = await _context.BursaryApplications
                .Include(a => a.Applicant)
                .Include(a => a.BursaryProgramme)
                .FirstOrDefaultAsync(a => a.BursaryApplicationId == id);

            // Stops if the requested application does not exist.
            if (application == null)
            {
                return NotFound();
            }

            // Converts the database record into a ViewModel.
            var viewModel = new ReviewApplicationViewModel
            {
                BursaryApplicationId = application.BursaryApplicationId,
                ReferenceNumber = application.ReferenceNumber,
                ApplicantName = application.Applicant == null
                    ? "Applicant unavailable"
                    : $"{application.Applicant.FirstName} {application.Applicant.LastName}",
                ApplicantEmail = application.Applicant?.Email ?? "Not available",
                ProgrammeName = application.BursaryProgramme?.ProgrammeName
                    ?? "Programme unavailable",
                DateSubmitted = application.DateSubmitted,
                ApplicationStatus = application.ApplicationStatus,
                AdministratorComments = application.AdministratorComments
            };

            return View(viewModel);
        }

        // Receives the administrator's decision and saves the review.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ReviewApplicationViewModel model)
        {
            // Defines the statuses administrators are allowed to select.
            string[] allowedStatuses =
            {
                "Pending",
                "Under Review",
                "Approved",
                "Rejected",
                "More Information Required"
            };

            // Prevents unsupported application statuses from being saved.
            if (!allowedStatuses.Contains(model.ApplicationStatus))
            {
                ModelState.AddModelError(
                    nameof(model.ApplicationStatus),
                    "Please select a valid application status.");
            }

            // Redisplays the form if validation fails.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Finds the application being reviewed.
            var application = await _context.BursaryApplications
                .FirstOrDefaultAsync(a =>
                    a.BursaryApplicationId == model.BursaryApplicationId);

            // Checks whether the application still exists.
            if (application == null)
            {
                return NotFound();
            }

            // Stores the previous status so we can detect an actual change.
            string previousStatus = application.ApplicationStatus;

            // Checks whether the administrator selected a different status.
            bool statusChanged = previousStatus != model.ApplicationStatus;

            // Updates the application status.
            application.ApplicationStatus = model.ApplicationStatus;

            // Saves the administrator's feedback.
            application.AdministratorComments = model.AdministratorComments;

            // Saves the application changes to SQL Server.
            await _context.SaveChangesAsync();

            // Only send a notification when the application status changes.
            if (statusChanged)
            {
                string notificationTitle;
                string notificationMessage;

                // Prepares a suitable message for the applicant.
                switch (application.ApplicationStatus)
                {
                    case "Approved":
                        notificationTitle = "Application Approved!";
                        notificationMessage =
                            "Congratulations! Your bursary application has been approved.";
                        break;

                    case "Rejected":
                        notificationTitle = "Application Update";
                        notificationMessage =
                            "Your bursary application has been reviewed and was not approved.";
                        break;

                    case "More Information Required":
                        notificationTitle = "Additional Information Required";
                        notificationMessage =
                            "The bursary administrator requires additional information or documents from you. Please check your application.";
                        break;

                    case "Under Review":
                        notificationTitle = "Application Under Review";
                        notificationMessage =
                            "Your bursary application is currently being reviewed by the administrator.";
                        break;

                    default:
                        notificationTitle = "Application Status Updated";
                        notificationMessage =
                            $"Your bursary application status has been updated to {application.ApplicationStatus}.";
                        break;
                }

                // Includes the administrator's comments when provided.
                if (!string.IsNullOrWhiteSpace(application.AdministratorComments))
                {
                    notificationMessage +=
                        "\n\nAdministrator's comments: " +
                        application.AdministratorComments;
                }

                // Creates a notification for the applicant.
                await _notificationService.CreateNotificationAsync(
                    application.ApplicantId,
                    notificationTitle,
                    notificationMessage,
                    "Application");
            }

            TempData["SuccessMessage"] =
                "The application review has been saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Registers an approved application as a bursary beneficiary.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsBeneficiary(int id)
        {
            // Retrieves the selected application and its bursary programme.
            var application = await _context.BursaryApplications
                .Include(a => a.BursaryProgramme)
                .FirstOrDefaultAsync(a => a.BursaryApplicationId == id);

            // Checks whether the application exists.
            if (application == null)
            {
                return NotFound();
            }

            // Only approved applications can become beneficiaries.
            if (application.ApplicationStatus != "Approved")
            {
                TempData["ErrorMessage"] =
                    "Only approved applications can be registered as beneficiaries.";

                return RedirectToAction(nameof(Index));
            }

            // Checks whether this application has already been registered.
            bool alreadyRegistered = await _context.Beneficiaries
                .AnyAsync(b => b.BursaryApplicationId == id);

            if (alreadyRegistered)
            {
                TempData["ErrorMessage"] =
                    "This application has already been registered as a beneficiary.";

                return RedirectToAction(nameof(Index));
            }

            // Creates a beneficiary record using the approved application.
            var beneficiary = new Beneficiary
            {
                BursaryApplicationId = application.BursaryApplicationId,
                DateAdded = DateTime.Now,
                BeneficiaryStatus = "Active",

                // Copies the funding amount from the bursary programme.
                FundingAmount = application.BursaryProgramme!.FundingAmount
            };

            // Adds the new beneficiary record to the database.
            _context.Beneficiaries.Add(beneficiary);

            // Saves the beneficiary record in SQL Server.
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "The approved applicant has been successfully registered as a beneficiary.";

            // Returns the administrator to the application list.
            return RedirectToAction(nameof(Index));
        }
    }
}
