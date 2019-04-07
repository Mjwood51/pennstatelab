using PennState.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PennState.ViewModels
{
    public class AddItemViewModel
    {
        public AddItemViewModel()
        {
            this.Item = new Item();
            this.Files = new Files();
            this.Photos = new Photos();
        }
        [DisplayName("Upload Files")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.doc|.DOC|.pdf|.PDF|.docx|.DOCX|.rtf|.RTF|.xlsx|.XLSX|.xlsm|.XLSM|.xls|.XLS|.txt|.TXT)$", ErrorMessage = "Please upload a valid file type (.pdf, .doc, .docx, .xlsx, .xlsm, .xls, .txt, .rtf)")]
        [FileType("pdf,doc,docx,xlsx,xlsm,xls,txt,rtf")]
        public HttpPostedFileBase[] FileUpload { get; set; }

        [DisplayName("Location Photos")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.jpg|.JPG|.gif|.GIF|.png|.PNG|.jpeg|.JPEG|.pjpeg|.PJPEG|.x-png|.X-PNG)$", ErrorMessage = "Please upload a valid image type (.jpg, jpeg, .gif, .png, pjpeg, x-png)")]
        [FileType("jpg,jpeg,gif,png,pjpeg,x-png")]
        public HttpPostedFileBase[] PhotoUpload { get; set; }

        public Item Item { get; set; }
        public Files Files { get; set; }
        public Photos Photos { get; set; }
    }
}