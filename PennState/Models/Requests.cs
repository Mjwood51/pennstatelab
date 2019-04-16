using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class Requests
    {
        public enum StatusEnum
        {
            Reorder, Broken, Repair, Missing, Other
        }

        [Key]
        public int Id { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        [DisplayName("Order Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        [DisplayName("New Request")]
        public string Message { get; set; }
        public int? UserId { get; set; }
        public int? ItemId { get; set; }

        [DisplayName("Total Price")]
        public string TotalPrice { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [DisplayName("Item Status")]
        public StatusEnum StatEnum { get; set; }
        
    }
}