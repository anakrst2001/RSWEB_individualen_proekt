using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecordStore.Models
{
    public class AlbumGenre
    {
        public int Id { get; set; }

        public int? AlbumId { get; set; }
        public Album? Album { get; set; }

        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
