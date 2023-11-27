namespace VechicleInfo
{
    using Xunit;
    using Moq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using VechicleInfo.Services;
    using Moq.Protected;
    using System.Threading;
    public class VehicleServiceTest
    {
        //Vehicle Registration Number is found 
        [Fact]
        public async Task GetVehicleInfo_ReturnsVehicleInformation_OnSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["VehicleServiceConfig:ApiKey"]).Returns("test_api_key");

            var vehicleService = new VehicleService(httpClient, configurationMock.Object);

            var registrationNumber = "TEST123";
            var responseContent = "[{\"registration\":\"TEST123\",\"make\":\"TestMake\",\"model\":\"TestModel\",\"primaryColour\":\"Red\",\"motTests\":[{\"completedDate\":\"2023-06-01 10:00:00\",\"testResult\":\"PASSED\",\"expiryDate\":\"2024-06-01\",\"odometerValue\":\"12345\",\"odometerUnit\":\"mi\",\"motTestNumber\":\"1234567890\",\"rfrAndComments\":[{\"text\":\"Rear Brake pad(s) wearing thin (1.1.13 (a) (ii))\",\"type\":\"ADVISORY\"}]}]}]"; // Mocked JSON response

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains(registrationNumber)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await vehicleService.GetVehicleInfo(registrationNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestMake", result.Make);
            Assert.Equal("TestModel", result.Model);
            Assert.Equal("Red", result.Colour);
            Assert.Equal("2024-06-01", result.MotExpiryDate);
            Assert.Equal("12345", result.MileageAtLastMot);
            
        }
        //Vehicle Registration Number is not found 
        [Fact]
        public async Task GetVehicleInfo_ReturnsNull_WhenVehicleNotFound()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["VehicleServiceConfig:ApiKey"]).Returns("test_api_key");

            var vehicleService = new VehicleService(httpClient, configurationMock.Object);

            var registrationNumber = "NOTFOUND";

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains(registrationNumber)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await vehicleService.GetVehicleInfo(registrationNumber);

            // Assert
            Assert.Null(result);
        }


    }
}