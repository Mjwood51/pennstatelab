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
    
    public partial class Tbl_CatagoryOwners
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_CatagoryOwners()
        {
            this.Tbl_CatagoryOwners1 = new HashSet<Tbl_CatagoryOwners>();
        }
    
        public int Id { get; set; }
        public string OwnerName { get; set; }
        public Nullable<int> Pid { get; set; }
        public bool HasChildren { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CatagoryOwners> Tbl_CatagoryOwners1 { get; set; }
        public virtual Tbl_CatagoryOwners Tbl_CatagoryOwners2 { get; set; }
    }
}
