using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeshUnivealsal.Models.DTOs
{
    public class CountryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ISO2 { get; set; }

        public string ISO3 { get; set; }

        public int? TotCities { get; set; }
    }

}