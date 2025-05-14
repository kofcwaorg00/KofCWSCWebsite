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
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

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
        public async Task<T?> GetAsync<T>(string endpoint,string callingprocess = "")
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
            // 2/12/2025 Tim Philomeno
            // trying to figure out why HOME is failing periodacally.  I think it is because
            // the API has to fire up from a cold start
            if (!response.IsSuccessStatusCode)
            {
                // 5/12/2025 Tim Philomeno
                // I came accross using Problem() to return structured error information from the API
                // This logic should allow the existing error processing to work and support this new
                // Problem() processing
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                if (problem == null)
                {
                    Exception myex = new Exception($"Thrown from inside apiHelper because response.IsSuccessStatusCode is {response.IsSuccessStatusCode.ToString()} for endpoint {endpoint} and baseaddress {_httpClient.BaseAddress.ToString()} - Reason: {"GetAsync - " + response.ReasonPhrase}+{endpoint} ");
                    Log.Error(Utils.FormatLogEntry(this, myex));
                    throw myex;
                }
                else
                {
                    Exception myex = new Exception($"Thrown from inside apiHelper,Message = {problem.Title}, {problem.Detail}");
                    Log.Error(Utils.FormatLogEntry(this, myex));
                    throw myex;
                }
                //throw new HttpRequestException($"GET request failed. Status Code: {response.StatusCode}, Reason: {"GetAsync " + response.ReasonPhrase}+{endpoint}");
            }
            var json = await response.Content.ReadAsStringAsync();
            if (typeof(T).Name == "String") 
            { 
                json = JsonSerializer.Serialize(json);
                
            }
            if (string.IsNullOrWhiteSpace(json)) { return default; };
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
                throw new HttpRequestException($"POST request failed. Status Code: {response.StatusCode}, Reason: {"PostAsync - " + response.ReasonPhrase}+{endpoint}");
            }

            var json = await response.Content.ReadAsStringAsync();
            if (typeof(TResponse).Name == "String")
            {
                json = JsonSerializer.Serialize(json);

            }
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
                throw new HttpRequestException($"PUT request failed. Status Code: {response.StatusCode}, Reason: {"PutAsync - " + response.ReasonPhrase}");
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
                throw new HttpRequestException($"DELETE request failed. Status Code: {response.StatusCode}, Reason: {"DeleteAsync - " + response.ReasonPhrase}");
            }
        }

    }


}