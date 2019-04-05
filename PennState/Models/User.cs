using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{

    [JsonObject(IsReference = true)]
    public class User
    {
        public User()
        {
            this.Items = new HashSet<Item>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter an Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email is not valid")]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [StringLength(60)]
        public string PasswordHashed { get; set; }

        public bool IsActive { get; set; }
        public Guid ActivationCode { get; set; }
        [Required(ErrorMessage = "Please select a Role Type")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Roles { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}