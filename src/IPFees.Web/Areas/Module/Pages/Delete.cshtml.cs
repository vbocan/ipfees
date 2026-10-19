using IPFees.Core.FeeCalculation;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{
    public class DeleteModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        /// <summary>
        /// What removing the module would do to the fee definitions that use it; set when the
        /// removal is refused.
        /// </summary>
        public ModuleValidationReport? Validation { get; private set; }

        private readonly IModuleRepository moduleRepository;
        private readonly IFeeRepository feeRepository;
        private readonly IFeeDefinitionValidator validator;

        public DeleteModel(IModuleRepository moduleRepository, IFeeRepository feeRepository, IFeeDefinitionValidator validator)
        {
            this.moduleRepository = moduleRepository;
            this.feeRepository = feeRepository;
            this.validator = validator;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await moduleRepository.GetModuleById(Id);
            Name = info.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            // A module is shared: removing a regional base removes the inputs its members compute
            // with, so the removal goes through the same gate as an edit.
            var report = await validator.ValidateModuleRemovalAsync(Id);
            if (!report.MayBeSaved)
            {
                Validation = report;
                return Page();
            }

            // The definitions that named the module still stand without it; drop the reference
            // so none of them is left pointing at a module that no longer exists.
            foreach (var fee in (await feeRepository.GetFees()).Where(f => f.ReferencedModules.Contains(Id)))
            {
                var res = await feeRepository.SetReferencedModules(fee.Id, fee.ReferencedModules.Where(m => m != Id).ToList());
                if (!res.Success) ErrorMessages.Add($"Error updating fee definition {fee.Name}: {res.Reason}");
            }
            if (ErrorMessages.Any()) return Page();

            var removed = await moduleRepository.RemoveModuleAsync(Id);
            if (!removed.Success)
            {
                ErrorMessages.Add($"Error deleting module: {removed.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
