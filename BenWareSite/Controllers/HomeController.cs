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
    private Random _rand;
    private static string _clientKey = "c29be7da50584af:ez7pa1a5sjgcpz4"; // TODO read from env

    private string[] _countries = {"sweden","mexico","thailand","new zealand"};
    private string[] _indicators = {"gdp","population","wages"};

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _rand = new Random();

    }


    [Route("")]
    public IActionResult Index()
    {
        // On page refresh make sure the order that the data comes down in is a new random order
        _rand = new Random();
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
            return Json(await GetHistoricalIndicatorsByCountries([country],[indicator]));
        }
        else {
            country = _countries[_rand.Next(0,_countries.Length)];
            indicator = _indicators[_rand.Next(0,_indicators.Length)];
            Console.WriteLine(country + "," + indicator);
            return Json(await GetHistoricalIndicatorsByCountries([country],[indicator]));
        }
    }

        /// <summary>
        /// Get historical indicators given countries, indicators, start and end date
        /// </summary>
        /// <param name="countries">List of countries</param>
        /// <param name="indicators">List of indicators</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>A task that will be resolved in a string with the request result</returns>
        public async static Task<string> GetHistoricalIndicatorsByCountries(string[] countries, string[] indicators)
        {
            return await HttpRequester($"/historical/country/{string.Join(",", countries)}/indicator/{string.Join(",", indicators)}");
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
  