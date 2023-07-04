using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Models;
using System.Collections.Generic;

namespace RecordStore.ViewModels
{
    public class ArtistNameSurnameNationalityFilter
    {
        public IList<Artist> Artists { get; set; }
        public SelectList Nationalities { get; set; }
        public string ArtistNationality { get; set; }
        public string ArtistFirstName { get; set; }
        public string ArtistLastName { get; set;}
    }
}
