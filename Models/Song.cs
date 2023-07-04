using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace RecordStore.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Year Released")]
        public int? YearReleased { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public ICollection<SongGenre> Genres { get; set; }

        [Display(Name = "Artist")]
        public int? ArtistId { get; set; }
        public Artist? Artist { get; set; }

        [Display(Name = "Album")]
        public int? AlbumId { get; set; }
        public Album? Album { get; set; }
    }
}
