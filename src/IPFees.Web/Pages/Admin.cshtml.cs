using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{    
    public class AdminModel : PageModel
    {
        public int JurisdictionCount { get; set; }
        public int FeeCount { get; set; }
        public int ModuleCount { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IFeeRepository feeRepository;        
        private readonly ILogger<IndexModel> _logger;

        public AdminModel(IJurisdictionRepository jurisdictionRepository, IModuleRepository moduleRepository, IFeeRepository feeRepository, ILogger<IndexModel> logger)
        {            
            this.jurisdictionRepository = jurisdictionRepository;
            this.moduleRepository = moduleRepository;
            this.feeRepository = feeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            JurisdictionCount = (await jurisdictionRepository.GetJurisdictions()).Count();
            FeeCount = (await feeRepository.GetFees()).Count();
            ModuleCount = (await moduleRepository.GetModules()).Count();            
            return Page();
        }
    }
}