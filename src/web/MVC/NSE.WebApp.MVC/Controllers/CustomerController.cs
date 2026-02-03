using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services.Customer;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Controllers;

public class CustomerController : MainController
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> NewAddress(AddressViewModel address)
    {
        var response = await _customerService.AddAddress(address);

        if (ResponseHasErrors(response))
            TempData["Errors"] =
                ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

        return RedirectToAction("DeliveryAddress", "Order");
    }
}