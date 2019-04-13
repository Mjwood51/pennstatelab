using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    [Table("Tbl_Locations")]
    public class Location
    {
        public Location()
        {
            this.Items = new HashSet<Item>();
            this.SubLocations = new HashSet<SubLocation>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter a Location")]
        [DisplayName("Location")]
        public string LocationName { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<SubLocation> SubLocations { get; set; }
    }

    [Table("Tbl_SubLocations")]
    public class SubLocation
    {
        public SubLocation()
        {
            this.Items = new HashSet<Item>();

        }
        [Key]
        public int Id { get; set; }
        [DisplayName("Sublocation")]
        public string SubLocationName { get; set; }
        public int? LocId { get; set; }

        [ForeignKey("LocId")]
        public virtual Location Location { get; set; }
        public virtual ICollection<Item> Items { get; set; }

    }
}