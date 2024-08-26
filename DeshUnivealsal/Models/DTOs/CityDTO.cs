using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DeshUnivealsal.Models.DTOs
{
    public class CityDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Latitude is required.")]
        [Range(-90.0000, 90.0000, ErrorMessage = "Latitude must be between -90.0000 and 90.0000.")]
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lat { get; set; }
        [Required(ErrorMessage = "Longitude is required.")]
        [Range(-180.0000, 180.0000, ErrorMessage = "Longitude must be between -180.0000 and 180.0000.")]
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lon { get; set; }

        [Display(Name = "Country Name")]

        public int CountryId { get; set; }

        public string CountryName { get; set; }
        
    }
}