using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace TechMoveCRM.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public CurrencyService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["CurrencyApi:BaseUrl"];
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Async/Await — never blocking with .Result
                var response = await client.GetStringAsync(_apiUrl);

                // Parse the JSON response
                var json = JObject.Parse(response);
                var rate = json["rates"]["ZAR"].Value<decimal>();
                return rate;
            }
            catch (Exception)
            {
                // If API is down, fall back to a sensible default rate
                // In production you'd log this and alert someone
                return 18.50m; // Fallback rate
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            if (rate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.", nameof(rate));
            if (usdAmount < 0) throw new ArgumentException("USD amount cannot be negative.", nameof(usdAmount));

            return Math.Round(usdAmount * rate, 2);
        }
    }
}