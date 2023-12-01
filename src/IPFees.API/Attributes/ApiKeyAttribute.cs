using IPFees.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.API.Attributes
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
