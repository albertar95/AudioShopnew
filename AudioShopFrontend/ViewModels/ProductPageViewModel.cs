using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.ViewModels
{
    public class ProductPageViewModel
    {
        public Product Product { get; set; }
        public List<ProductDTO> Similars { get; set; }
    }
}