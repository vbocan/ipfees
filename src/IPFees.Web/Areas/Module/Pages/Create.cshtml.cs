using IPFees.Core.FeeCalculation;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{    
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public bool AutoRun { get; set; }
        [BindProperty] public string SourceCode { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        /// <summary>What IPFLang made of the module on the last save attempt.</summary>
        public ModuleValidationReport? Validation { get; private set; }

        private readonly IModuleRepository moduleRepository;
        private readonly IFeeDefinitionValidator validator;

        public CreateModel(IModuleRepository moduleRepository, IFeeDefinitionValidator validator)
        {
            this.moduleRepository = moduleRepository;
            this.validator = validator;
            ErrorMessages = new List<string>();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // A new module reaches no definition yet unless it runs before every calculation, in
            // which case it is checked against all of them.
            Validation = await validator.ValidateModuleAsync(null, SourceCode, AutoRun);
            if (!Validation.MayBeSaved) return Page();

            var res1 = await moduleRepository.AddModuleAsync(Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error creating module: {res1.Reason}");
            }
            var res2 = await moduleRepository.SetModuleDescriptionAsync(res1.Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var res3 = await moduleRepository.SetModuleSourceCodeAsync(res1.Id, SourceCode);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res3.Reason}");
            }
            var res4 = await moduleRepository.SetModuleAutoRunStatusAsync(res1.Id, AutoRun);
            if (!res4.Success)
            {
                ErrorMessages.Add($"Error setting autorun status: {res4.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
