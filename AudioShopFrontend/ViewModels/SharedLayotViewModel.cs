﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;
using AudioShopFrontend.Services;
using System.Web.Security;

namespace AudioShopFrontend.ViewModels
{
    public class SharedLayotViewModel
    {
        public static int GetUserFavoritesCount()
        {
            try
            {
                if(HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin") && HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                {
                    return int.Parse(HttpContext.Current.Request.Cookies["AudioShopFavorites"].Value);
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }
        public int GetUserCartCount()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin") && HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    return int.Parse(HttpContext.Current.Request.Cookies["AudioShopCart"].Value);
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }
        public static List<ProductDTO> GetCurrentCartItems()
        {
            List<ProductDTO> result = new List<ProductDTO>();
            if(HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                var ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                foreach (var cart in dataTransfer.GetAllCartByNidUser(Guid.Parse(niduser)))
                {
                    result.Add(dataTransfer.GetProductDtoByID(cart.NidProduct));
                }
            }
            return result;
        }
        public static string GetUserId()
        {
            List<ProductDTO> result = new List<ProductDTO>();
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                var ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies["AudioShopLogin"].Value);
                return ticket.UserData.Split(',').First();
            }
            return "";
        }
        public static List<CategoryLiteDTO> GetCategories()
        {
            try
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                return dataTransfer.GetcategoryList();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}