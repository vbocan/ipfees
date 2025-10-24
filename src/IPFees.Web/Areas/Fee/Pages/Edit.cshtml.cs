using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Fee.Pages
{    
    public class EditModel : PageModel
    {
        [BindProperty] public string Category { get; set; } = null!;
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string JurisdictionName { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public string SourceCode { get; set; } = null!;
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> CategoryItems { get; set; }        
        public IEnumerable<SelectListItem> JurisdictionNameItems { get; set; }

        private readonly IFeeRepository feeRepository;
        private readonly IModuleRepository moduleRepository;

        public EditModel(IJurisdictionRepository jurisdictionRepository, IFeeRepository feeRepository, IModuleRepository moduleRepository)
        {
            this.feeRepository = feeRepository;
            this.moduleRepository = moduleRepository;
            CategoryItems = Enum.GetValues<FeeCategory>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
            JurisdictionNameItems = jurisdictionRepository.GetJurisdictions().Result.OrderBy(o=>o.Name).ThenBy(t=>t.Description).Select(s => new SelectListItem($"{s.Name} - {s.Description}", s.Name));
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            // Retrieve the fee by name
            var jur = await feeRepository.GetFeeById(Id);
            Name = jur.Name;
            JurisdictionName = jur.JurisdictionName;
            Description = jur.Description;
            SourceCode = jur.SourceCode;
            Category = jur.Category.ToString();            
            // Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Where(w => !w.AutoRun).Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, jur.ReferencedModules.Contains(s.Id))).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res1 = await feeRepository.SetFeeNameAsync(Id, Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error setting name: {res1.Reason}");
            }
            var res2 = await feeRepository.SetFeeDescriptionAsync(Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Id).ToList();
            var res3 = await feeRepository.SetReferencedModules(Id, RefMod);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting referenced modules: {res3.Reason}");
            }
            var res4 = await feeRepository.SetFeeSourceCodeAsync(Id, SourceCode);
            if (!res4.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res4.Reason}");
            }
            var parsedCategory = (FeeCategory)Enum.Parse(typeof(FeeCategory), Category);
            var res5 = await feeRepository.SetFeeCategoryAsync(Id, parsedCategory);
            if (!res5.Success)
            {
                ErrorMessages.Add($"Error setting category: {res5.Reason}");
            }
            var res6 = await feeRepository.SetFeeJurisdictionNameAsync(Id, JurisdictionName);
            if (!res6.Success)
            {
                ErrorMessages.Add($"Error setting jurisdiction name: {res6.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
