using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    [Table("Tbl_Photo")]
    public class Photos
    {
        public Photos()
        {
            PhotoList = null;
            this.Items = new HashSet<Item>();
        }
        [Key]
        public int Id { get; set; }
        public string PhotoName { get; set; }
        public byte[] DataStream { get; set; }
        public int? SubId { get; set; }

        public List<Photos> GetPhotoList { get; set; }
        public string[] PhotoList { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}