using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AudioShopFrontend.DTO
{
    public class CategoryDTO
    {
        public int NidCategory { get; set; }
        public string CategoryName { get; set; }
        public string Pictures { get; set; }
        public string Description { get; set; }
        public string keywords { get; set; }
        public Nullable<bool> IsSubmmited { get; set; }
    }
}