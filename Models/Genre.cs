using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace RecordStore.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Genre")]
        [StringLength(50)]
        public string GenreName { get; set; }

        public ICollection<SongGenre> Songs { get; set; }

        public ICollection<AlbumGenre> Albums { get; set; }
    }
}
