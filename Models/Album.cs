using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace RecordStore.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Released { get; set; }
        public string? Image { get; set; }

        [StringLength(50)]
        public string? Label { get; set; }

        public ICollection<AlbumGenre>? Genres { get; set; }

        [Display(Name = "Artist")]
        public int? ArtistId { get; set; }
        public Artist? Artist { get; set; }
        public ICollection<Song>? Songs { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserAlbum>? UserAlbums { get; set; }
    }
}
