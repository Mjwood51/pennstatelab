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
    
    public partial class Tbl_CatagoryLocations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_CatagoryLocations()
        {
            this.Tbl_CatagoryLocations1 = new HashSet<Tbl_CatagoryLocations>();
        }
    
        public int Id { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> Pid { get; set; }
        public bool HasChildren { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_CatagoryLocations> Tbl_CatagoryLocations1 { get; set; }
        public virtual Tbl_CatagoryLocations Tbl_CatagoryLocations2 { get; set; }
    }
}
