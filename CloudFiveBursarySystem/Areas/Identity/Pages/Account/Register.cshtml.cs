#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Areas.Identity.Pages.Account
{
    // Handles the registration of new CLOUD 5IVE applicants.
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Receives the services required to create accounts and save applicant profiles.
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _roleManager = roleManager;
        }

        // Holds the information entered on the registration form.
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Defines the information an applicant must provide when registering.
        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(20)]
            [Display(Name = "ID Number")]
            public string IDNumber { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [StringLength(250)]
            [Display(Name = "Residential Address")]
            public string Address { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 8,
                ErrorMessage = "The password must be at least 8 characters long.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password",
                ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        // Displays the registration page.
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // Processes the registration form when the applicant submits it.
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check whether the ID number has already been registered.
            bool idAlreadyExists = await _context.Applicants
                .AnyAsync(a => a.IDNumber == Input.IDNumber);

            if (idAlreadyExists)
            {
                ModelState.AddModelError(
                    "Input.IDNumber",
                    "This ID number is already registered.");

                return Page();
            }

            // Create the user's Identity login account.
            var user = new IdentityUser();

            await _userStore.SetUserNameAsync(
                user, Input.Email, CancellationToken.None);

            await _emailStore.SetEmailAsync(
                user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                try
                {
                    // Make sure the Applicant role exists.
                    if (!await _roleManager.RoleExistsAsync("Applicant"))
                    {
                        var roleResult = await _roleManager.CreateAsync(
                            new IdentityRole("Applicant"));

                        if (!roleResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                "The Applicant role could not be created.");
                        }
                    }

                    // Assign the new account to the Applicant role.
                    var roleAssignment = await _userManager.AddToRoleAsync(
                        user, "Applicant");

                    if (!roleAssignment.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "The Applicant role could not be assigned.");
                    }

                    // Create an applicant profile linked to the Identity account.
                    var applicant = new Applicant
                    {
                        UserId = user.Id,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        IDNumber = Input.IDNumber,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        Address = Input.Address
                    };

                    // Save the applicant profile in SQL Server.
                    _context.Applicants.Add(applicant);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "A new applicant account was successfully registered.");

                    // Sign the newly registered applicant into the system.
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect the applicant to their personal dashboard.
                    return RedirectToAction("Index", "ApplicantDashboard");
                }
                catch (Exception ex)
                {
                    // Remove the login account if creating the applicant profile fails.
                    await _userManager.DeleteAsync(user);

                    _logger.LogError(
                        ex, "Applicant registration failed while saving the profile.");

                    ModelState.AddModelError(
                        string.Empty,
                        "Registration could not be completed. Please try again.");

                    return Page();
                }
            }

            // Display any Identity registration errors on the form.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        // Returns a new Identity user account.
        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Unable to create an instance of '{nameof(IdentityUser)}'.");
            }
        }

        // Obtains the email store used by ASP.NET Core Identity.
        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "The application requires an Identity user store that supports email.");
            }

            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}