using ChristmasCalendar.Domain;
using ChristmasCalendar.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ChristmasCalendar.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel? Input { get; set; }

        public string? LoginProvider { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required] public bool WantsDailyNotification { get; set; } = false;
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("/");
        }

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("/");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
                return RedirectToPage("/");

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity!.Name, info.LoginProvider);

                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }

            if (result.IsLockedOut)
                return RedirectToPage("/");

            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                    throw new ApplicationException("Error loading external login information during confirmation.");

                var user = new ApplicationUser
                {
                    UserName = Input!.Email,
                    Email = Input.Email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                    DateOfBirth = info.Principal.FindFirstValue(ClaimTypes.DateOfBirth),
                    EmailAddressFromAuthProvider = info.Principal.FindFirstValue(ClaimTypes.Email),
                    WantsDailyNotification = Input.WantsDailyNotification,
                    EmailConfirmed = true // Note 2021-11-28: Social identity provider requires that email is confirmed from user. Ref: https://stackoverflow.com/questions/41598437/signinmanagerexternalloginsigninasync-returns-isnotallowed-for-social-login
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        return LocalRedirect(Url.GetLocalUrl(returnUrl));
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            ReturnUrl = returnUrl;

            return Page();
        }
    }
}
