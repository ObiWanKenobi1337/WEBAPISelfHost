using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using MoreLinq;

namespace SelfHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public List<WeatherForecast> ListOfWeatherForecasts = new List<WeatherForecast>();
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            var rng = new Random();
            foreach (var item in Enumerable.Range(1, 5))
            {
                ListOfWeatherForecasts.Add(new WeatherForecast{Date = DateTime.Now.Subtract(TimeSpan.FromDays(item)), TemperatureC = rng.Next(-20, 55), Summary = Summaries[rng.Next(Summaries.Length)]});
            }
            //Enumerable.Range(1, 5).ForEach(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //});
        }

        //[HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        public IEnumerable<WeatherForecast> GetByDate(DateTime dateFrom)
        {
            return ListOfWeatherForecasts.Where(item => item.Date >= dateFrom);
        }
    }
}
