using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class CatagoryVendor
    {
        [Key]
        public int Id { get; set; }

        public string VendorName { get; set; }

        public int? Pid { get; set; }

        public bool HasChildren { get; set; }

        [ForeignKey("Pid")]
        public virtual CatagoryVendor Parent { get; set; }
        public virtual ICollection<CatagoryVendor> Childs { get; set; }
    }
}