using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AudioShopFrontend.DTO
{
    public class Category_TypeDTO
    {
        public System.Guid NidType { get; set; }
        public string TypeName { get; set; }
        public int NidCategory { get; set; }
    }
}