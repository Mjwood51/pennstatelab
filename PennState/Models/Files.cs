using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class Files
    {
        public Files()
        {
            FileList = null;
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