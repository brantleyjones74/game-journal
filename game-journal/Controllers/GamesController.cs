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
using System.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace game_journal.Controllers
{
    public class GamesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _context = context;
        }
        // GET: Games
        public async Task<IActionResult> IndexAsync() // returns a list of games from DB
        {
            var model = new GameViewModel();

            var request = new HttpRequestMessage(HttpMethod.Get, "/games?fields=name,first_release_date,cover.url,summary");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGames = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            List<Game> gamesFromApi = new List<Game>();

            foreach (var game in deserializedGames)
            {
                Game newGame = new Game
                {
                    GameId = game.GameId,
                    Name = game.Name,
                    Summary = game.Summary,
                    first_release_date = game.first_release_date
                };
                gamesFromApi.Add(newGame);
            }

            return View(gamesFromApi);
        }

        // GET: Search Games By Name
        public async Task<IActionResult> SearchByName(string gameName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"games?search={gameName}&fields=id,name,summary,cover,first_release_date");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGames = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            List<Game> searchedGames = new List<Game>();

            foreach (var game in deserializedGames)
            {
                Game newGame = new Game
                {
                    GameId = game.GameId,
                    Name = game.Name,
                    Summary = game.Summary,
                    first_release_date = game.first_release_date
                };
                searchedGames.Add(newGame);
            }

            return View(searchedGames);

        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int id) // gets detail of selected game
        {
            if (id == 0)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/games?fields=name,summary,first_release_date,genres,platforms&filter[id][eq]={id}");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGame = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            List<Game> gameFromApi = new List<Game>();

            foreach (var game in deserializedGame)
            {
                Game newGame = new Game
                {
                    GameId = game.GameId,
                    Name = game.Name,
                    Summary = game.Summary,
                    first_release_date = game.first_release_date,
                    GenreIds = game.GenreIds,
                    PlatformIds = game.PlatformIds
                };
                gameFromApi.Add(newGame);
            }

            Game singleGameFromApi = gameFromApi[0];
            ViewBag.gameObj = singleGameFromApi;
            return View(singleGameFromApi);
        }

        public async Task<IActionResult> SaveGame(Game singleGameFromApi)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ModelState.Remove("ApplicationUserId");

            if (ModelState.IsValid)
            {
                singleGameFromApi.ApplicationUserId = user;
                _context.Add(singleGameFromApi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyGamesList));
        }

        public async Task<IActionResult> MyGamesList()
        {
            //var model = new MyGamesViewModel();
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userGames = _context.Games.Where(g => g.ApplicationUserId == user).ToList();

            //model.Games = userGames;

            return await Task.Run(() => View(userGames));
        }

        public async Task<IActionResult> MyGameDetails(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var game = await _context.Games.FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        //GET: Games/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        //POST: Games/Edit/5
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameId,Name,first_release_date,_releaseDate,ReleaseDate,Summary,Notes,HoursPlayed,UserRating,ApplicationUserId")] Game game)
        {
            if (id != game.GameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.GameId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyGamesList));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyGamesList));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }


    }
}
