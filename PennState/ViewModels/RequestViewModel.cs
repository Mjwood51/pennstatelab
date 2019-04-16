using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PennState.Models;

namespace PennState.ViewModels
{
    public class RequestViewModel
    {
        public IEnumerable<Requests> Requests { get; set; }
    }
}