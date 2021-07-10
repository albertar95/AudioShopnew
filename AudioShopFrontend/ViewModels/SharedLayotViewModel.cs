using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.DTO;
using AudioShopFrontend.Services;
using System.Web.Security;
using AudioShopFrontend.Models;

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
        public static List<Cart> GetCurrentCartItems()
        {
            List<Cart> result = new List<Cart>();
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                var ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                result = dataTransfer.GetAllCartByNidUser(Guid.Parse(niduser));
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
        public static bool CheckUserLogin()
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                Services.DataTransfer dataTransfer = new Services.DataTransfer();
                var ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                var tmpUser = dataTransfer.GetUserByNidUser(Guid.Parse(niduser));
                if (tmpUser != null)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        public static string GetImageUrl(string pictureUrl,int type,int size)
        {
            try
            {
                if(type == 1)
                {
                    switch (size)
                    {
                        case 1:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Product/Big/{3}"//600x600
                             , HttpContext.Current.Request.Url.Scheme
                             , HttpContext.Current.Request.Url.Host
                             , HttpContext.Current.Request.Url.Port
                             , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        case 2:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Product/Normal/{3}"//255x165
                            , HttpContext.Current.Request.Url.Scheme
                            , HttpContext.Current.Request.Url.Host
                            , HttpContext.Current.Request.Url.Port
                            , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));

                        case 3:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Product/Medium/{3}"//540x540
                            , HttpContext.Current.Request.Url.Scheme
                            , HttpContext.Current.Request.Url.Host
                            , HttpContext.Current.Request.Url.Port
                            , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        case 4:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Product/Small/{3}"//70x70
                            , HttpContext.Current.Request.Url.Scheme
                            , HttpContext.Current.Request.Url.Host
                            , HttpContext.Current.Request.Url.Port
                            , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        default:
                            return pictureUrl.Split(',').FirstOrDefault();
                    }
                }else if(type == 2)
                {
                    switch (size)
                    {
                        case 1:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Banner/Big/{3}"//870x175
                             , HttpContext.Current.Request.Url.Scheme
                             , HttpContext.Current.Request.Url.Host
                             , HttpContext.Current.Request.Url.Port
                             , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        case 2:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Banner/Normal/{3}"//720x200
                            , HttpContext.Current.Request.Url.Scheme
                            , HttpContext.Current.Request.Url.Host
                            , HttpContext.Current.Request.Url.Port
                            , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        case 3:
                            return string.Format("{0}://admin.{1}:{2}/Uploads/Banner/Sidbar/{3}"//270x345
                            , HttpContext.Current.Request.Url.Scheme
                            , HttpContext.Current.Request.Url.Host
                            , HttpContext.Current.Request.Url.Port
                            , pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Replace(pictureUrl.Split(',').FirstOrDefault().Split('/').LastOrDefault().Split('.').LastOrDefault(), "webp"));
                        default:
                            return pictureUrl.Split(',').FirstOrDefault();
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }


    }

}