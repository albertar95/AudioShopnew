//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudioShopFrontend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cart
    {
        public System.Guid NidCart { get; set; }
        public System.Guid NidUser { get; set; }
        public System.Guid NidProduct { get; set; }
        public Nullable<System.Guid> NidOrder { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public byte State { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
