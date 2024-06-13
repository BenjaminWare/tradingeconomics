namespace BenWareSite.Models;
using Microsoft.AspNetCore.Mvc;
public class CountryViewModel
{
    public string? CountryColor {get;set;}
    public List<CountryDatapoint>? Data {get;set;}

}
public struct CountryDatapoint
{
    public string Category;
    public string Country;
    public string DateTime;
    public string Frequency;
    public string HistoricalDataSymbol;
    public string LastUpdate;
    public float Value;
}