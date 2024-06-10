using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Newtonsoft.Json;

using BenWareSite.Models;

namespace BenWareSite.Controllers;

[Route("")]
public class CountryController : Controller
{
    private readonly ILogger<CountryController> _logger;
    private Random _rand;
    private static string _clientKey = "c29be7da50584af:ez7pa1a5sjgcpz4";

    public CountryController(ILogger<CountryController> logger)
    {
        _logger = logger;
        _rand = new Random();
        // _clientKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["ClientKey"];

    }


    [Route("{country}")]
    public async Task<IActionResult> Index(string country)
    {
        // On page refresh make sure the order that the data comes down in is a new random order
        _rand = new Random();
        CountryViewModel model = new CountryViewModel(){
            // TODO multiple requests at once .all() from js
            Data = JsonConvert.DeserializeObject<List<CountryDatapoint>>((await (GetHistoricalIndicatorsByCountry(country)))),
          
        };
        return View(model);
    }
    


        /// <summary>
        /// Get historical indicators given countries, indicators, start and end date
        /// </summary>
        /// <param name="countries">List of countries</param>
        /// <param name="indicators">List of indicators</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>A task that will be resolved in a string with the request result</returns>
        [HttpGet("getdata/{country?}/{indicator?}")]
        public async static Task<string> GetHistoricalIndicatorsByCountry(string country)
        {
            var req =  await HttpRequester($"/historical/country/{country}/indicator/population,gdp");
            return req;
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
  