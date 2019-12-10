using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace game_journal.Controllers
{
    public class APIController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public APIController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/games");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            string name = await response.Content.ReadAsStringAsync();
            return View();
        }
    }
}