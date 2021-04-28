using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;

namespace AudioShopFrontend.ViewModels
{
    public class CheckoutViewModel
    {
        public List<Cart> Carts { get; set; }
        public User User { get; set; }
        public Order Order { get; set; }
    }
}