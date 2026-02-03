namespace NSE.WebApp.MVC.ViewModel;

public class OrderItemViewModel
{
    public Guid OrderId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
}