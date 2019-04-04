using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PennState.Models;

namespace PennState.ViewModels
{
    public class AddUserViewModel
    {
        public User User { get; set; }
        public IEnumerable<Role> RoleTypes { get; set; }
    }
}