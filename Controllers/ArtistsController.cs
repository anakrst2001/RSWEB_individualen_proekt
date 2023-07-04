using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.Models;
using RecordStore.ViewModels;

namespace RecordStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly RecordStoreContext _context;

        public ArtistsController(RecordStoreContext context)
        {
            _context = context;
        }

        // GET: Artists
        /*
        public async Task<IActionResult> Index()
        {
              return _context.Artist != null ? 
                          View(await _context.Artist.ToListAsync()) :
                          Problem("Entity set 'RecordStoreContext.Artist'  is null.");
        }
        */

        public async Task<IActionResult> Index(string artistNationality, string artistFirstName, string artistLastName)
        {
            IQueryable<Artist> artists = _context.Artist.AsQueryable();
            IQueryable<string> nationalityQuery = _context.Artist.OrderBy(m => m.Nationality).Select(m => m.Nationality).Distinct();
            if (!string.IsNullOrEmpty(artistFirstName))
            {
                artists = artists.Where(s => s.FirstName.Contains(artistFirstName));
            }
            if (!string.IsNullOrEmpty(artistLastName))
            {
                artists = artists.Where(s => s.LastName.Contains(artistLastName));
            }
            if (!string.IsNullOrEmpty(artistNationality))
            {
                artists = artists.Where(x => x.Nationality == artistNationality);
            }
            artists = artists.Include(m => m.Songs).Include(m => m.Albums);
            var artistNameSurnameNationalityVM = new ArtistNameSurnameNationalityFilter
            {
                Nationalities = new SelectList(await nationalityQuery.ToListAsync()),
                Artists = await artists.ToListAsync()
            };
            return View(artistNameSurnameNationalityVM);
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artist == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artist == null)
            {
                return Problem("Entity set 'RecordStoreContext.Artist'  is null.");
            }
            var artist = await _context.Artist.FindAsync(id);
            if (artist != null)
            {
                _context.Artist.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
          return (_context.Artist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
