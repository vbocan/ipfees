using IPFees.Evaluator;
using IPFees.Web.Data;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Pages.Module
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<ModuleInfo> Modules { get; set; }
        private readonly IModuleRepository moduleRepository;

        public IndexModel(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;            
        }
        public void OnGet()
        {
            Modules = moduleRepository.GetModules();
        }
    }
}
