using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PennState.Models;
namespace PennState.ViewModels
{
    public class FlagItemViewModel
    {
        public Item TheItem { get; set; }
        public Requests TheRequest { get; set; }
    }
}