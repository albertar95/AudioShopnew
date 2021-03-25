using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AudioShopFrontend.DTO
{
    public class Category_BrandDTO
    {
        public System.Guid NidBrand { get; set; }
        public string BrandName { get; set; }
        public Nullable<int> NidCategory { get; set; }
    }
}