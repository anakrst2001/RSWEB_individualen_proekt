using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Models;
using System.Collections.Generic;

namespace RecordStore.ViewModels
{
    public class AlbumGenresViewModel
    {
        public Album Album { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
        public IEnumerable<int>? SelectedSongs { get; set; }
        public IEnumerable<SelectListItem>? SongList { get; set; }
    }
}
