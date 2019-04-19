using PennState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PennState.ViewModels
{
    public class ItemDetailsModel
    {
        public User User { get; set; } 
        public Item Item { get; set; }
    }
}