//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PennState.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Requests
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string Message { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string ItemName { get; set; }
    
        public virtual Tbl_Items Tbl_Items { get; set; }
        public virtual Tbl_Users Tbl_Users { get; set; }
    }
}
