namespace NSE.WebApp.MVC.ViewModel;
public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public decimal Price { get; set; }
    public DateTime DateAdded { get; set; }
    public string Image { get; set; }
    public int Stock { get; set; }
}