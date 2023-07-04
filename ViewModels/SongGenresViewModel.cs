using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Models;
using System.Collections.Generic;

namespace RecordStore.ViewModels
{
    public class SongGenresViewModel
    {
        public Song Song { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
