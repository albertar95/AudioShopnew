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
    
    public partial class Category_Brands
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category_Brands()
        {
            this.Products = new HashSet<Product>();
        }
    
        public System.Guid NidBrand { get; set; }
        public string BrandName { get; set; }
        public Nullable<int> NidCategory { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
