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

        public async Task<IActionResult> IndexAsync(string searchString, string buttonValue, int clickValue)
        {
            var model = new GameViewModel();

            if (buttonValue == "null" || buttonValue == null)
            {
                ViewData["ClickValue"] = 0;
                ViewData["ButtonValue"] = null;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/games?fields=*");
            /*
             Calls GameResponseHandler async method w/ HttpRequestMessage paramter
             Sets the response to IEnumerable<Game>
             */
            IEnumerable<Game> games = await GameResponseHandler(request);

            if (buttonValue == "Next" && searchString == null)
            {
                var offsetValue = clickValue + 10;
                ViewData["ClickValue"] = offsetValue;
                var pagedRequest = new HttpRequestMessage(HttpMethod.Get, $"/games?fields=*&offset={offsetValue}");
                IEnumerable<Game> pagedGames = await GameResponseHandler(pagedRequest);
                model.Games = pagedGames;
                return View(model);
            }

            if (buttonValue == "Previous" && searchString == null)
            {
                var offsetValue = clickValue - 10;
                ViewData["ClickValue"] = offsetValue;
                var pagedRequest = new HttpRequestMessage(HttpMethod.Get, $"/games?fields=*&offset={offsetValue}");
                IEnumerable<Game> pagedGames = await GameResponseHandler(pagedRequest);
                model.Games = pagedGames;
                return View(model);
            }

            model.Games = games; // sets Games in view model to IEnumerbale<Game> games

            // if search string is NOT null or empty
            if (!String.IsNullOrEmpty(searchString))
            {
                ViewData["SearchString"] = searchString;
                // Makes new request to API w/ search parameter and a limit of 10.
                var searchRequest = new HttpRequestMessage(HttpMethod.Get, $"games?search={searchString}&fields=*");
                // Call GameResponseHandler like before
                IEnumerable<Game> searchedGames = await GameResponseHandler(searchRequest);
                // Set the view model Games to searchedGames response.
                model.Games = searchedGames;

                if (buttonValue == "Next")
                {
                    var offsetValue = clickValue + 10;
                    ViewData["ClickValue"] = offsetValue;
                    var pagedRequest = new HttpRequestMessage(HttpMethod.Get, $"/games?search={searchString}&fields=*&offset={offsetValue}");
                    IEnumerable<Game> pagedGames = await GameResponseHandler(pagedRequest);
                    model.Games = pagedGames;
                    return View(model);
                }

                if (buttonValue == "Previous")
                {
                    var offsetValue = clickValue - 10;
                    ViewData["ClickValue"] = offsetValue;
                    var pagedRequest = new HttpRequestMessage(HttpMethod.Get, $"/games?search={searchString}&fields=*&offset={offsetValue}");
                    IEnumerable<Game> pagedGames = await GameResponseHandler(pagedRequest);
                    model.Games = pagedGames;
                    return View(model);
                }
                return View(model);
            }



            model.Games = games;
            return View(model);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int id) // gets detail of selected game
        {
            var model = new GameViewModel();

            // id is 0 return not found message.
            if (id == 0)
            {
                return NotFound();
            }

            // new request w/ filter of id of the game from the API.
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/games?fields=name,summary,first_release_date,genres,platforms,cover&filter[id][eq]={id}");
            // call the GameResponseHandler
            IEnumerable<Game> gameDetailsResponse = await GameResponseHandler(request);
            // convert IEnumerable to List
            var gameFromApi = gameDetailsResponse.ToList();
            // pull out first (only) item from the list and set to an instance of a Game.
            Game singleGameFromApi = gameFromApi[0];
            // set the Game in View Model to the single Game.
            model.Game = singleGameFromApi;

            // If the coverId is not equal to 0
            if (singleGameFromApi.CoverId != 0)
            {
                // set Id to var coverId
                var coverId = singleGameFromApi.CoverId;
                // Create a list of covers using async CoverApiHandler w/ Id as parameter.
                List<Cover> coverList = await CoverApiHandler(coverId);
                // Create an instance of a single cover from the first (and only) item in the list.
                Cover cover = coverList[0];
                // Set the Cover view model to the single instance.
                model.Cover = cover;
            }

            /* NOTE: Local database was pre-populated w/ all platforms and genres from IGDB Api*/
            // if genreIds and platformIds are not null
            if (singleGameFromApi.GenreIds != null && singleGameFromApi.PlatformIds != null)
            {
                // foreach platform Id
                foreach (var platformId in singleGameFromApi.PlatformIds)
                {
                    // query local DB for the first platform where the ApiPlatformId == platformId
                    var platform = _context.Platforms.First(p => p.ApiPlatformId == platformId);
                    // add the platform to view model.
                    model.Platforms.Add(platform);
                }
                // foreach genre Id
                foreach (var genreId in singleGameFromApi.GenreIds)
                {
                    // query local DB for the first genre where the ApiGenreId == genreId
                    var genre = _context.Genres.First(p => p.ApiGenreId == genreId);
                    // Add genre to view model.
                    model.Genres.Add(genre);
                }
            }
            // put the singleGameFromApi in a ViewBag.
            ViewBag.gameObj = singleGameFromApi;
            // return the View with the GameViewModel
            return View(model);
        }

        public async Task<IActionResult> SaveGame(GameViewModel gameToBeSaved)
        {
            // Save the userId.
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Remove "ApplicationUserId" from ModelState
            ModelState.Remove("ApplicationUserId");

            // If the ModelState is valid
            if (ModelState.IsValid)
            {
                // Take the gameToBeSaved.Game and set the AppUserId to the user;
                gameToBeSaved.Game.ApplicationUserId = user;
                // Add the game to _context (ApplicationDbContext)
                _context.Add(gameToBeSaved.Game);
                // Save changes asynchronously. 
                await _context.SaveChangesAsync();
                // If the GenreIds and PlatformIds are not null.
                if (gameToBeSaved.Game.GenreIds != null && gameToBeSaved.Game.PlatformIds != null)
                {
                    // for each GenreId 
                    foreach (var genreId in gameToBeSaved.Game.GenreIds)
                    {
                        // save first genre where ApiGenreId == genreId to local variable
                        var localGenreObj = _context.Genres.FirstOrDefault(g => g.ApiGenreId == genreId);
                        /* create instance of gameGenre and set the GameId to the id 
                        of the Game and GenreId to id of the genre. */
                        GameGenre gameGenre = new GameGenre
                        {
                            GameId = gameToBeSaved.Game.GameId,
                            GenreId = localGenreObj.GenreId,
                        };
                        // add gameGenre to _context
                        _context.Add(gameGenre);
                        // save changes
                        await _context.SaveChangesAsync();
                    }
                    // for each platformId
                    foreach (var platformId in gameToBeSaved.Game.PlatformIds)
                    {
                        // save first platform where ApiPlatformId == platformId to varialbe
                        var localPlatformObj = _context.Platforms.FirstOrDefault(p => p.ApiPlatformId == platformId);
                        /* create instance of GamePlatform and set the GameId to the id of the
                        game and the Platform Id to the id of the platform.*/
                        GamePlatform gamePlatform = new GamePlatform
                        {
                            GameId = gameToBeSaved.Game.GameId,
                            PlatformId = localPlatformObj.PlatformId
                        };
                        // add gamePlatform to _context
                        _context.Add(gamePlatform);
                        // save changes
                        await _context.SaveChangesAsync();
                    }
                }
            }
            // once saved redirect to logged in user's list of games.
            return RedirectToAction(nameof(MyGamesList));
        }

        public async Task<IActionResult> MyGamesList()
        {
            // save userId
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get all games where userId = logged in User's id.
            var userGames = _context.Games.Where(g => g.ApplicationUserId == user).ToList();
            // return the view of user's games.
            return await Task.Run(() => View(userGames));
        }

        // Allows user to see details of games saved to profile.
        public async Task<IActionResult> MyGameDetails(int id)
        {
            // create instance of GameViewModel
            var model = new GameViewModel();
            if (id == 0)
            {
                return NotFound();
            }

            // get first or default game w/ matching id.
            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.GameId == id);
            // set game to view model.
            model.Game = game;
            // get info for genres
            model.GameGenres = await _context.GameGenres.Include(g => g.Genre)
            .Where(g => g.GameId == id)
            .ToListAsync();
            // get info for platforms.
            model.GamePlatforms = await _context.GamePlatforms.Include(p => p.Platform)
            .Where(p => p.GameId == id)
            .ToListAsync();
            // return the view.
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

        // game response handler helper method w/ http request message as parameter
        public async Task<IEnumerable<Game>> GameResponseHandler(HttpRequestMessage request)
        {
            // create a client using _clientFactory.
            var client = _clientFactory.CreateClient("igdb");
            // wait for response from the client.
            var response = await client.SendAsync(request);
            // read the response it returns as a JsonString.
            var gamesAsJson = await response.Content.ReadAsStringAsync();
            // deserialize string to a list of games 
            var deserializedGames = JsonConvert.DeserializeObject<List<Game>>(gamesAsJson);

            // create an empty list of games.
            List<Game> games = new List<Game>();

            // for each game after deserialization
            foreach (var game in deserializedGames)
            {
                // create instance of a newGame and set values from the API.
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
                games.Add(newGame);
                // if genreIds and platformIds are not null. then create instnace for necessary relationships.
                if (newGame.GenreIds != null && newGame.PlatformIds != null)
                {
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
            return games;
        }

        // CoverApiHandler which follows similar pattern as GameApiResponse.
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
    }
}
