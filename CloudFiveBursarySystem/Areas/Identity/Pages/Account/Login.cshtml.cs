#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CloudFiveBursarySystem.Areas.Identity.Pages.Account
{
    // Handles user login and redirects users according to their roles.
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // Receives the services needed to authenticate users and check their roles.
        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Stores the information entered on the login form.
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        // Temporarily stores login error messages.
        [TempData]
        public string ErrorMessage { get; set; }

        // Defines the fields required for login.
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // Displays the login page.
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clears any previous external authentication session.
            await HttpContext.SignOutAsync(
                IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // Processes the login form.
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Checks whether the submitted information is valid.
            if (!ModelState.IsValid)
            {
                ExternalLogins = (await _signInManager
                    .GetExternalAuthenticationSchemesAsync()).ToList();

                return Page();
            }

            // Attempts to authenticate the user using ASP.NET Core Identity.
            // Failed attempts count towards account lockout protection.
            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in successfully.");

                // Retrieves the account belonging to the supplied email address.
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    await _signInManager.SignOutAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        "The user account could not be found.");

                    return Page();
                }

                // Sends applicants to their applicant dashboard.
                if (await _userManager.IsInRoleAsync(user, "Applicant"))
                {
                    // Preserves a valid local return destination when one was requested.
                    if (!string.IsNullOrEmpty(returnUrl)
                        && returnUrl != "/"
                        && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction(
                        "Index",
                        "ApplicantDashboard");
                }

                // Sends administrators to the administrator dashboard.
                if (await _userManager.IsInRoleAsync(user, "Administrator"))
                {
                    return RedirectToAction(
                        "Index",
                        "AdminDashboard");
                }

                // Sends accounts without an assigned role to the home page.
                return RedirectToAction("Index", "Home");
            }

            // Handles accounts requiring two-factor authentication.
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    "./LoginWith2fa",
                    new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = Input.RememberMe
                    });
            }

            // Handles accounts temporarily locked after failed login attempts.
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");

                return RedirectToPage("./Lockout");
            }

            // Displays a general error without revealing whether the email exists.
            ModelState.AddModelError(
                string.Empty,
                "Invalid login attempt.");

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync()).ToList();

            return Page();
        }
    }
}