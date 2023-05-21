using IPFees.Evaluator;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using IPFees.Core.Repository;
using IPFees.Core.Model;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<ModuleCategoryInfo> ModuleCategories { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IJurisdictionRepository jurisdictionRepository;

        public IndexModel(IKeyValueRepository keyvalueRepository, IJurisdictionRepository jurisdictionRepository, IModuleRepository moduleRepository)
        {
            this.keyvalueRepository = keyvalueRepository;
            this.jurisdictionRepository = jurisdictionRepository;
            this.moduleRepository = moduleRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the distinct list of categories
            ModuleCategories = (await moduleRepository.GetModules())
                .DistinctBy(d => d.Category)
                .Select(s => new ModuleCategoryInfo(s.Category, keyvalueRepository.GetKeyAsync(s.Category).Result))
                .OrderBy(o => o.Weight);
            return Page();
        }
    }

    public record ModuleViewModel(ModuleInfo Mod, int Count);
}
