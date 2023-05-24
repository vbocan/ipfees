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
            // Retrieve the list of attorney fee levels
            var afs = from s in Enum.GetValues(typeof(JurisdictionAttorneyFeeLevel)).Cast<JurisdictionAttorneyFeeLevel>()
                     let af = settingsRepository.GetAttorneyFeeAsync(s).Result
                     select new AttorneyFeeInfo(s, af.Amount, af.Currency ?? string.Empty);
            AttorneyFees = afs.ToList();
            return Page();
        }
    }
}
