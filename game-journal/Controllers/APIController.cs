using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> IndexAsync(/*string body*/)
        {
            // replicate this and add parameter for specific searches. 
            // also need to get params into body of request. named client model
            var request = new HttpRequestMessage(HttpMethod.Get, "/games?fields=name&filter[id][eq]=1942");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            //body = "fields *; where id = 1942;";
            string gamesAsJson = await response.Content.ReadAsStringAsync();
            return View();
        }
    }
}