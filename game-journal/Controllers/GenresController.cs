using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using game_journal.Data;
using game_journal.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace game_journal.Controllers
{
    public class GenresController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _context = context;
        }

        // Get all genres and add to DB
        public async Task<IActionResult> GetAllAndSaveGenres()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/genres?fields=name,id&limit=40");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var genresAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGenres = JsonConvert.DeserializeObject<List<Genre>>(genresAsJson);

            List<Genre> genresFromApi = new List<Genre>();

            foreach (var genreObj in deserializedGenres)
            {
                Genre newGenre = new Genre
                {
                    ApiGenreId = genreObj.ApiGenreId,
                    Name = genreObj.Name
                };

                if (!GenreExists(genreObj.ApiGenreId))
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(newGenre);
                        await _context.SaveChangesAsync();
                    }

                }
                genresFromApi.Add(newGenre);
            }
            return Redirect("/Games/Index");
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.ApiGenreId == id);
        }
    }
}
