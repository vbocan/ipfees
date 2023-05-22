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
        [BindProperty] public IEnumerable<ModuleCategoryInfo> ModuleGroups { get; set; }
        [BindProperty] public IEnumerable<AttorneyFeeInfo> AttorneyFees { get; set; }

        private readonly ISettingsRepository settingsRepository;
        private readonly IModuleRepository moduleRepository;

        public IndexModel(ISettingsRepository settingsRepository, IModuleRepository moduleRepository)
        {
            this.settingsRepository = settingsRepository;
            this.moduleRepository = moduleRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the distinct list of module groups
            var mca = from s in (await moduleRepository.GetModules()).DistinctBy(d => d.GroupName)
                     let cd = settingsRepository.GetModuleGroupAsync(s.GroupName).Result
                     orderby cd.Item1 ascending
                     select new ModuleCategoryInfo(s.GroupName, cd.Item1 ?? string.Empty, cd.Item2);
            ModuleGroups = mca.ToList();

            // Retrieve the list of attorney fee levels
            var afs = from s in Enum.GetValues(typeof(JurisdictionAttorneyFeeLevel)).Cast<JurisdictionAttorneyFeeLevel>()
                     let af = settingsRepository.GetAttorneyFeeAsync(s).Result
                     select new AttorneyFeeInfo(s, af.Item1, af.Item2 ?? string.Empty);
            AttorneyFees = afs.ToList();
            return Page();
        }
    }
}
