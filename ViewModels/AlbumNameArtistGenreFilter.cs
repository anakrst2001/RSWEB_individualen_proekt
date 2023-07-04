using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Models;
using System.Collections.Generic;

namespace RecordStore.ViewModels
{
    public class AlbumNameArtistGenreFilter
    {
        public IList<Album> Albumi { get; set; }
        public SelectList Genres { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
    }
}
