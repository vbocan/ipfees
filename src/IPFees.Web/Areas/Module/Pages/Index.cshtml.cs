using IPFees.Evaluator;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using IPFees.Core.Repository;
using IPFees.Core.Model;

namespace IPFees.Web.Areas.Module.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<ModuleViewModel> Modules { get; set; }
        private readonly IModuleRepository moduleRepository;
        private readonly IJurisdictionRepository jurisdictionRepository;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, IModuleRepository moduleRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.moduleRepository = moduleRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve modules from the database
            var DbMod = (await moduleRepository.GetModules()).OrderBy(o => o.Name).ThenBy(t=>t.LastUpdatedOn);
            // Compute the number of jurisdictions that reference each module
            var DbJur = await jurisdictionRepository.GetJurisdictions();
            Modules = from m in DbMod
                      select new ModuleViewModel
                      (m,
                          (from j in DbJur
                           where j.ReferencedModules.Contains(m.Id)
                           select j).Count()
                      );
            return Page();
        }
    }

    public record ModuleViewModel(ModuleInfo Mod, int Count);
}
