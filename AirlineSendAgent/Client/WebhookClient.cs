using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AirlineSendAgent.Dtos;

namespace AirlineSendAgent.Client
{
    public class WebhookClient : IWebhookClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task SendWebhookNotificationAsync(FlightDetailChangePayloadDto flightDetailChangePayloadDto)
        {
            var serializedPayload = JsonSerializer.Serialize(flightDetailChangePayloadDto);

            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, flightDetailChangePayloadDto.WebhookURI);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedPayload);

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try 
            {
                using (var responce = await httpClient.SendAsync(request))
                {
                    responce.EnsureSuccessStatusCode();
                    Console.WriteLine("Success");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unsuccessful {ex.Message}");
            }
        }
    }
}