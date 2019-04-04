using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class CatagoryType
    {
        [Key]
        public int Id { get; set; }

        //Cat Name  
        public string TypeName { get; set; }

        //represnts Parent ID and it's nullable  
        public int? Pid { get; set; }

        public bool HasChildren { get; set; }

        [ForeignKey("Pid")]
        public virtual CatagoryType Parent { get; set; }
        public virtual ICollection<CatagoryType> Childs { get; set; }
    }
}