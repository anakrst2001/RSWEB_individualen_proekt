using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
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
    public class AlbumsController : Controller
    {
        private readonly RecordStoreContext _context;
        private readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AlbumsController(RecordStoreContext context, IBufferedFileUploadService bufferedFileUploadService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Albums
        /*
        public async Task<IActionResult> Index()
        {
            var recordStoreContext = _context.Album.Include(a => a.Artist).Include(a=>a.Songs).Include(a=>a.Genres).ThenInclude(a=>a.Genre).Include(a=>a.Reviews);
            return View(await recordStoreContext.ToListAsync());
        }
        */

        /*
        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Album == null)
            {
                return Problem("Entity set 'RecordStoreContext.Album'  is null.");
            }

            var albums = from a in _context.Album
                        select a;
            albums = albums.Include(p => p.Reviews).Include(p=>p.Songs).Include(p=>p.Genres).ThenInclude(p=>p.Genre);

            if (id < 1 || id == null)
            {
                albums = albums.Include(m => m.Artist);
            }
            else
            {
                albums = albums.Include(m => m.Artist).Where(b => b.ArtistId == id);
            }
            return View(await albums.ToListAsync());
        }
        */

        public async Task<IActionResult> Index(int? id, string Name, string Artist, string Genre)
        {
            IQueryable<Album> albumi = _context.Album.AsQueryable();
            if (id < 1 || id == null)
            {
                albumi = albumi.Include(m => m.Artist);
            }
            else
            {
                albumi = albumi.Include(m => m.Artist).Where(b => b.ArtistId == id);
            }
            albumi = albumi.Include(p => p.Reviews).Include(p => p.Songs).Include(p => p.Genres).ThenInclude(p => p.Genre);
            IQueryable<string> genreQuery = _context.Genre.Select(m => m.GenreName).Distinct();
            if (!string.IsNullOrEmpty(Name))
            {
                albumi = albumi.Where(s => s.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Artist))
            {
                albumi = albumi.Where(s => s.Artist.FirstName.Contains(Artist) || s.Artist.LastName.Contains(Artist));
            }
            if (!string.IsNullOrEmpty(Genre))
            {
                albumi = albumi.Where(s => s.Genres.Any(m => m.Genre.GenreName == Genre));
            }
            var VM = new AlbumNameArtistGenreFilter
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Albumi = await albumi.ToListAsync()
            };
            return View(VM);
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .Include(a => a.Genres).ThenInclude(a => a.Genre)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        /*
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Released,Image,Label,ArtistId")] Album album)
        {
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", album.ArtistId);
            return View(album);
        }
        */
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var genres = _context.Genre.OrderBy(s => s.GenreName);
            var songs = _context.Song.OrderBy(s => s.Title);

            var viewmodel = new AlbumGenresViewModel
            {
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SongList = new MultiSelectList(songs, "Id", "Title")
            };

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName");
            return View(viewmodel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumGenresViewModel viewmodel, IFormFile? imagefile)
        {
            if (ModelState.IsValid && viewmodel.Album != null)
            {
                var album = viewmodel.Album;

                if (imagefile != null)
                {
                    string slika_pateka = await _bufferedFileUploadService.UploadFile(imagefile, _webHostEnvironment);
                    if (slika_pateka != "none")
                    {
                        ViewBag.Message = "File Upload Successful!";
                        album.Image = slika_pateka;
                    }
                    else
                    {
                        ViewBag.Message = "File Upload Failed!";
                    }
                }

                _context.Album.Add(album);
                await _context.SaveChangesAsync();

                if (viewmodel.SelectedGenres != null)
                {
                    var selectedGenres = _context.Genre.Where(g => viewmodel.SelectedGenres.Contains(g.Id));
                    foreach (var genre in selectedGenres)
                    {
                        _context.AlbumGenre.Add(new AlbumGenre { Album = album, Genre = genre });
                    }
                }

                if (viewmodel.SelectedSongs != null)
                {
                    var selectedSongs = _context.Song.Where(s => viewmodel.SelectedSongs.Contains(s.Id));
                    foreach (var song in selectedSongs)
                    {
                        song.Album = album;
                        _context.Song.Update(song);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var genres = _context.Genre.OrderBy(s => s.GenreName);
            var songs = _context.Song.OrderBy(s => s.Title);

            viewmodel.GenreList = new MultiSelectList(genres, "Id", "GenreName");
            viewmodel.SongList = new MultiSelectList(songs, "Id", "Title");

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", viewmodel.Album.ArtistId);
            return View(viewmodel);
        }

        // GET: Albums/Edit/5
        /*
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", album.ArtistId);
            return View(album);
        }
        */

        /*
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = _context.Album.Where(m => m.Id == id).Include(m => m.Genres).First();

            if (album == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            var songs = _context.Song.AsEnumerable();
            songs = songs.OrderBy(s => s.Title);

            AlbumGenresViewModel viewmodel = new AlbumGenresViewModel
            {
                Album = album,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = (IEnumerable<int>)album.Genres.Select(sa => sa.GenreId.Value).ToList(),
                SongList = new MultiSelectList(songs, "Id", "Title"),
                SelectedSongs = (IEnumerable<int>)album.Songs.Select(s=>s.Id).ToList()
            };

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", album.ArtistId);
            return View(viewmodel);
        }

        */
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = _context.Album.Where(m => m.Id == id).Include(m => m.Genres).FirstOrDefault();

            if (album == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.OrderBy(s => s.GenreName).ToList();
            var songs = _context.Song.OrderBy(s => s.Title).ToList();

            AlbumGenresViewModel viewmodel = new AlbumGenresViewModel
            {
                Album = album,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = album.Genres?.Select(sa => sa.GenreId.Value),
                SongList = new MultiSelectList(songs, "Id", "Title"),
                SelectedSongs = album.Songs?.Select(s => s.Id)
            };

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", album.ArtistId);
            return View(viewmodel);
        }
        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Released,Image,Label,ArtistId")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", album.ArtistId);
            return View(album);
        }
        */

        public async Task<IActionResult> Edit(int id, AlbumGenresViewModel viewmodel, IFormFile? file)
        {
            if (id != viewmodel.Album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Album);
                    var existingSongs = _context.Song.Where(s => s.AlbumId == id);
                    foreach (var song in existingSongs)
                    {
                        song.AlbumId = null;
                    }

                    if (viewmodel.SelectedSongs != null)
                    {
                        foreach (int songId in viewmodel.SelectedSongs)
                        {
                            var song = _context.Song.FirstOrDefault(s => s.Id == songId);
                            if (song != null)
                            {
                                song.AlbumId = id;
                            }
                        }
                    }

                    var existingGenres = _context.AlbumGenre.Where(ag => ag.AlbumId == id);
                    _context.AlbumGenre.RemoveRange(existingGenres);
                    if (viewmodel.SelectedGenres != null)
                    {
                        foreach (int genreId in viewmodel.SelectedGenres)
                        {
                            _context.AlbumGenre.Add(new AlbumGenre { AlbumId = id, GenreId = genreId });
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
                        viewmodel.Album.Image = slika_pateka;
                        _context.Update(viewmodel.Album);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(viewmodel.Album.Id))
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

            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "FullName", viewmodel.Album.ArtistId);
            return View(viewmodel);
        }
        // GET: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Album == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .Include(a => a.Genres).ThenInclude(a => a.Genre)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        /*
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Album == null)
            {
                return Problem("Entity set 'RecordStoreContext.Album'  is null.");
            }
            var album = await _context.Album.FindAsync(id);
            if (album != null)
            {
                _context.Album.Remove(album);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.Album.Include(a => a.Songs)
                                            .Include(a => a.Genres)
                                            .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            foreach (var song in album.Songs)
            {
                song.AlbumId = null;
            }

            foreach (var genre in album.Genres.ToList())
            {
                album.Genres.Remove(genre);
            }

            _context.Album.Remove(album);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
          return (_context.Album?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
