using IPFees.Web.Areas.Run.Pages;
using Microsoft.AspNetCore.Mvc;

namespace IPFees.Web.ViewComponents
{
    /// <summary>
    /// Display data collection input elements
    /// </summary>
    public class DataCollectViewComponent : ViewComponent
    {
        public DataCollectViewComponent() { }
        /// <summary>
        /// Display input controls
        /// </summary>        
        /// <param name="Inputs">Input controls</param>
        /// <param name="HtmlClass">Class attributes that will be transferred over to the image</param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(IList<InputViewModel> Inputs, string HtmlClass)
        {
            var model = new DataCollectViewModel(Inputs, HtmlClass);
            return View(model);
        }
    }

    public record DataCollectViewModel(IList<InputViewModel> Inputs, string HtmlClass);
}
