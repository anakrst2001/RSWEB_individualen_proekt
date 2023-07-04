using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RecordStore.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.Models;
using NuGet.ContentModel;
using System.Diagnostics.Metrics;
using System.Numerics;
using Humanizer.Bytes;
using Humanizer;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RecordStore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecordStore.Controllers
{
    public class UserAlbumsController : Controller
    {
        private readonly RecordStoreContext _context;
        private readonly UserManager<RecordStoreUser> _userManager;

        public UserAlbumsController(RecordStoreContext context, UserManager<RecordStoreUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        private Task<RecordStoreUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: UserAlbums
        public async Task<IActionResult> Index()
        {
            var recordStoreContext = _context.UserAlbum.Include(m => m.Album).ThenInclude(m => m.Artist).ThenInclude(m => m.Songs).ThenInclude(m => m.Genres);
            return View(await recordStoreContext.ToListAsync());
        }

        // GET: UserAlbums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserAlbum == null)
            {
                return NotFound();
            }

            var userAlbum = await _context.UserAlbum
                .Include(u => u.Album)
                .ThenInclude(m => m.Artist).ThenInclude(m => m.Songs).ThenInclude(m => m.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAlbum == null)
            {
                return NotFound();
            }

            return View(userAlbum);
        }

        // GET: UserAlbums/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Name");
            return View();
        }

        // POST: UserAlbums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUser,AlbumId")] UserAlbum userAlbum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAlbum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Name", userAlbum.AlbumId);
            return View(userAlbum);
        }

        // GET: UserAlbums/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserAlbum == null)
            {
                return NotFound();
            }

            var userAlbum = await _context.UserAlbum.FindAsync(id);
            if (userAlbum == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Name", userAlbum.AlbumId);
            return View(userAlbum);
        }

        // POST: UserAlbums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUser,AlbumId")] UserAlbum userAlbum)
        {
            if (id != userAlbum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAlbum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAlbumExists(userAlbum.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Name", userAlbum.AlbumId);
            return View(userAlbum);
        }

        // GET: UserAlbums/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserAlbum == null)
            {
                return NotFound();
            }

            var userAlbum = await _context.UserAlbum
                .Include(u => u.Album)
                .ThenInclude(m => m.Artist).ThenInclude(m => m.Songs).ThenInclude(m => m.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAlbum == null)
            {
                return NotFound();
            }

            return View(userAlbum);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToMyRecords(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var RecordStoreContext = _context.UserAlbum.Where(m => m.AlbumId == id);
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                UserAlbum userAlbum = new UserAlbum();
                userAlbum.AlbumId = (int)id;
                userAlbum.AppUser = user.UserName;
                _context.UserAlbum.Add(userAlbum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAlbums));
            }
            if (RecordStoreContext != null)
            {
                return View(await RecordStoreContext.ToListAsync());
            }
            else
            {
                return Problem("Entity set 'RecordStoreContext.UserAlbum' is null!");
            }
        }

        [Authorize(Roles = "User")]
        
        public async Task<IActionResult> MyAlbums()
        {
            var user = await GetCurrentUserAsync();
            var UserAlbumContext = _context.UserAlbum.AsQueryable().Include(m => m.Album).ThenInclude(m => m.Artist).ThenInclude(m => m.Songs).ThenInclude(m => m.Genres).Where(m => m.AppUser == user.UserName);
            var MyRecordsList = _context.Album.AsQueryable();
            MyRecordsList = UserAlbumContext.Select(m => m.Album);
            if (UserAlbumContext != null)
            {
                return View("~/Views/UserAlbums/MyAlbums.cshtml", await MyRecordsList.ToListAsync());
            }
            else
            {
                return Problem("Entity set 'RecordStoreContext.UserAlbum' is null!");
            }
        }

        // POST: UserAlbums/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserAlbum == null)
            {
                return Problem("Entity set 'RecordStoreContext.UserAlbum'  is null.");
            }
            var userAlbum = await _context.UserAlbum.FindAsync(id);
            if (userAlbum != null)
            {
                _context.UserAlbum.Remove(userAlbum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAlbumExists(int id)
        {
          return (_context.UserAlbum?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
