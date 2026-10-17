using System.Globalization;

namespace RoboForge.Frontend.Components.Shared;

/// <summary>Turns the API's enum names and amounts into what a Polish customer reads on screen.</summary>
public static class PolishDisplayNames
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    public static string Category(string category) => category switch
    {
        "Housing" => "Obudowa",
        "Drive" => "Napęd",
        "Electronics" => "Elektronika",
        "Sensor" => "Czujnik",
        _ => category,
    };

    public static string Stage(string stage) => stage switch
    {
        "Printing" => "Druk 3D",
        "Assembly" => "Montaż",
        "QaTests" => "Testy jakości",
        "Packing" => "Pakowanie",
        "Shipped" => "Wysyłka",
        _ => stage,
    };

    public static string OrderStatus(string status) => status switch
    {
        "Placed" => "Złożone",
        "Confirmed" => "Potwierdzone",
        "InProduction" => "W produkcji",
        "Shipped" => "Wysłane",
        "Cancelled" => "Anulowane",
        _ => status,
    };

    public static string JobStatus(string status) => status switch
    {
        "InProgress" => "W toku",
        "OnHold" => "Wstrzymane",
        "Shipped" => "Wysłane",
        _ => status,
    };

    public static string Price(decimal amount) => amount.ToString("N2", PolishCulture) + " zł";

    public static string LocalTime(DateTime utc) => utc.ToLocalTime().ToString("dd.MM.yyyy HH:mm", PolishCulture);
}
