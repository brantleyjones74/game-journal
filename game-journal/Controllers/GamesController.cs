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
using PagedList;

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

        public async Task<IEnumerable<Game>> GameResponseHandler(HttpRequestMessage request)
        {
            var client = _clientFactory.CreateClient("igdb");
            var response =  await client.SendAsync(request);
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
        // GET: Games
        public async Task<IActionResult> IndexAsync() // returns a list of games from DB
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/games?fields=*");
            IEnumerable<Game> games = await GameResponseHandler(request).ConfigureAwait(false);

            return View(games);
        }

        // GET: Search Games By Name
        public async Task<IActionResult> SearchByName(string gameName, string currentFilter, int? pageNumber)
        {
            // request and deserialize from API
            var request = new HttpRequestMessage(HttpMethod.Get, $"games?search={gameName}&fields=id,name,summary,cover,first_release_date&limit=200");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            var deserializedGames = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            // new list of SearchedGames
            List<Game> searchedGames = new List<Game>();

            // grabbing data from deserializedGames and add to searchedGames
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
                searchedGames.Add(newGame);
            }

            //ViewData["CurrentFilter"] = searchedGames;
            //currentFilter = searchedGames.ToString();
            //pageNumber = 1;
            //int pageSize = 10;
            // takes a single page number. 
            // ?? represents null-coalescing operator. this defines a default value for a nullable type.
            // (pageNumber ?? 1) means return value of pageNumber if greater than 1 or return 1 if null.
            // return View(await PaginatedList<Game>.CreateAsync(searchedGames, pageNumber ?? 1, pageSize));
            return View(searchedGames);
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
                    var coverRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/covers?fields=*&filter[id][eq]={coverId}");
                    var coverClient = _clientFactory.CreateClient("igdb");
                    var coverResponse = await client.SendAsync(coverRequest);
                    var coverAsJson = await coverResponse.Content.ReadAsStringAsync();
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
                        model.Cover = newCover;
                    }
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
    }
}
