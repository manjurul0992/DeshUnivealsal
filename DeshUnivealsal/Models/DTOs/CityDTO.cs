using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeshUnivealsal.Models.DTOs
{
    public class CityDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public decimal Lat { get; set; }

        public decimal Lon { get; set; }
        [Display(Name = "Country Name")]

        public int CountryId { get; set; }

        public string CountryName { get; set; }
        
    }
}