using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using CloudFiveBursarySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CloudFiveBursarySystem.Controllers
{
    // Only logged-in applicants can access their profiles.
    [Authorize(Roles = "Applicant")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Available academic year options.
        private readonly string[] _yearOfStudyOptions =
        {
            "1st Year",
            "2nd Year",
            "3rd Year",
            "4th Year",
            "5th Year",
            "Other"
        };

        public ProfileController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profile/Index
        // Displays the personal and academic information of the applicant.
        public async Task<IActionResult> Index()
        {
            // Get the ID of the currently logged-in user.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Find the applicant linked to this account.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Transfer database information into the ViewModel.
            var viewModel = new ApplicantProfileViewModel
            {
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                IDNumber = applicant.IDNumber,
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber,
                Address = applicant.Address,

                // Academic information.
                InstitutionName = applicant.InstitutionName,
                CourseName = applicant.CourseName,
                YearOfStudy = applicant.YearOfStudy,
               
            };

            // Supply the academic year options to the dropdown.
            ViewBag.YearOfStudyOptions = _yearOfStudyOptions;

            return View(viewModel);
        }

        // POST: Profile/Index
        // Saves changes made to the applicant's personal and academic details.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ApplicantProfileViewModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Find the applicant's existing profile.
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (applicant == null)
            {
                return NotFound("Applicant profile could not be found.");
            }

            // Check whether another applicant already uses this ID number.
            bool idAlreadyExists = await _context.Applicants
                .AnyAsync(a =>
                    a.IDNumber == model.IDNumber &&
                    a.ApplicantId != applicant.ApplicantId);

            if (idAlreadyExists)
            {
                ModelState.AddModelError(
                    nameof(model.IDNumber),
                    "This ID number is already registered to another applicant.");
            }

            // Check whether another account already uses this email address.
            var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingEmailUser != null && existingEmailUser.Id != userId)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "This email address is already registered to another account.");
            }

            // Ensure the selected academic year is one of the available options.
            if (!string.IsNullOrWhiteSpace(model.YearOfStudy) &&
                !_yearOfStudyOptions.Contains(model.YearOfStudy))
            {
                ModelState.AddModelError(
                    nameof(model.YearOfStudy),
                    "Please select a valid year of study.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.YearOfStudyOptions = _yearOfStudyOptions;
                return View(model);
            }

            // Find the Identity account connected to this applicant.
            var identityUser = await _userManager.FindByIdAsync(userId);

            if (identityUser == null)
            {
                return NotFound("Login account could not be found.");
            }

            // Start a database transaction so the profile and login
            // account changes can be saved together.
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Update the login account details.
                identityUser.Email = model.Email;
                identityUser.UserName = model.Email;
                identityUser.NormalizedEmail =
                    _userManager.NormalizeEmail(model.Email);
                identityUser.NormalizedUserName =
                    _userManager.NormalizeName(model.Email);
                identityUser.PhoneNumber = model.PhoneNumber;

                var identityResult =
                    await _userManager.UpdateAsync(identityUser);

                // Stop if Identity reports a problem.
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    ViewBag.YearOfStudyOptions = _yearOfStudyOptions;
                    return View(model);
                }

                // Update personal information.
                applicant.FirstName = model.FirstName;
                applicant.LastName = model.LastName;
                applicant.IDNumber = model.IDNumber;
                applicant.Email = model.Email;
                applicant.PhoneNumber = model.PhoneNumber;
                applicant.Address = model.Address;

                // Update academic information.
                applicant.InstitutionName = model.InstitutionName;
                applicant.CourseName = model.CourseName;
                applicant.YearOfStudy = model.YearOfStudy;
              

                await _context.SaveChangesAsync();

                // Confirm both sets of changes.
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Your profile has been updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Undo database changes if something goes wrong.
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while updating your profile. Please try again.");

                ViewBag.YearOfStudyOptions = _yearOfStudyOptions;
                return View(model);
            }
        }
    }
}