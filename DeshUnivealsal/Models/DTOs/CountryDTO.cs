using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DeshUnivealsal.Models.DTOs
{
    public class CountryDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name code is required.")]


        public string Name { get; set; }
        [Required(ErrorMessage = "ISO2 code is required.")]
        [StringLength(2, ErrorMessage = "ISO2 code must be 2 characters long.")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "ISO2 code must consist of 2 uppercase letters.")]
        public string ISO2 { get; set; }


        [Required(ErrorMessage = "ISO3 code is required.")]
        [StringLength(3, ErrorMessage = "ISO3 code must be 3 characters long.")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "ISO3 code must consist of 3 uppercase letters.")]
        public string ISO3 { get; set; }


        public int TotCities { get; set; }
    }

}