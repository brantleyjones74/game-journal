using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using game_journal.Data;
using game_journal.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using game_journal.Models.View_Models;
using System;

namespace game_journal.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> IndexAsync(string sortOrder, string searchString, string currentFilter, int? page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/games?fields=*");
            IEnumerable<Game> games = await GameResponseHandler(request);

            if (!String.IsNullOrEmpty(searchString))
            {
                var searchRequest = new HttpRequestMessage(HttpMethod.Get, $"games?search={searchString}&fields=*&limit=200");
                IEnumerable<Game> searchedGames = await GameResponseHandler(searchRequest);

                page = 1;
                searchString = currentFilter;
                ViewBag.CurrentFilter = searchString;


                SortGames(sortOrder, searchedGames);
                return View(searchedGames);
            }

            SortGames(sortOrder, games);
            return View(games);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int id) // gets detail of selected game
        {
            var model = new GameViewModel();

            if (id == 0)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/games?fields=name,summary,first_release_date,genres,platforms,cover&filter[id][eq]={id}");
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
                    PlatformIds = game.PlatformIds,
                    CoverId = game.CoverId
                };
                gameFromApi.Add(newGame);

                if (game.CoverId != 0)
                {
                    var coverId = game.CoverId;
                    List<Cover> coverList = await CoverApiHandler(coverId);
                    Cover cover = coverList[0];
                    model.Cover = cover;
                }

                if (newGame.GenreIds != null && newGame.PlatformIds != null)
                {
                    // makes join table relationships for genres and platforms
                    foreach (var genreId in newGame.GenreIds)
                    {
                        GameGenre gameGenre = new GameGenre
                        {
                            GameId = newGame.GameId,
                            GenreId = genreId
                        };
                    }
                    foreach (var platformId in newGame.PlatformIds)
                    {
                        GamePlatform gamePlatform = new GamePlatform
                        {
                            GameId = newGame.GameId,
                            PlatformId = platformId
                        };
                    }
                }
            }
            Game singleGameFromApi = gameFromApi[0];

            model.Game = singleGameFromApi;
            // get genres and platforms in the model
            if (singleGameFromApi.GenreIds != null && singleGameFromApi.PlatformIds != null)
            {
                foreach (var platformId in singleGameFromApi.PlatformIds)
                {
                    var platform = _context.Platforms.First(p => p.ApiPlatformId == platformId);
                    model.Platforms.Add(platform);
                }
                foreach (var genreId in singleGameFromApi.GenreIds)
                {
                    var genre = _context.Genres.First(p => p.ApiGenreId == genreId);
                    model.Genres.Add(genre);
                }
            }
            ViewBag.gameObj = singleGameFromApi;
            return View(model);
        }

        public async Task<IActionResult> SaveGame(GameViewModel gameToBeSaved)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ModelState.Remove("ApplicationUserId");

            if (ModelState.IsValid)
            {
                gameToBeSaved.Game.ApplicationUserId = user;
                _context.Add(gameToBeSaved.Game);
                await _context.SaveChangesAsync();
                if (gameToBeSaved.Game.GenreIds != null && gameToBeSaved.Game.PlatformIds != null)
                {
                    foreach (var genreId in gameToBeSaved.Game.GenreIds)
                    {
                        var localGenreObj = _context.Genres.FirstOrDefault(g => g.ApiGenreId == genreId);
                        GameGenre gameGenre = new GameGenre
                        {
                            GameId = gameToBeSaved.Game.GameId,
                            GenreId = localGenreObj.GenreId,
                        };
                        _context.Add(gameGenre);
                        await _context.SaveChangesAsync();
                    }
                    foreach (var platformId in gameToBeSaved.Game.PlatformIds)
                    {
                        var localPlatformObj = _context.Platforms.FirstOrDefault(p => p.ApiPlatformId == platformId);
                        GamePlatform gamePlatform = new GamePlatform
                        {
                            GameId = gameToBeSaved.Game.GameId,
                            PlatformId = localPlatformObj.PlatformId
                        };
                        _context.Add(gamePlatform);
                        await _context.SaveChangesAsync();
                    }
                }

            }
            return RedirectToAction(nameof(MyGamesList));
        }

        public async Task<IActionResult> MyGamesList()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userGames = _context.Games.Where(g => g.ApplicationUserId == user).ToList();

            return await Task.Run(() => View(userGames));
        }

        public async Task<IActionResult> MyGameDetails(int id)
        {
            var model = new GameViewModel();
            if (id == 0)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.GameId == id);
            model.Game = game;

            model.GameGenres = await _context.GameGenres.Include(g => g.Genre)
            .Where(g => g.GameId == id)
            .ToListAsync();

            model.GamePlatforms = await _context.GamePlatforms.Include(p => p.Platform)
            .Where(p => p.GameId == id)
            .ToListAsync();

            return View(model);
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
            if (id == 0)
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


        /*************** HELPER METHODS ***************/
        // Handles the response from the API for a game.
        public async Task<IEnumerable<Game>> GameResponseHandler(HttpRequestMessage request)
        {
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGames = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            List<Game> games = new List<Game>();

            foreach (var game in deserializedGames)
            {
                Game newGame = new Game
                {
                    GameId = game.GameId,
                    Name = game.Name,
                    Summary = game.Summary,
                    first_release_date = game.first_release_date,
                    CoverId = game.CoverId
                };
                games.Add(newGame);
            }

            return games;
        }

        public async Task<List<Cover>> CoverApiHandler(int coverId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/covers?fields=*&filter[id][eq]={coverId}");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var coverAsJson = await response.Content.ReadAsStringAsync();
            var deserializedCover = JsonConvert.DeserializeObject<List<Cover>>(coverAsJson);

            List<Cover> coverFromApi = new List<Cover>();

            foreach (var cover in deserializedCover)
            {
                Cover newCover = new Cover
                {
                    CoverId = coverId,
                    ImageId = cover.ImageId,
                    PxlHeight = cover.PxlHeight,
                    PxlWidth = cover.PxlWidth,
                    Url = cover.Url,
                };
                coverFromApi.Add(newCover);
            }
            return coverFromApi;
        }

        public IEnumerable<Game> SortGames(string sortOrder, IEnumerable<Game> games)
        {
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            switch (sortOrder)
            {
                case "Name":
                    games = games.OrderBy(g => g.Name);
                    break;
                case "name_desc":
                    games = games.OrderByDescending(g => g.Name);
                    break;
                case "date_desc":
                    games = games.OrderByDescending(g => g.ReleaseDate);
                    break;
                default:
                    games = games.OrderBy(g => g.ReleaseDate);
                    break;
            }
            return games;
        }
    }
}
