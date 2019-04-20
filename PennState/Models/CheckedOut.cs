using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PennState.Models
{
    public class CheckedOut
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UserId { get; set; }
        public int? ItemId { get; set; }
        public System.DateTime? CheckOutDate { get; set; }
        public System.DateTime? CheckInDate { get; set; }
    }
}