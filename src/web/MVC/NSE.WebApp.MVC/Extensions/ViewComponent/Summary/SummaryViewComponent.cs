using Microsoft.AspNetCore.Mvc;

namespace NSE.WebApp.MVC.Extensions.ViewComponent.Summary;

public class SummaryViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}