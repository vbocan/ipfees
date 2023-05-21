using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IPFees.Core.Repository;
using IPFees.Core.Model;
using IPFees.Core.Enum;
using System.Xml.Linq;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<ModuleCategoryInfo> ModuleCategories { get; set; }
        [BindProperty] public IEnumerable<AttorneyFeeInfo> AttorneyFees { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;
        private readonly IModuleRepository moduleRepository;        

        public IndexModel(IKeyValueRepository keyvalueRepository, IModuleRepository moduleRepository)
        {
            this.keyvalueRepository = keyvalueRepository;            
            this.moduleRepository = moduleRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the distinct list of categories
            ModuleCategories = from s in (await moduleRepository.GetModules()).DistinctBy(d => d.Category)
                               let cd = keyvalueRepository.GetCategoryAsync(s.Category).Result
                               orderby cd.Item1 ascending
                               select new ModuleCategoryInfo(s.Name, cd.Item1, cd.Item2);
                
            // Retrieve the list of attorney fee levels
            AttorneyFees = from s in Enum.GetValues(typeof(JurisdictionAttorneyFeeLevel)).Cast<JurisdictionAttorneyFeeLevel>()
                           let af = keyvalueRepository.GetAttorneyFeeAsync(s).Result
                           select new AttorneyFeeInfo(s, af.Item1, af.Item2);
            return Page();
        }
    }

    public record ModuleViewModel(ModuleInfo Mod, int Count);
}
