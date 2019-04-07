using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PennState.Models;
namespace PennState.ViewModels
{
    public class EditItemViewModel
    {
        public EditItemViewModel()
        {
            this.Item = new Item();
            this.Files = new Files();
            this.Photos = new Photos();
        }

        [DisplayName("Upload Files")]
        [FileType("pdf,doc,docx,xlsx,xlsm,xls,txt,rtf")]
        public HttpPostedFileBase[] FileUpload { get; set; }

        [DisplayName("Location Photos")]
        [FileType("jpg,jpeg,gif,png,pjpeg,x-png")] 
        public HttpPostedFileBase[] PhotoUpload { get; set; }

        public Item Item { get; set; }
        public Files Files { get; set; }
        public Photos Photos { get; set; }

    }
}