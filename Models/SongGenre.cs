using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecordStore.Models
{
    public class SongGenre
    {
        public int Id { get; set; }

        public int? SongId { get; set; }
        public Song? Song { get; set; }

        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
