using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;
using AudioShopFrontend.Models;

namespace AudioShopFrontend.ViewModels
{
    public class CartViewModel
    {
        public List<ProductDTO> Products { get; set; }
        public List<Cart> Carts { get; set; }
    }
}