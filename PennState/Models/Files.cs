using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    [Table("Tbl_File")]
    public class Files
    {
        public Files()
        {
            FileList = null;
            this.Items = new HashSet<Item>();
        }
        [Key]
        public int Id { get; set; }
        public string ItemFileName { get; set; }
        public byte[] DataStream { get; set; }

        public List<Files> GetFileList { get; set; }
        public string[] FileList { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}