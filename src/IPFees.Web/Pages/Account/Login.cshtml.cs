using System.Security.Claims;
using IPFees.Web.Authoring;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthoringSettings settings;
        private readonly ILogger<LoginModel> logger;

        public LoginModel(IOptions<AuthoringSettings> settings, ILogger<LoginModel> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        [BindProperty] public string? Password { get; set; }
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

        public bool Enabled => settings.Enabled;
        public string? Error { get; private set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!settings.Accepts(Password))
            {
                // Slow down guessing; the check itself takes constant time.
                await Task.Delay(TimeSpan.FromSeconds(1));
                logger.LogWarning("Refused an authoring sign-in from {Address}", HttpContext.Connection.RemoteIpAddress);
                Error = settings.Enabled ? "That is not the authoring password." : "Authoring is disabled on this instance.";
                return Page();
            }

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "author") }, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/");
        }
    }
}
