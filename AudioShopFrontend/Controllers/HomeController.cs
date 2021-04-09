using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AudioShopFrontend.Services;
using AudioShopFrontend.DTO;
using AudioShopFrontend.ViewModels;
using AudioShopFrontend.Models;
using System.Web.Security;
using System.IO;

namespace AudioShopFrontend.Controllers
{
    public class HomeController : Controller
    {
        DataTransfer dataTransfer = null;
        // GET: Home
        public ActionResult Index()//done
        {
            IndexViewModel ivm = new IndexViewModel();
            dataTransfer = new DataTransfer();
            ivm.Categories = dataTransfer.GetcategoryList();
            ivm.LatestProducts = dataTransfer.GetLatestProducts();
            ivm.PopularProducts = dataTransfer.GetPopularProducts();
            ivm.SpecialProducts = dataTransfer.GetSpecialProducts();
            //discounts
            //blogs
            return View(ivm);
        }
        public ActionResult Category(int Nidcategory)//done
        {
            dataTransfer = new DataTransfer();
            ViewModels.CategoryViewModel cvm = new ViewModels.CategoryViewModel();
            var tmpCategory = dataTransfer.GetCategoryByNidCategory(Nidcategory);
            cvm.CategoryBrands = dataTransfer.GetCategory_BrandByNidcategory(tmpCategory.NidCategory);
            cvm.CategoryTypes = dataTransfer.GetCategory_TypeByNidcategory(tmpCategory.NidCategory);
            cvm.Products = dataTransfer.GetProductsByNidcategory(tmpCategory.NidCategory);
            var minmax = dataTransfer.GetMinMaxCategoryPrice(tmpCategory.NidCategory);
            cvm.MinPrice = minmax.Item1;
            cvm.MaxPrice = minmax.Item2;
            cvm.Category = tmpCategory;
            return View(cvm);
        }
        public ActionResult Generals(int Typo)//done
        {
            List<ProductDTO> Products = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            TempData["GeneralPageName"] = Typo.ToString();
            switch (Typo)
            {
                case 1:
                    Products = dataTransfer.GetLatestProducts();
                    break;
                case 2:
                    Products = dataTransfer.GetSpecialProducts();
                    break;
                case 3:
                    Products = dataTransfer.GetPopularProducts();
                    break;
                default:
                    break;
            }
            return View(Products);
        }
        public ActionResult Product(Guid NidProduct)//done
        {
            dataTransfer = new DataTransfer();
            ProductPageViewModel ppvm = new ProductPageViewModel();
            ppvm.Product = dataTransfer.GetProductByID(NidProduct);
            ppvm.Similars = dataTransfer.GetSimilarProducts(NidProduct);
            return View(ppvm);
        }
        public ActionResult Register()//done
        {
            return View();
        }
        public ActionResult Login()//done
        {
            return View();
        }
        public ActionResult SubmitLogin(string Username,string Password,bool isPersistant)//done
        {
            dataTransfer = new DataTransfer();
            var User = dataTransfer.GetUserByUsername(Username);
            if(User != null)
            {
                if (DataTransfer.Encrypt(Password) == User.Password)
                {
                    FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, User.Username, DateTime.Now, DateTime.Now.AddMinutes(30), isPersistant, User.NidUser.ToString() + "," + User.FirstName + " " + User.LastName, FormsAuthentication.FormsCookiePath);
                    string encTicket = FormsAuthentication.Encrypt(Ticket);
                    Response.Cookies.Add(new HttpCookie("AudioShopLogin", encTicket));
                    //cartcookie
                    var cartandfav = dataTransfer.GetCartAndFavoriteCount(User.NidUser);
                    if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                    {
                        Response.Cookies["AudioShopCart"].Value = cartandfav.Item1.ToString();
                    }
                    else
                    {
                        HttpCookie newCookie = new HttpCookie("AudioShopCart", cartandfav.Item1.ToString());
                        Response.Cookies.Add(newCookie);
                    }
                    //favcookie
                    if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                    {
                        Response.Cookies["AudioShopFavorites"].Value = cartandfav.Item2.ToString();
                    }
                    else
                    {
                        HttpCookie newCookie = new HttpCookie("AudioShopFavorites", cartandfav.Item2.ToString());
                        Response.Cookies.Add(newCookie);
                    }
                    //expiries
                    if (isPersistant)
                    {
                        Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddDays(7);
                        Response.Cookies["AudioShopCart"].Expires = DateTime.Now.AddDays(7);
                        Response.Cookies["AudioShopFavorites"].Expires = DateTime.Now.AddDays(7);
                    }
                    else
                    {
                        Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddHours(4);
                        Response.Cookies["AudioShopCart"].Expires = DateTime.Now.AddHours(4);
                        Response.Cookies["AudioShopFavorites"].Expires = DateTime.Now.AddHours(4);
                    }
                    //Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddMinutes(30);
                    //Response.Cookies["AudioShopLogin"].HttpOnly = true;
                    //Response.Cookies["AudioShopLogin"].Secure = true;
                    return Json(new JsonResults() { HasValue = true });
                }
                else
                    return Json(new JsonResults() { HasValue = false, Message = "رمز عبور اشتباه می باشد" });
            }else
                return Json(new JsonResults() { HasValue = false,Message = "نام کاربری موجود نمی باشد" });
        }
        public ActionResult SubmitRegister(User User)//done
        {
            dataTransfer = new DataTransfer();
            if(!dataTransfer.CheckUsername(User.Username))
            {
                User.CreateDate = DateTime.Now;
                User.Enabled = true;
                User.IsAdmin = false;
                User.NidUser = Guid.NewGuid();
                User.Password = DataTransfer.Encrypt(User.Password);
                if(dataTransfer.AddUser(User))
                    return RedirectToAction("Index");
                else
                {
                    TempData["RegisterError"] = "error in database";
                    return View("Register");
                }
            }
            else
            {
                TempData["RegisterError"] = "username already exists";
                return RedirectToAction("Register");
            }
            
        }
        public ActionResult Logout()//done
        {
            Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddYears(-1);
            Response.Cookies["AudioShopCart"].Expires = DateTime.Now.AddYears(-1);
            Response.Cookies["AudioShopFavorites"].Expires = DateTime.Now.AddYears(-1);
            return RedirectToAction("Index");
        }
        public ActionResult MyCart()//done
        {
            List<Cart> carts = new List<Cart>();
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                carts = dataTransfer.GetAllCartByNidUser(Guid.Parse(niduser));
            }
            return View(carts);
        }
        public ActionResult CartQuantityChanged(Guid NidCart,int Quantity)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
            
                dataTransfer = new DataTransfer();
            int tmpresult = dataTransfer.UpdateCartQuantity(NidCart,Quantity);
            if (tmpresult != 0)
            return Json(new JsonResults() { HasValue = true, tmpNidCategory = tmpresult, Message = dataTransfer.CartPriceAggregateByNidUser(Guid.Parse(niduser)).ToString() });
            else
                return Json(new JsonResults() { HasValue = false });
            }else
                return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult MyFavorites()
        {
            List<ProductDTO> prods = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                //if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                //{
                //    string[] fav = Request.Cookies["AudioShopFavorites"].Value.Split(',');
                //    foreach (var f in fav)
                //    {
                //        prods.Add(dataTransfer.GetProductDtoByID(Guid.Parse(f)));
                //    }
                //}
                foreach (var fav in dataTransfer.GetAllFavoritesByNidUser(Guid.Parse(niduser)))
                {
                    prods.Add(dataTransfer.GetProductDtoByID(fav.NidProduct));
                }
            }
            return View(prods);
        }
        public ActionResult AddProductToFavorites(Guid NidProduct)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                dataTransfer = new DataTransfer();
                int favs = dataTransfer.AddFavorite(new Favorite() { NidFav = Guid.NewGuid(), CreateDate = DateTime.Now, NidProduct = NidProduct, NidUser = Guid.Parse(niduser) });
                if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                {
                    Response.Cookies["AudioShopFavorites"].Value = favs.ToString();
                }
                else
                {
                    HttpCookie newCookie = new HttpCookie("AudioShopFavorites", favs.ToString());
                    Response.Cookies.Add(newCookie);
                }
                return Json(new JsonResults() { HasValue = true, Html = favs.ToString() });
            }
            else
            return Json(new JsonResults() {  HasValue = false});
        }
        public ActionResult AddProductToCart(Guid NidProduct,int Quantity)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                dataTransfer = new DataTransfer();
                int carts = dataTransfer.AddCart(new Cart() {  CreateDate = DateTime.Now, NidCart = Guid.NewGuid(), NidProduct = NidProduct, NidUser = Guid.Parse(niduser), Quantity = Quantity});
                if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    Response.Cookies["AudioShopCart"].Value = carts.ToString();
                }
                else
                {
                    HttpCookie newCookie = new HttpCookie("AudioShopCart", carts.ToString());
                    Response.Cookies.Add(newCookie);
                }
                return Json(new JsonResults() { HasValue = true, tmpNidCategory = carts, Html = RenderViewToString(this.ControllerContext, "_CartPopup", null) });
            }
            return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult RemoveProductFromCart(Guid NidProduct)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                dataTransfer = new DataTransfer();
                var tmpcart = dataTransfer.GetCartByNidUserAndProduct(Guid.Parse(niduser),NidProduct);
                if(tmpcart != null)
                {
                    int carts = dataTransfer.RemoveCart(tmpcart);
                    if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                    {
                        Response.Cookies["AudioShopCart"].Value = carts.ToString();
                    }
                    else
                    {
                        HttpCookie newCookie = new HttpCookie("AudioShopCart", carts.ToString());
                        Response.Cookies.Add(newCookie);
                    }
                    return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CartPopup",null), tmpNidCategory = carts, Message = dataTransfer.CartPriceAggregateByNidUser(Guid.Parse(niduser)).ToString() });
                }
            }
            return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult MyProfile()
        {
            ProfileViewModel pvm = new ProfileViewModel();
            List<Order> Orders = new List<Order>();
            Models.User User = new User();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData.Split(',').First();
                User = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                Orders = dataTransfer.GetUsersOrder(Guid.Parse(NidUser));
            }
            pvm.Orders = Orders;
            pvm.UserInfo = User;
            return View(pvm);
        }
        public ActionResult ChangePassword(string CurrentPassword,string NewPassword)
        {
            string message = "";
            bool IsUpdated = false;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData.Split(',').First();
                var CurrentUser = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                if (CurrentUser != null)
                {
                    if (CurrentUser.Password == DataTransfer.Encrypt(CurrentPassword))
                    {
                        CurrentUser.Password = DataTransfer.Encrypt(NewPassword);
                        if (dataTransfer.UpdateUser(CurrentUser))
                        {
                            message = "password updated successfully";
                            IsUpdated = true;
                        }
                        else
                            message = "error in database";
                    }
                    else
                        message = "current password doesnt meet!";
                }
                else
                    message = "user not found";
            }
            else
                message = "user not logined";

            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult ChangeAddress(string NewAddress)
        {
            string message = "";
            bool IsUpdated = false;
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData.Split(',').First();
                var CurrentUser = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                if (CurrentUser != null)
                {
                    CurrentUser.Address = NewAddress;
                    if (dataTransfer.UpdateUser(CurrentUser))
                    {
                        message = "done";
                        IsUpdated = true;
                    }
                }
                else
                    message = "user not found";
            }
            else
                message = "user not logined";
            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }
        public ActionResult SubmitAddReview(string CommentText,string NidUser)//done
        {
            Comment tmpcomment = new Comment() {  CreateDate = DateTime.Now, NidComment = Guid.NewGuid(), State = 2};
            if(NidUser != "")
            {
                tmpcomment.NidUser = Guid.Parse(NidUser);
                tmpcomment.CommentText = CommentText;
                dataTransfer = new DataTransfer();
                if (dataTransfer.AddComment(tmpcomment))
                    return Json(new JsonResults() { HasValue = true, Message = "نظر شما با موفقیت ثبت شد" });
                else
                    return Json(new JsonResults() { HasValue = false, Message = "مشکل در سرور.لطفا مجدد امتحان کنید" });
            }
            else
            {
                return Json(new JsonResults() { HasValue = false, Message = "برای ثبت نظر می بایست ابتدا وارد شوید" });
            }
        }
        //categories() demo for categories
        //checkout() go to dargah
        public ActionResult Pagination(int id,int currentpage,int target,int Nidcategory,string FilterType = "",decimal MinPrice = 0,decimal MaxPrice = 0,string NidBrands = "",string NidTypes = "")//done
        {
            dataTransfer = new DataTransfer();
            switch (id)
            {
                case 1:
                    CategoryViewModel cvm = new CategoryViewModel();
                    if (FilterType == "")//no filter
                    {
                        cvm.Products = CategoryProductFilter("", Nidcategory, currentpage, target);
                    }
                    else
                    {
                        cvm.Products = CategoryProductFilter(FilterType, Nidcategory, currentpage, target, MinPrice, MaxPrice, NidBrands, NidTypes);
                    }
                    if (target == 0)
                        cvm.MinPrice = currentpage + 1;
                    else
                        cvm.MinPrice = currentpage - 1;
                    return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CategoryProducts", cvm) });
                case 2:
                    ProductViewModel pvm = new ProductViewModel();
                    switch (Nidcategory)
                    {
                        case 1:
                            pvm.Products = dataTransfer.GetLatestProducts(10,(currentpage + target)*10);
                            break;
                        case 2:
                            pvm.Products = dataTransfer.GetSpecialProducts(10,(currentpage + target) * 10);
                            break;
                        case 3:
                            pvm.Products = dataTransfer.GetPopularProducts(10, (currentpage + target) * 10);
                            break;
                    }

                    if (target == 0)
                        pvm.PageNumber = currentpage + 1;
                    else
                        pvm.PageNumber = currentpage - 1;
                    return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_GeneralProducts", pvm) });
                default:
                    return Json(new JsonResults() { HasValue = false });
            }
        }
        public List<ProductDTO> CategoryProductFilter(string FilterType,int Nidcategory, int currentpage, int target, decimal MinPrice = 0, decimal MaxPrice = 0, string NidBrands = "", string NidTypes = "")//done
        {
            if (FilterType == "")
            {
                return dataTransfer.GetProductsByNidcategory(Nidcategory, 10, (currentpage + target) * 10);
            }
            else
            {
                string[] filters = FilterType.Split(',');
                int FilterTypo = 0;
                if (filters.Contains("1"))
                {
                    if (filters.Contains("2"))
                    {
                        if (filters.Contains("3"))//all filters
                        {
                            FilterTypo = 1;
                        }
                        else //just 1,2
                        {
                            FilterTypo = 2;
                        }
                    }
                    else
                    {
                        if (filters.Contains("3"))//just 1,3
                        {
                            FilterTypo = 3;
                        }
                        else //just 1
                        {
                            FilterTypo = 4;
                        }
                    }
                }
                else
                {
                    if (filters.Contains("2"))
                    {
                        if (filters.Contains("3"))//just 2,3
                        {
                            FilterTypo = 5;
                        }
                        else //just 2
                        {
                            FilterTypo = 6;
                        }
                    }
                    else
                    {
                        if (filters.Contains("3"))//just 3
                        {
                            FilterTypo = 7;
                        }
                        else //none
                        {
                            FilterTypo = 0;
                        }
                    }
                }
                return dataTransfer.GetProductsByNidcategory(Nidcategory, 10, (currentpage + target) * 10, MinPrice, MaxPrice, NidBrands, NidTypes, FilterTypo);
            }
        }
        public ActionResult SortChange(int id,string NidProducts,int sortId)//done
        {
            dataTransfer = new DataTransfer();
            List<ProductDTO> products = new List<ProductDTO>();
            switch (id)
            {
                case 1:
                    CategoryViewModel cvm = new CategoryViewModel();
                    foreach (var nids in NidProducts.Split(','))
                    {
                        products.Add(dataTransfer.GetProductDtoByID(Guid.Parse(nids)));
                    }
                    switch (sortId)
                    {
                        case 1:
                            products = products.OrderBy(p => p.ProductName).ToList();
                            break;
                        case 2:
                            products = products.OrderBy(p => p.Price).ToList();
                            break;
                        case 3:
                            products = products.OrderByDescending(p => p.Price).ToList();
                            break;
                    }
                    cvm.Products = products;
                    return Json(new JsonResults() {  HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CategoryProductSort", cvm)});
                case 2:
                    foreach (var nids in NidProducts.Split(','))
                    {
                        products.Add(dataTransfer.GetProductDtoByID(Guid.Parse(nids)));
                    }
                    switch (sortId)
                    {
                        case 1:
                            products = products.OrderBy(p => p.ProductName).ToList();
                            break;
                        case 2:
                            products = products.OrderBy(p => p.Price).ToList();
                            break;
                        case 3:
                            products = products.OrderByDescending(p => p.Price).ToList();
                            break;
                    }
                    return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_GeneralProductSort", products) });
            }
            return Json(new JsonResults() { });
        }
        public ActionResult SearchThis(string Text,int Nidcategory)
        {
            dataTransfer = new DataTransfer();
            var res = dataTransfer.SearchProduct(Text, Nidcategory);
            if(res.Count != 0)
            {
                return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_SearchResult", res) });
            }
            else
            {
                return Json(new JsonResults() { HasValue = false});
            }
        }
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
    public class JsonResults
    {
        public bool HasValue { get; set; }
        public string Message { get; set; }
        public string Html { get; set; }
        public int tmpNidCategory { get; set; }
    }
}