using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Services;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class JwtResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string status { get; set; }
    }
    public class AddressRequestsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        const string JWTbaseUrl = "https://api.usps.com/oauth2/v3/token";

       

        public AddressRequestsController(ApplicationDbContext context, IConfiguration configuration)
        {
            //_context = context;
            _configuration = configuration;

        }

       

        // GET: AddressRequests/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(USPSAddress addressRequest) // string street, string city, string state, string zipCode)
        {
            // USPS API endpoint
            var endpoint = "https://api.usps.com/addresses/v3/address";

            // Replace these with your actual Consumer Key and Secret
            var consumerKey = _configuration["USPS:ConsumerKey"];
            var consumerSecret = _configuration["USPS:ConsumerSecret"];

            var jwtToken = await GetJwtTokenAsync(JWTbaseUrl, consumerKey, consumerSecret);
            Log.Information(jwtToken);
            var httpClient = new HttpClient();

            var payload = new
            {
                client_id = consumerKey,
                client_secret = consumerSecret,
                grant_type = "client_credentials"
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Create query parameters (modify as per USPS API documentation)
            var queryParams = $"?streetAddress={Uri.EscapeDataString(addressRequest.Address.StreetAddress)}&city={Uri.EscapeDataString(addressRequest.Address.City)}&state={Uri.EscapeDataString(addressRequest.Address.State)}&ZIPCode={Uri.EscapeDataString(addressRequest.Address.ZIPCode)}";
            var requestUri = endpoint + queryParams;

            try
            {
                // Call USPS API
                var responseData = await httpClient.GetAsync(requestUri);

                if (responseData.IsSuccessStatusCode)
                {
                    var contentAddr = await responseData.Content.ReadAsStringAsync();
                    var desAddr = JsonSerializer.Deserialize<USPSAddress>(contentAddr, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    // Return API response (you can also deserialize to a model)
                    ViewBag.USPSAddress = "Found This Address";
                    return View(desAddr);
                }
                else
                {
                    var myError = await responseData.Content.ReadAsStringAsync();
                    ViewBag.USPSAddress = "Address Not Found.  Try Again";
                    var myRetVal = new USPSAddress();
                    return View(myRetVal);
                    //return StatusCode((int)responseData.StatusCode, await responseData.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        static async Task<string> GetJwtTokenAsync(string baseUrl, string clientId, string clientSecret)
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

            // Prepare the request payload
            var payload = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "client_credentials"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Send POST request to the authentication endpoint
            Log.Information(baseUrl + content.ToString());
            var response = await client.PostAsync(baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonSerializer.Deserialize<JwtResponse>(responseString);
                Log.Information(jsonResponse.access_token);
                return jsonResponse.access_token;
            }
            else
            {
                Log.Error($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
}
