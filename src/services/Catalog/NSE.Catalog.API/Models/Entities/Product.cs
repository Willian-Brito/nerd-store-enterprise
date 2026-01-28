using NSE.Core.DomainObjects;

namespace NSE.Catalog.API.Models.Entities;

public class Product : Entity, IAggregateRoot
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    public bool Active { get; set; }
    public decimal Price { get; set; }
    public DateTime DateAdded { get; set; }
    public string Image { get; set; }
    public int Stock { get; set; }

    public void TakeFromInventory(int quantity)
    {
        if (Stock >= quantity)
            Stock -= quantity;
    }

    public bool IsAvailable(int quantity)
    {
        return Active && Stock >= quantity;
    }
}