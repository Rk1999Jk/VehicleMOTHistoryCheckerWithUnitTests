using Microsoft.Extensions.Configuration;
using System.Text.Json;
using VechicleData.Data;
using VechicleInfo.Data;

namespace VechicleInfo.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly HttpClient _httpClient;
        
        private readonly string _apiKey;
        public VehicleService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["VehicleServiceConfig:ApiKey"];
        }
        
        public async Task<VehicleInformation> GetVehicleInfo(string registrationNumber)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests?registration={registrationNumber}");
            request.Headers.Add("x-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            //Name plate info exists
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var vehicles = JsonSerializer.Deserialize<List<VehicleResponse>>(responseContent, options);
                var vehicle = vehicles?.FirstOrDefault();
                var latestMotTest = vehicle?.MotTests?.OrderByDescending(t => t.CompletedDate).FirstOrDefault();
                 
                return new VehicleInformation
                {
                    Make = vehicle?.Make,
                    Model = vehicle?.Model,
                    Colour = vehicle?.PrimaryColour,
                    MotExpiryDate = latestMotTest?.ExpiryDate,
                    MileageAtLastMot = latestMotTest?.OdometerValue
                    
                };
                
                
            }

            //When vehicle nameplate does not exist
            return null;

        }
    }
}
