# BenWareSite
This is a demo aspnet mvc application showing off some of TradingEconomics free functionality

## Running
This project can be run with "dotnet run" or "dotnet watch". It requires an environment variable TE_api_key, which is a working trading economics api key

## Features
This site uses standard server side rendering, most of the work is done in the CountryController.cs and the corresponding view Country/index.cshtml
It currently sends down 6 categories of historical country data: "gdp,population,wages,temperature,personal savings,military%20expenditure" but, this
can be changed by just adding categories to this string on line 75 in CountryController.cs, the graphs will automatically draw if data is available.


## Hosing
This demo is hosted on Azure and can be accessed at: https://benwarete.azurewebsites.net/