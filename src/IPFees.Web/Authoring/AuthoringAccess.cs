using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IPFees.Web.Authoring
{
    /// <summary>
    /// Who may change what an instance serves. Calculating is open to anyone; storing a fee
    /// definition, a module, a jurisdiction or a service fee requires signing in with the
    /// authoring password set in configuration (<c>Authoring:Password</c>, or the environment
    /// variable <c>Authoring__Password</c>). An instance without one is read-only.
    /// </summary>
    public class AuthoringSettings
    {
        public const string SectionName = "Authoring";

        public string? Password { get; set; }

        public bool Enabled => !string.IsNullOrEmpty(Password);

        /// <summary>Compare in constant time, so the response time says nothing about the password.</summary>
        public bool Accepts(string? candidate)
        {
            if (!Enabled || candidate is null) return false;
            var expected = SHA256.HashData(Encoding.UTF8.GetBytes(Password!));
            var given = SHA256.HashData(Encoding.UTF8.GetBytes(candidate));
            return CryptographicOperations.FixedTimeEquals(expected, given);
        }
    }

    /// <summary>
    /// Refuses an anonymous request that would change anything in the authoring areas. The pages
    /// stay readable, so the IPFLang source behind a calculation can be shown to anyone, but no
    /// form in them is accepted without signing in.
    /// </summary>
    public class AuthoringPostFilter : IAsyncPageFilter
    {
        public static readonly IReadOnlySet<string> Areas =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Fee", "Module", "Jurisdiction", "Settings" };

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var http = context.HttpContext;
            var area = context.RouteData.Values["area"] as string;
            var changes = !HttpMethods.IsGet(http.Request.Method) && !HttpMethods.IsHead(http.Request.Method);

            if (changes && area is not null && Areas.Contains(area) && http.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new ChallengeResult();
                return;
            }

            await next();
        }
    }
}
