//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudioShopBackend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Carts = new HashSet<Cart>();
            this.Favorites = new HashSet<Favorite>();
            this.Orders = new HashSet<Order>();
        }
    
        public System.Guid NidProduct { get; set; }
        public string ProductName { get; set; }
        public int NidCategory { get; set; }
        public System.Guid NidBrand { get; set; }
        public System.Guid NidType { get; set; }
        public string Pictures { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public decimal Price { get; set; }
        public byte State { get; set; }
        public Nullable<byte> Priority { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Category Category { get; set; }
        public virtual Category_Brands Category_Brands { get; set; }
        public virtual Category_Types Category_Types { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favorite> Favorites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
