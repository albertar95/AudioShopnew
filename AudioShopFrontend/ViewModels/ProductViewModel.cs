using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.ViewModels
{
    public class ProductViewModel
    {
        public List<ProductDTO> Products { get; set; }
        public int PageNumber { get; set; }
    }
}