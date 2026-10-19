using IPFees.Core.FeeCalculation;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{    
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public bool AutoRun { get; set; }
        [BindProperty] public string SourceCode { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        /// <summary>What IPFLang made of the module and its dependents on the last save attempt.</summary>
        public ModuleValidationReport? Validation { get; private set; }

        private readonly IModuleRepository moduleRepository;
        private readonly IFeeDefinitionValidator validator;

        public EditModel(IModuleRepository moduleRepository, IFeeDefinitionValidator validator)
        {
            this.moduleRepository = moduleRepository;
            this.validator = validator;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await moduleRepository.GetModuleById(Id);
            Name = info.Name;
            Description = info.Description;
            AutoRun = info.AutoRun;
            SourceCode = info.SourceCode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            // A module is shared by every fee definition that names it, and a regional base is a
            // module, so a change here is checked against all of them before it is stored.
            Validation = await validator.ValidateModuleAsync(Id, SourceCode, AutoRun);
            if (!Validation.MayBeSaved) return Page();

            var res = await moduleRepository.SetModuleNameAsync(Id, Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting name: {res.Reason}");
            }
            res = await moduleRepository.SetModuleDescriptionAsync(Id, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            res = await moduleRepository.SetModuleSourceCodeAsync(Id, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res.Reason}");
            }
            res = await moduleRepository.SetModuleAutoRunStatusAsync(Id, AutoRun);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting autorun status: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
