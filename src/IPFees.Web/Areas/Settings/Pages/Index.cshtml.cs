using IPFees.Core.Enum;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{    
    public class IndexModel : PageModel
    {        
        [BindProperty] public IEnumerable<ServiceFeeInfo> ServiceFees { get; set; }

        private readonly ISettingsRepository settingsRepository;
        private readonly IModuleRepository moduleRepository;

        public IndexModel(ISettingsRepository settingsRepository, IModuleRepository moduleRepository)
        {
            this.settingsRepository = settingsRepository;
            this.moduleRepository = moduleRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the list of service fee levels
            var afs = from s in Enum.GetValues(typeof(ServiceFeeLevel)).Cast<ServiceFeeLevel>()
                     let af = settingsRepository.GetServiceFeeAsync(s).Result
                     select new ServiceFeeInfo(s, af.Amount, af.Currency ?? string.Empty);
            ServiceFees = afs.ToList();
            return Page();
        }
    }
}
