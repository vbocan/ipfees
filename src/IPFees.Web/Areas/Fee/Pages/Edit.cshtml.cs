using IPFees.Core;
using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Fee.Pages
{
    public class EditModel : PageModel
    {
        [BindProperty] public string Category { get; set; }
        [BindProperty] public string AttorneyFeeLevel { get; set; }
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> CategoryItems { get; set; }
        public IEnumerable<SelectListItem> AttorneyFeeLevelItems { get; set; }

        private readonly IFeeRepository feeRepository;
        private readonly IModuleRepository moduleRepository;

        public EditModel(IFeeRepository feeRepository, IModuleRepository moduleRepository)
        {
            this.feeRepository = feeRepository;
            this.moduleRepository = moduleRepository;
            CategoryItems = Enum.GetValues<FeeCategory>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
            AttorneyFeeLevelItems = Enum.GetValues<AttorneyFeeLevel>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            // Retrieve the fee by name
            var jur = await feeRepository.GetFeeById(Id);
            Name = jur.Name;
            Description = jur.Description;
            SourceCode = jur.SourceCode;
            Category = jur.Category.ToString();
            AttorneyFeeLevel = jur.AttorneyFeeLevel.ToString();
            // Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, jur.ReferencedModules.Contains(s.Id))).ToList();
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
            var parsedAttorneyFeeLevel = (AttorneyFeeLevel)Enum.Parse(typeof(AttorneyFeeLevel), AttorneyFeeLevel);
            var res6 = await feeRepository.SetAttorneyFeeLevelAsync(Id, parsedAttorneyFeeLevel);
            if (!res6.Success)
            {
                ErrorMessages.Add($"Error setting fee level: {res6.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
