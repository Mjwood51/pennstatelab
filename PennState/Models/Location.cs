using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter a Location")]
        [DisplayName("Location")]
        public string LocationName { get; set; }

    }

    public class SubLocation
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter a Sublocation")]
        [DisplayName("Sublocation")]
        public string SubLocationName { get; set; }
        public int? LocId { get; set; }

        [ForeignKey("LocId")]
        public virtual Location Location { get; set; }

    }
}