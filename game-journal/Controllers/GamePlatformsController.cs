using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using game_journal.Data;
using game_journal.Models;

namespace game_journal.Controllers
{
    public class GamePlatformsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamePlatformsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GamePlatforms
        public async Task<IActionResult> Index()
        {
            return View(await _context.GamePlatforms.ToListAsync());
        }

        // GET: GamePlatforms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamePlatform = await _context.GamePlatforms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gamePlatform == null)
            {
                return NotFound();
            }

            return View(gamePlatform);
        }

        // GET: GamePlatforms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GamePlatforms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameId,GameName,PlatformId,PlatformName")] GamePlatform gamePlatform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gamePlatform);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gamePlatform);
        }

        // GET: GamePlatforms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamePlatform = await _context.GamePlatforms.FindAsync(id);
            if (gamePlatform == null)
            {
                return NotFound();
            }
            return View(gamePlatform);
        }

        // POST: GamePlatforms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,GameName,PlatformId,PlatformName")] GamePlatform gamePlatform)
        {
            if (id != gamePlatform.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gamePlatform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GamePlatformExists(gamePlatform.Id))
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
            return View(gamePlatform);
        }

        // GET: GamePlatforms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamePlatform = await _context.GamePlatforms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gamePlatform == null)
            {
                return NotFound();
            }

            return View(gamePlatform);
        }

        // POST: GamePlatforms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gamePlatform = await _context.GamePlatforms.FindAsync(id);
            _context.GamePlatforms.Remove(gamePlatform);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GamePlatformExists(int id)
        {
            return _context.GamePlatforms.Any(e => e.Id == id);
        }
    }
}
