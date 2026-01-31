using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Extensions.ViewComponent.Paging;

public class PagingViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    public IViewComponentResult Invoke(IPagedList pagingModel)
    {
        return View(pagingModel);
    }
}