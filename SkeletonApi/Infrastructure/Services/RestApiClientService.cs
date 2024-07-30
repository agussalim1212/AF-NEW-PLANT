using SkeletonApi.Application.DTOs.RestApiData;
using SkeletonApi.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SkeletonApi.Infrastructure.Services
{
    public class RestApiClientService : IRestApiClientService
    {
        private readonly HttpClient _httpClient;

        public RestApiClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendAsync(RestDataTraceability request)
        {
            string username = "training103";
            string password = "Matah4r1@@";
            await Console.Out.WriteLineAsync("MASUK SINI");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{username}:{password}")));
            var result = await _httpClient.PostAsync("https://portaldev.ahm.co.id/iit01/ahmsvitsys000-ahs/rest/it/sys001/login", null);
            var response = await result.Content.ReadAsStringAsync();
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
            await Task.CompletedTask;
        }
    }
}