using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Account.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor, ILogger<LogoutModel> logger)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostAsync()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
