using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    [Table("Tbl_Roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [StringLength(10)]
        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}