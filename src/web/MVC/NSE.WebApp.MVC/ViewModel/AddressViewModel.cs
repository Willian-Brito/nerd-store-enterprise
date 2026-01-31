using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NSE.WebApp.MVC.ViewModel;

public class AddressViewModel
{
    [Required]
    [DisplayName("Street Address")]
    public string StreetAddress { get; set; }

    [Required]
    [DisplayName("Building Number")]
    public string BuildingNumber { get; set; }

    [DisplayName("Secondary Address")] 
    public string SecondaryAddress { get; set; }

    [Required] 
    public string Neighborhood { get; set; }

    [Required] 
    [DisplayName("Zip Code")] 
    public string ZipCode { get; set; }

    [Required] 
    public string City { get; set; }

    [Required] 
    public string State { get; set; }

    public override string ToString()
    {
        return $"{StreetAddress}, {BuildingNumber} {SecondaryAddress} - {Neighborhood} - {City} - {State}";
    }
}