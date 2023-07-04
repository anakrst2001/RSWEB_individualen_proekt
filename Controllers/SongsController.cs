using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Data;
using RecordStore.Interfaces;
using RecordStore.Models;
using RecordStore.ViewModels;

namespace RecordStore.Controllers
{
    public class SongsController : Controller
    {
        private readonly RecordStoreContext _context;
        private readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SongsController(RecordStoreContext context, IBufferedFileUploadService bufferedFileUploadService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Songs
        /*
        public async Task<IActionResult> Index()
        {
            var recordStoreContext = _context.Song.Include(s => s.Album).Include(s => s.Artist).Include(m => m.Genres).ThenInclude(m => m.Genre);
            return View(await recordStoreContext.ToListAsync());
        }
        */

        /*
        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Song == null)
            {
                return Problem("Entity set 'RecordStoreContext.Song'  is null.");
            }

            var songs = from s in _context.Song
                         select s;
            songs = songs.Include(s => s.Artist).Include(m => m.Genres).ThenInclude(m => m.Genre);

            if (id < 1 || id == null)
            {
                songs = songs.Include(m => m.Album);
            }
            else
            {
                songs = songs.Include(m => m.Album).Where(b => b.AlbumId == id);
            }
            return View(await songs.ToListAsync());
        }
        */

        public async Task<IActionResult> Index(int? id, string Title, string Artist, string Album, string Genre)
        {
            IQueryable<Song> songs = _context.Song.AsQueryable();
            songs = songs.Include(s => s.Artist).Include(m => m.Genres).ThenInclude(m => m.Genre);
            if (id < 1 || id == null)
            {
                songs = songs.Include(m => m.Album);
            }
            else
            {
                songs = songs.Include(m => m.Album).Where(b => b.AlbumId == id);
            }
            IQueryable<string> genreQuery = _context.Genre.Select(m => m.GenreName).Distinct();
            if (!string.IsNullOrEmpty(Title))
            {
                songs = songs.Where(s => s.Title.Contains(Title));
            }
            if (!string.IsNullOrEmpty(Artist))
            {
                songs = songs.Where(s => s.Artist.FirstName.Contains(Artist) || s.Artist.LastName.Contains(Artist));
            }
            if (!string.IsNullOrEmpty(Album))
            {
                songs = songs.Where(s => s.Album.Name.Contains(Album));
            }
            if (!string.IsNullOrEmpty(Genre))
            {
                songs = songs.Where(s => s.Genres.Any(m => m.Genre.GenreName == Genre));
            }
            var VM = new SongTitleArtistAlbumGenre
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Songs = await songs.ToListAsync()
            };
            return View(VM);
        }
        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Album)
                .Include(s => s.Artist)
                .Include(m => m.Genres).ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Set<Album>(), "Id", "Name");
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "FullName");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,YearReleased,Description,Length,Image,ArtistId,AlbumId")] Song song)
        {
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Set<Album>(), "Id", "Name", song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "FullName", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Edit/5
        /*
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Set<Album>(), "Id", "Name", song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "FullName", song.ArtistId);
            return View(song);
        }
        */
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = _context.Song.Where(m => m.Id == id).Include(m => m.Genres).FirstOrDefault();

            if (song == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.OrderBy(s => s.GenreName).ToList();

            SongGenresViewModel viewmodel = new SongGenresViewModel
            {
                Song = song,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = song.Genres?.Select(sa => sa.GenreId.Value),
            };

            ViewData["AlbumId"] = new SelectList(_context.Set<Album>(), "Id", "Name", song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", song.ArtistId);
            return View(viewmodel);
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            var genres = await _context.Genre.OrderBy(g => g.GenreName).ToListAsync();
            var selectedGenres = song.Genres.Select(g => g.GenreId.Value).ToList();

            var viewModel = new SongGenresViewModel
            {
                Song = song,
                GenreList = new SelectList(genres, "Id", "GenreName"),
                SelectedGenres = selectedGenres
            };

            return View(viewModel);
        }
        */
        

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SongGenresViewModel viewmodel, IFormFile? file)
        {
            if (id != viewmodel.Song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Song);
                    var existingGenres = _context.SongGenre.Where(ag => ag.SongId == id);
                    _context.SongGenre.RemoveRange(existingGenres);
                    if (viewmodel.SelectedGenres != null)
                    {
                        foreach (int genreId in viewmodel.SelectedGenres)
                        {
                            _context.SongGenre.Add(new SongGenre { SongId = id, GenreId = genreId });
                        }
                    }

                    if (file != null)
                    {
                        string slika_pateka = await _bufferedFileUploadService.UploadFile(file, _webHostEnvironment);
                        if (slika_pateka != "none")
                        {
                            ViewBag.Message = "File Upload Successful!";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed!";
                        }
                        viewmodel.Song.Image = slika_pateka;
                        _context.Update(viewmodel.Song);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(viewmodel.Song.Id))
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

            ViewData["AlbumId"] = new SelectList(_context.Set<Album>(), "Id", "Name", viewmodel.Song.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", viewmodel.Song.ArtistId);
            return View(viewmodel);
        }
        /*
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = _context.Song.Where(m => m.Id == id).Include(m => m.Genres).FirstOrDefault();

            if (song == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.OrderBy(s => s.GenreName).ToList();

            SongGenresViewModel viewmodel = new SongGenresViewModel
            {
                Song = song,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = song.Genres?.Select(sa => sa.GenreId.Value)
            };

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", song.ArtistId);
            return View(viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SongGenresViewModel viewmodel)
        {
            if (id != viewmodel.Song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Song);
                    var existingGenres = _context.SongGenre.Where(ag => ag.SongId == id);
                    _context.SongGenre.RemoveRange(existingGenres);
                    if (viewmodel.SelectedGenres != null)
                    {
                        foreach (int genreId in viewmodel.SelectedGenres)
                        {
                            _context.SongGenre.Add(new SongGenre { SongId = id, GenreId = genreId });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(viewmodel.Song.Id))
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

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", viewmodel.Song.ArtistId);
            return View(viewmodel);
        }
        */
        // GET: Songs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Album)
                .Include(s => s.Artist)
                .Include(m => m.Genres).ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Song == null)
            {
                return Problem("Entity set 'RecordStoreContext.Song'  is null.");
            }
            var song = await _context.Song.FindAsync(id);
            if (song != null)
            {
                _context.Song.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
          return (_context.Song?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
