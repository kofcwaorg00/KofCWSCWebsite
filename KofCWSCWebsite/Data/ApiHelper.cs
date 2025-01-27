using System.Text.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using jdk.nashorn.@internal.parser;
using KofCWSCWebsite.Models;
using System.Net.Http.Headers;
using Serilog;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using KofCWSCWebsite.Services;
using System.Diagnostics;

namespace KofCWSCWebsite.Data
{

    
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly DataSetService _dataSetService;

        public ApiHelper( DataSetService dataSetService, string? token = null)
        {


            // Add default headers if needed
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            _dataSetService = dataSetService;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_dataSetService.GetAPIBaseAddress())
            };

        }

        // Generic GET method
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            ////////if (endpoint.Contains("IsKofCMember/2944485"))
            ////////{
            ////////    Debugger.Break();
            ////////}

            var response = await _httpClient.GetAsync(endpoint);
            //*****************************************************************************
            // 1/25/2025 Tim Philomeno
            // I added this to prevent the exception beign thrown when the api returns
            // nothing.  Specifically for IsKofCMember during the import of delegates.
            // I realize that this may cause other issues so testing will be done.
            if (response.ReasonPhrase == "Not Found") { return default(T); }
            //______________________________________________________________________________
            if (!response.IsSuccessStatusCode)
            {
                Log.Error($"Thrown from inside apiHelper because response.IsSuccessStatusCode is {response.IsSuccessStatusCode.ToString()} for endpoint {endpoint} and baseaddress {_httpClient.BaseAddress.ToString()} ");
                throw new HttpRequestException($"GET request failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}+{endpoint}");
            }
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex,"GET"+endpoint));
                return default;
            }
            
        }

        // Generic POST method
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST request failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}+{endpoint}");
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) { return default; };
            try
            {
                return JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex,"POST"+endpoint));
                return default;
            }
            
        }

        // Generic PUT method
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"PUT request failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) { return default; };
            try
            {
                return JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex,"PUT"+endpoint));
                return default;

            }

        }

        // Generic DELETE method
        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"DELETE request failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }
        }
    }


}