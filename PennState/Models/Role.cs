using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [StringLength(10)]
        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}