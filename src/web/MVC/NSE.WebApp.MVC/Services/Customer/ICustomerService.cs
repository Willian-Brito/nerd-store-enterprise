using NSE.Core.Communication;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Services.Customer;

public interface ICustomerService
{
    Task<AddressViewModel> GetAddress();
    Task<ResponseResult> AddAddress(AddressViewModel address);
}