using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;

namespace NSE.WebApp.MVC.Extensions.Helpers;

public static class RazorHelpers
{
    public static string HashEmailForGravatar(this RazorPage page, string email)
    {
        var md5Hasher = MD5.Create();
        var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
        var sBuilder = new StringBuilder();
        foreach (var t in data) sBuilder.Append(t.ToString("x2"));
        return sBuilder.ToString();
    }

    public static string FormatMoney(this RazorPage page, decimal valor)
    {
        return FormatMoney(valor);
    }

    private static string FormatMoney(decimal valor)
    {
        return string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:C}", valor);
    }

    public static string StockMessage(this RazorPage page, int quantity)
    {
        return quantity > 0 ? $"Only {quantity} at stock!" : "No stock!";
    }

    public static string UnityByProduct(this RazorPage page, int units)
    {
        return units > 1 ? $"{units} units" : $"{units} unit";
    }

    public static string SelectOptionsByQuantity(this RazorPage page, int quantity, int selectedValue = 0)
    {
        var sb = new StringBuilder();
        for (var i = 1; i <= quantity; i++)
        {
            var selected = "";
            if (i == selectedValue) selected = "selected";
            sb.Append($"<option {selected} value='{i}'>{i}</option>");
        }

        return sb.ToString();
    }

    public static string UnitByProductAmount(this RazorPage page, int units, decimal value)
    {
        return $"{units}x {FormatMoney(value)} = Total: {FormatMoney(value * units)}";
    }

    public static string ShowStatus(this RazorPage page, int status)
    {
        var statusMessage = "";
        var statusClass = "";

        switch (status)
        {
            case 1:
                statusClass = "info";
                statusMessage = "Processing";
                break;
            case 2:
                statusClass = "primary";
                statusMessage = "Approved";
                break;
            case 3:
                statusClass = "danger";
                statusMessage = "Refused";
                break;
            case 4:
                statusClass = "success";
                statusMessage = "Delivered";
                break;
            case 5:
                statusClass = "warning";
                statusMessage = "Canceled";
                break;
        }

        return $"<span class='badge bg-{statusClass}'>{statusMessage}</span>";
    }
}