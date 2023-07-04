using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Models;
using System.Collections.Generic;

namespace RecordStore.ViewModels
{
    public class SongTitleArtistAlbumGenre
    {
        public IList<Song> Songs { get; set; }
        public SelectList Genres { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }

        public string Album { get; set; }
        public string Genre { get; set; }
    }
}
