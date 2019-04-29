using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PennState.Models;

namespace PennState.ViewModels
{
    public class ItemViewModel
    {
        public Files Files { get; set; }
        public Photos Photos { get; set; }
        public Item Item { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<CatagoryLocation> LocationsC { get; set; }
        public IEnumerable<CatagoryVendor> Vendors { get; set; }
        public IEnumerable<CatagoryOwner> Owners { get; set; }
        public IEnumerable<CatagoryType> Types { get; set; }
    }
}