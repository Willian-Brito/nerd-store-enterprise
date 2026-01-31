using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Services.Catalog;

public interface ICatalogService
{
    Task<PagedViewModel<ProductViewModel>> GetAll(int pageSize, int pageIndex, string query = null);
    Task<ProductViewModel> GetById(Guid id);
}