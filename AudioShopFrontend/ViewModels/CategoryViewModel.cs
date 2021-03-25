using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryDTO Category { get; set; }
        public List<Category_BrandDTO> CategoryBrands { get; set; }
        public List<Category_TypeDTO> CategoryTypes { get; set; }
        public List<ProductDTO> Products { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}