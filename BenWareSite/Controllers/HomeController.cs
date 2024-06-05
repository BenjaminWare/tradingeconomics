using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using BenWareSite.Models;

namespace BenWareSite.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static string _clientKey = "guest:guest"; // TODO read from env

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;

    }


    [Route("")]
    public IActionResult Index()
    {
        HomeViewModel model = new HomeViewModel()
        {
            count = 0
        };

        return View(model);
    }
    
    [HttpGet("getdata/{country?}/{indicator?}")]
    public async Task<JsonResult> GetData(string? country, string? indicator)
    {
        if (country != null && indicator != null) {
            return Json(await GetForecastsByCountryIndicator(country,indicator));
        }
        else {
            return Json("Invalid Params: This query needs 'getdata/{country}/{indicator}'");
        }
    }

    /// <summary>
    /// Get forecasts for a single country and indicator
    /// </summary>
    /// <param name="country">List of countries</param>
    /// <param name="indicator">List of indicators</param>

    /// <returns>A task that will be resolved in a string with the request result</returns>
    public async static Task<string> GetForecastsByCountryIndicator(string country, string indicator)
    {

        return await HttpRequester($"/forecast/country/{country}/indicator/{indicator}");
    }

     /// <summary>
    /// Method to make HTTP calls to TradingEconomics API
    /// </summary>
    /// <param name="url">The URL to fetch</param>
    /// <param name="baseURL">The base path, the default is 'https://api.tradingeconomics.com/'</param>
    /// <returns>A task tha will be resolved in a string with the content of the response</returns>
    public async static Task<string> HttpRequester(string url, string baseURL = "https://api.tradingeconomics.com/")
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();

                //ADD Acept Header to tell the server what data type you want
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                //ADD Authorization
                AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Client", _clientKey);
                client.DefaultRequestHeaders.Authorization = auth;

                HttpResponseMessage response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception e)
        {
            return $"Error at HttpRequester with msg: {e}";
        }
    }
    

}
  