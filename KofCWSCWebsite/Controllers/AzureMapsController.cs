using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KofCWSCWebsite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureMapsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AzureMapsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet("DriveDistance")]
        public async Task<IActionResult> CalculateDrivingDistance(string address1, string address2)
        {
            try
            {
                //@address1 = "6028 Nathan Way SE,Auburn, WA 98092"
                //@address2 = "5000 Abbey Way SE,Lacey, WA 98503"
                var azureMapsKey = _configuration["AzureMaps:ApiKey"];
                if (string.IsNullOrEmpty(azureMapsKey))
                {
                    return BadRequest("Azure Maps API key is not configured.");
                }

                // Step 1: Geocode the first address
                var geocodeUrl1 = $"https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key={azureMapsKey}&query={Uri.EscapeDataString(address1)}";
                var response1 = await _httpClient.GetAsync(geocodeUrl1);
                response1.EnsureSuccessStatusCode();
                var geocodeResult1 = JsonDocument.Parse(await response1.Content.ReadAsStringAsync());
                var coordinates1 = geocodeResult1.RootElement
                    .GetProperty("results")[0]
                    .GetProperty("position");

                var lat1 = coordinates1.GetProperty("lat").GetDouble();
                var lon1 = coordinates1.GetProperty("lon").GetDouble();

                // Step 2: Geocode the second address
                var geocodeUrl2 = $"https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key={azureMapsKey}&query={Uri.EscapeDataString(address2)}";
                var response2 = await _httpClient.GetAsync(geocodeUrl2);
                response2.EnsureSuccessStatusCode();
                var geocodeResult2 = JsonDocument.Parse(await response2.Content.ReadAsStringAsync());
                var coordinates2 = geocodeResult2.RootElement
                    .GetProperty("results")[0]
                    .GetProperty("position");

                var lat2 = coordinates2.GetProperty("lat").GetDouble();
                var lon2 = coordinates2.GetProperty("lon").GetDouble();

                // Step 3: Call the Route API to calculate driving distance
                var routeUrl = $"https://atlas.microsoft.com/route/directions/json?api-version=1.0&subscription-key={azureMapsKey}&query={lat1},{lon1}:{lat2},{lon2}";
                var routeResponse = await _httpClient.GetAsync(routeUrl);
                routeResponse.EnsureSuccessStatusCode();
                var routeResult = JsonDocument.Parse(await routeResponse.Content.ReadAsStringAsync());
                var travelSummary = routeResult.RootElement
                    .GetProperty("routes")[0]
                    .GetProperty("summary");

                var travelDistance = travelSummary.GetProperty("lengthInMeters").GetDouble(); // Distance in meters
                var travelTime = travelSummary.GetProperty("travelTimeInSeconds").GetDouble(); // Time in seconds

                return Ok(new
                {
                    DistanceInKilometers = travelDistance / 1000,
                    DistanceInMiles = (travelDistance / 1000) * 0.621372,
                    TravelTimeInMinutes = travelTime / 60
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}


