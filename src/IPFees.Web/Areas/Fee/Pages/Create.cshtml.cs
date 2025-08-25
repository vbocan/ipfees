using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Fee.Pages
{    
    public class CreateModel : PageModel
    {
        [BindProperty] public string Category { get; set; }        
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string JurisdictionName { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> CategoryItems { get; set; }        
        public IEnumerable<SelectListItem> JurisdictionNameItems { get; set; }
        
        private readonly IFeeRepository feeRepository;
        private readonly IModuleRepository moduleRepository;

        public CreateModel(IJurisdictionRepository jurisdictionRepository, IFeeRepository feeRepository, IModuleRepository moduleRepository)
        {            
            this.feeRepository = feeRepository;
            this.moduleRepository = moduleRepository;
            CategoryItems = Enum.GetValues<FeeCategory>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));            
            JurisdictionNameItems = jurisdictionRepository.GetJurisdictions().Result.Select(s => new SelectListItem($"{s.Name} - {s.Description}", s.Name));
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Where(w=>!w.AutoRun).Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, false)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res1 = await feeRepository.AddFeeAsync(Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error creating fee: {res1.Reason}");
            }
            var res2 = await feeRepository.SetFeeDescriptionAsync(res1.Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Id).ToList();
            var res3 = await feeRepository.SetReferencedModules(res1.Id, RefMod);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting referenced modules: {res3.Reason}");
            }
            var res4 = await feeRepository.SetFeeSourceCodeAsync(res1.Id, SourceCode);
            if (!res4.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res4.Reason}");
            }
            var parsedCategory = (FeeCategory)Enum.Parse(typeof(FeeCategory), Category);
            var res5 = await feeRepository.SetFeeCategoryAsync(res1.Id, parsedCategory);
            if (!res5.Success)
            {
                ErrorMessages.Add($"Error setting category: {res5.Reason}");
            }
            var res6 = await feeRepository.SetFeeJurisdictionNameAsync(res1.Id, JurisdictionName);
            if (!res6.Success)
            {
                ErrorMessages.Add($"Error setting jurisdiction name: {res6.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }

    public record ModuleViewModel(Guid Id, string Name, string Description, DateTime LastUpdatedOn, bool Checked);
}
