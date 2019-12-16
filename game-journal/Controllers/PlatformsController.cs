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
    public class PlatformsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public PlatformsController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> GetPlatformById(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/platforms?fields=name&filter[id][eq]={id}");
            var client = _clientFactory.CreateClient("igdb");
            var response = await client.SendAsync(request);
            var platformAsJson = await response.Content.ReadAsStringAsync();
            var deserializedPlatform = JsonConvert.DeserializeObject<List<Platform>>(platformAsJson);

            List<Platform> platformsFromApi = new List<Platform>();

            foreach (var platform in deserializedPlatform)
            {
                Platform newPlatform = new Platform
                {
                    PlatformId = platform.PlatformId,
                    Name = platform.Name
                };

                platformsFromApi.Add(newPlatform);

            }

            return View(platformsFromApi);
        }

        //public async Task<IActionResult> GetAllPlatforms()
        //{
            //var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/platforms?fields=id,name");
            //var client = _clientFactory.CreateClient("igdb");
            //var response = await client.SendAsync(request);
            //var platformAsJson = await response.Content.ReadAsStringAsync();
            //var deserializedPlatform = JsonConvert.DeserializeObject<List<Platform>>(platformAsJson);
        //}


        // GET: Platforms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Platforms.ToListAsync());
        }

        // GET: Platforms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platforms
                .FirstOrDefaultAsync(m => m.PlatformId == id);
            if (platform == null)
            {
                return NotFound();
            }

            return View(platform);
        }

        // GET: Platforms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Platforms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlatformId,Name")] Platform platform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(platform);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }

        // GET: Platforms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }
            return View(platform);
        }

        // POST: Platforms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlatformId,Name")] Platform platform)
        {
            if (id != platform.PlatformId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(platform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformExists(platform.PlatformId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }

        // GET: Platforms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platforms
                .FirstOrDefaultAsync(m => m.PlatformId == id);
            if (platform == null)
            {
                return NotFound();
            }

            return View(platform);
        }

        // POST: Platforms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var platform = await _context.Platforms.FindAsync(id);
            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformExists(int id)
        {
            return _context.Platforms.Any(e => e.PlatformId == id);
        }
    }
}
