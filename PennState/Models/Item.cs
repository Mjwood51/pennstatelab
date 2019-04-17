using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    [Table("Tbl_Items")]
    [JsonObject(IsReference = true)]
    public class Item
    {
        public Item()
        {
            this.Photos = new HashSet<Photos>();
            this.Files = new HashSet<Files>();
            this.Requests = new HashSet<Requests>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("Item Name")]
        public string ItemName { get; set; }

        [Required]
        [DisplayName("Unit Amount")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [RegularExpression(@"[0-9]{1,}", ErrorMessage = "Please enter a whole number")]
        public int AmountInStock { get; set; }

        [DisplayName("Location Comments")]
        [StringLength(255, MinimumLength = 1)]
        public string LocationComments { get; set; }

        [StringLength(50)]
        public string Manufacturer { get; set; }

        [StringLength(25)]
        [DisplayName("Catalog #")]
        public string CatalogNumber { get; set; }

        [StringLength(350)]
        [DisplayName("URL Address")]
        public string WebAddress { get; set; }

        [StringLength(50)]
        public string Vendor { get; set; }

        [StringLength(200)]
        [DisplayName("Contact Information")]
        public string ContactInfo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Purchase Date")]
        public DateTime? PurchaseDate { get; set; }

        public DateTime? Added { get; set; }

        public DateTime? Updated { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Please enter valid price")]
        [DisplayName("Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [DisplayName("Request Comments")]
        [StringLength(150)]
        public string Flagged { get; set; }

        [Required]
        [DisplayName("Item Type")]
        [StringLength(50, MinimumLength = 1)]
        public string ItemType { get; set; }

        [StringLength(255)]
        [DisplayName("Item Notes")]
        public string ItemNotes { get; set; }

        [StringLength(255)]
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }


        public virtual ICollection<Photos> Photos { get; set; }
        public virtual ICollection<Files> Files { get; set; }
        public virtual ICollection<Requests> Requests { get; set; }

        public int UsrId { get; set; }

        public int? LocId { get; set; }

        public int? SubId { get; set; }

        [ForeignKey("LocId")]
        public virtual Location Location { get; set; }

        [ForeignKey("SubId")]
        public virtual SubLocation SubLocation { get; set; }

        [ForeignKey("UsrId")]
        public virtual User User { get; set; }

    }
}