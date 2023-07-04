using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace RecordStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name ="App User")]
        [StringLength(200)]
        public string AppUser { get; set; }

        [StringLength(200)]
        public string Comment { get; set; }
        public int? Rating { get; set; }

        [Display(Name = "Album")]
        public int? AlbumId { get; set; }
        public Album? Album { get; set; }
    }
}
