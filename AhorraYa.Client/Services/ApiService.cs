using Microsoft.AspNetCore.Http.Connections.Features;
using System.Net.Http.Headers;

namespace AhorraYa.WebClient.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _factory = factory;
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClient CreateClient()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7284/");

            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if(!string.IsNullOrEmpty(token) )
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
