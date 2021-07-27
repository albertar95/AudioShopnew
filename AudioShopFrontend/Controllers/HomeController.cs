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
        const string Merchant = "42022b3d-9a49-4bd8-9c2c-73ce0a5303f9";
        //demostration section
        //[OutputCache(Duration = 2600000)]
        public ActionResult Index()//done
        {
            IndexViewModel ivm = new IndexViewModel();
            dataTransfer = new DataTransfer();
            ivm.Categories = dataTransfer.GetcategoryList();
            ivm.LatestProducts = dataTransfer.GetLatestProducts();
            ivm.PopularProducts = dataTransfer.GetPopularProducts();
            ivm.SpecialProducts = dataTransfer.GetSpecialProducts();
            ivm.Settings = dataTransfer.GetAllSettings();
            //discounts
            //blogs
            return View(ivm);
        }
        //[OutputCache(Duration = 85000,VaryByParam ="Nid")]
        public ActionResult Category(int Nid, string Title = "")
        {
            dataTransfer = new DataTransfer();
            ViewModels.CategoryViewModel cvm = new ViewModels.CategoryViewModel();
            var tmpCategory = dataTransfer.GetCategoryByNidCategory(Nid);
            cvm.CategoryBrands = dataTransfer.GetCategory_BrandByNidcategory(tmpCategory.NidCategory);
            cvm.CategoryTypes = dataTransfer.GetCategory_TypeByNidcategory(tmpCategory.NidCategory);
            cvm.Products = dataTransfer.GetProductsByNidcategory(tmpCategory.NidCategory);
            var minmax = dataTransfer.GetMinMaxCategoryPrice(tmpCategory.NidCategory);
            cvm.MinPrice = minmax.Item1;
            cvm.MaxPrice = minmax.Item2;
            cvm.Category = tmpCategory;
            ViewBag.Title = Title;
            ViewBag.Desc = tmpCategory.Description;
            return View(cvm);
        }
        //[OutputCache(Duration = 85000, VaryByParam = "Nid")]
        public ActionResult Generals(int Nid, string Title = "")
        {
            List<ProductDTO> Products = new List<ProductDTO>();
            dataTransfer = new DataTransfer();
            TempData["GeneralPageName"] = Nid.ToString();
            var Settings = dataTransfer.GetAllSettings();
            switch (Nid)
            {
                case 1:
                    Products = dataTransfer.GetLatestProducts();
                    break;
                case 2:
                    Products = dataTransfer.GetSpecialProducts();
                    TempData["GeneralBanner"] = Settings.Where(p => p.SettingAttribute == "specialBanner").FirstOrDefault().SettingValue;
                    break;
                case 3:
                    Products = dataTransfer.GetPopularProducts();
                    TempData["GeneralBanner"] = Settings.Where(p => p.SettingAttribute == "popularBanner").FirstOrDefault().SettingValue;
                    break;
                default:
                    break;
            }
            ViewBag.Title = Title;
            return View(Products);
        }
        //[OutputCache(Duration = 85000, VaryByParam = "Nid")]
        public ActionResult Product(int Nid,string Title = "")
        {
            dataTransfer = new DataTransfer();
            ProductPageViewModel ppvm = new ProductPageViewModel();
            ppvm.Product = dataTransfer.GetProductByNumber(Nid);
            ppvm.Similars = dataTransfer.GetSimilarProductsByNumber(Nid);
            ViewBag.Title = Title;
            ViewBag.Desc = ppvm.Product.DetailDescription;
            return View(ppvm);
        }
        //[OutputCache(Duration = 2600000)]
        public ActionResult AboutUs()//done
        {
            return View();
        }
        //[OutputCache(Duration = 2600000)]
        public ActionResult Blog()
        {
            return View();
        }
        //[OutputCache(Duration = 2600000)]
        public ActionResult Categories()//done
        {
            dataTransfer = new DataTransfer();
            var result = dataTransfer.GetAllCategory();
            return View(result);
        }
        public ActionResult MoreCategories(int PageNumber)//done
        {
            dataTransfer = new DataTransfer();
            var result = dataTransfer.GetAllCategory(10,PageNumber*10);
            return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_MoreCategories", result), tmpNidCategory = PageNumber + 1 });
        }
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
        public ActionResult SearchThis(string Text,int Nidcategory)//done
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
        //user section
        //[OutputCache(Duration = 7800000)]
        public ActionResult Register()//done
        {
            return View();
        }
        public ActionResult SubmitRegister(User User)//done
        {
            dataTransfer = new DataTransfer();
            if (!dataTransfer.CheckUsername(User.Username))
            {
                User.CreateDate = DateTime.Now;
                User.Enabled = true;
                User.IsAdmin = false;
                User.NidUser = Guid.NewGuid();
                User.Password = DataTransfer.Encrypt(User.Password);
                if (dataTransfer.AddUser(User))
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
        //[OutputCache(Duration = 7800000)]
        public ActionResult Login()//done
        {
            return View();
        }
        public ActionResult SubmitLogin(string Username, string Password, bool isPersistant)//done
        {
            dataTransfer = new DataTransfer();
            var User = dataTransfer.GetUserByUsername(Username);
            if (User != null)
            {
                if (DataTransfer.Encrypt(Password) == User.Password)
                {
                    if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
                    {
                        Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddYears(-1);
                    }
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
                    //Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddHours(30);
                    //Response.Cookies["AudioShopLogin"].HttpOnly = true;
                    //Response.Cookies["AudioShopLogin"].Secure = true;
                    return Json(new JsonResults() { HasValue = true });
                }
                else
                    return Json(new JsonResults() { HasValue = false, Message = "رمز عبور اشتباه می باشد" });
            }
            else
                return Json(new JsonResults() { HasValue = false, Message = "نام کاربری موجود نمی باشد" });
        }
        public ActionResult Logout()//done
        {
            Response.Cookies["AudioShopLogin"].Expires = DateTime.Now.AddYears(-1);
            Response.Cookies["AudioShopCart"].Expires = DateTime.Now.AddYears(-1);
            Response.Cookies["AudioShopFavorites"].Expires = DateTime.Now.AddYears(-1);
            return RedirectToAction("Index");
        }
        public ActionResult MyFavorites()//done
        {
            List<Favorite> favs = new List<Favorite>();
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                favs = dataTransfer.GetAllFavoritesByNidUser(Guid.Parse(niduser));
            }
            return View(favs);
        }
        public ActionResult MyProfile()//done
        {
            ProfileViewModel pvm = new ProfileViewModel();
            List<Order> Orders = new List<Order>();
            Models.User User = new User();
            List<Favorite> favs = new List<Favorite>();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                dataTransfer = new DataTransfer();
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string NidUser = ticket.UserData.Split(',').First();
                User = dataTransfer.GetUserByNidUser(Guid.Parse(NidUser));
                Orders = dataTransfer.GetUsersOrder(Guid.Parse(NidUser));
                favs = dataTransfer.GetAllFavoritesByNidUser(Guid.Parse(NidUser));
            }
            pvm.Orders = Orders;
            pvm.UserInfo = User;
            pvm.Favorites = favs;
            return View(pvm);
        }
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword)//done
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
                            message = "رمز عبور شما با موفقیت تغییر کرد";
                            IsUpdated = true;
                        }
                        else
                            message = "مشکل در سرور.لطفا مجددا امتحان نمایید";
                    }
                    else
                        message = "لطفا رمز عبور فعلی خود را صحیح وارد نمایید";
                }
                else
                    message = "نام کاربری یافت نشد";
            }
            else
                message = "کاربر وارد نشده است";

            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult ChangeAddress(string NewAddress)//done
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
                        message = "آدرس شما با موفقیت ویرایش گردید";
                        IsUpdated = true;
                    }
                }
                else
                    message = "نام کاربری یافت نشد";
            }
            else
                message = "کاربر وارد نشده است";
            return Json(new JsonResults() { HasValue = IsUpdated, Message = message });
        }
        public ActionResult SubmitAddReview(string CommentText, string NidUser)//done
        {
            Comment tmpcomment = new Comment() { CreateDate = DateTime.Now, NidComment = Guid.NewGuid(), State = 2 };
            if (NidUser != "")
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
        public ActionResult UpdateUserAddress(Guid NidUser, string Address, string ZipCode, string Tel)
        {
            dataTransfer = new DataTransfer();
            var user = dataTransfer.GetUserByNidUser(NidUser);
            user.Address = Address;
            user.ZipCode = decimal.Parse(ZipCode);
            user.Tel = Tel;
            if (dataTransfer.UpdateUser(user))
                return Json(new JsonResults() { HasValue = true, Message = "اطلاعات کاربری با موفقیت ثبت گردید" });
            else
                return Json(new JsonResults() { HasValue = false, Message = "خطا در سرور.لطفا مجددا امتحان کنید" });
        }
        public ActionResult AddToNewsletter(string Mail)
        {
            Setting setting = new Setting() { NidSetting = Guid.NewGuid(), SettingAttribute = "NewsletterMail", SettingValue = Mail };
            dataTransfer = new DataTransfer();
            return Json(new JsonResults() { HasValue = dataTransfer.AddSetting(setting) });
        }
        //shop section
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
        public ActionResult AddProductToCart(Guid NidProduct, int Quantity,int price)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                dataTransfer = new DataTransfer();
                int carts = dataTransfer.AddCart(new Cart() { CreateDate = DateTime.Now, NidCart = Guid.NewGuid(), NidProduct = NidProduct, NidUser = Guid.Parse(niduser), Quantity = Quantity });
                if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                {
                    Response.Cookies["AudioShopCart"].Value = carts.ToString();
                }
                else
                {
                    HttpCookie newCookie = new HttpCookie("AudioShopCart", carts.ToString());
                    Response.Cookies.Add(newCookie);
                }
                return Json(new JsonResults() { HasValue = true, tmpNidCategory = carts, Html = RenderViewToString(this.ControllerContext, "_CartPopup", null), Message = @String.Format("{0:n0} ریال", price) });
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
                var tmpcart = dataTransfer.GetCartByNidUserAndProduct(Guid.Parse(niduser), NidProduct);
                if (tmpcart != null)
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
                    return Json(new JsonResults() { HasValue = true, Html = RenderViewToString(this.ControllerContext, "_CartPopup", null), tmpNidCategory = carts, Message = dataTransfer.CartPriceAggregateByNidUser(Guid.Parse(niduser)).ToString() });
                }
            }
            return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult CartQuantityChanged(Guid NidCart, int Quantity)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();

                dataTransfer = new DataTransfer();
                decimal tmpresult = dataTransfer.UpdateCartQuantity(NidCart, Quantity);
                if (tmpresult != 0)
                    return Json(new JsonResults() { HasValue = true, Html = String.Format("{0:n0}", tmpresult), Message = String.Format("{0:n0}", dataTransfer.CartPriceAggregateByNidUser(Guid.Parse(niduser))) });
                else
                    return Json(new JsonResults() { HasValue = false });
            }
            else
                return Json(new JsonResults() { HasValue = false });
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
                return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult RemoveProductFromFav(Guid NidFav)//done
        {
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                dataTransfer = new DataTransfer();
                var tmpfav = dataTransfer.GetFavoriteById(NidFav);
                if (tmpfav != null)
                {
                    int favs = dataTransfer.RemoveFavorite(tmpfav);
                    if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                    {
                        Response.Cookies["AudioShopFavorites"].Value = favs.ToString();
                    }
                    else
                    {
                        HttpCookie newCookie = new HttpCookie("AudioShopFavorites", favs.ToString());
                        Response.Cookies.Add(newCookie);
                    }
                    return Json(new JsonResults() { HasValue = true, tmpNidCategory = favs });
                }
            }
            return Json(new JsonResults() { HasValue = false });
        }
        public ActionResult Checkout()
        {
            CheckoutViewModel cvm = new CheckoutViewModel();
            List<Cart> carts = new List<Cart>();
            User user = null;
            Order order = null;
            dataTransfer = new DataTransfer();
            if (Request.Cookies.AllKeys.Contains("AudioShopLogin"))
            {
                var ticket = FormsAuthentication.Decrypt(Request.Cookies["AudioShopLogin"].Value);
                string niduser = ticket.UserData.Split(',').First();
                carts = dataTransfer.GetAllCartByNidUser(Guid.Parse(niduser));
                user = dataTransfer.GetUserByNidUser(Guid.Parse(niduser));
                order = dataTransfer.GetUsersOrder(Guid.Parse(niduser)).Where(p => p.state != 100 && p.state != 101).FirstOrDefault();
            }
            cvm.User = user;
            cvm.Carts = carts;
            cvm.Order = order;
            return View(cvm);
        }
        public ActionResult CheckoutSubmit(Order Order)
        {
            dataTransfer = new DataTransfer();
            if (Order.NidOrder == Guid.Empty)
            {
                var tmpCarts = dataTransfer.GetAllCartByNidUser(Order.NidUser);
                decimal total = 0;
                foreach (var cart in tmpCarts)
                {
                    total += cart.Product.Price * cart.Quantity ?? 1;
                }
                Order.CreateDate = DateTime.Now;
                Order.NidOrder = Guid.NewGuid();
                Order.state = 0;
                Order.TotalPrice = total;
                if (dataTransfer.AddOrder(Order))
                {
                    foreach (var cart in tmpCarts)
                    {
                        cart.NidOrder = Order.NidOrder;
                        dataTransfer.UpdateCart(cart);
                    }
                }
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در سرور لطفا مجددا امتحان کنید" });
                if (!dataTransfer.AddShip(new Ship() { NidShip = Guid.NewGuid(), NidOrder = Order.NidOrder, CreateDate = DateTime.Now, Address = Order.Address, ShipPrice = 0, State = 0, ZipCode = Order.Zipcode }))
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در سرور لطفا مجددا امتحان کنید" });

                return Json(new JsonResults() { HasValue = true, Message = Order.NidOrder.ToString() });
            }
            else
            {
                if (dataTransfer.UpdateOrder(Order))
                {
                    var tmpShip = dataTransfer.GetShipByNidOrder(Order.NidOrder);
                    if (tmpShip == null)
                    {
                        if (!dataTransfer.AddShip(new Ship() { NidShip = Guid.NewGuid(), NidOrder = Order.NidOrder, CreateDate = DateTime.Now, Address = Order.Address, ShipPrice = 0, State = 0, ZipCode = Order.Zipcode }))
                            return Json(new JsonResults() { HasValue = false, Message = "خطا در سرور لطفا مجددا امتحان کنید" });
                    }
                    return Json(new JsonResults() { HasValue = true, Message = Order.NidOrder.ToString() });
                }
                else
                    return Json(new JsonResults() { HasValue = false, Message = "خطا در سرور لطفا مجددا امتحان کنید" });
            }
        }
        public ActionResult CheckoutDetail(Guid NidOrder)
        {
            dataTransfer = new DataTransfer();
            var order = dataTransfer.GetOrderByNidOrder(NidOrder);
            List<Cart> carts = dataTransfer.GetAllCartsByNidOrder(NidOrder);
            CheckoutViewModel cvm = new CheckoutViewModel() { Carts = carts, Order = order, User = order.User };
            return View(cvm);
        }
        public ActionResult Payment(Guid NidOrder, decimal TotalPrice)
        {
            dataTransfer = new DataTransfer();
            var tmporder = dataTransfer.GetOrderByNidOrder(NidOrder);
            if (tmporder != null)
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                //ZarinpalTest.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinpalTest.PaymentGatewayImplementationServicePortTypeClient();//debug
                string Authority;
                int ProcessedPrice = decimal.ToInt32(tmporder.TotalPrice / 10);
                int Status = zp.PaymentRequest(Merchant, ProcessedPrice, "خرید از سایت کاربیس", tmporder.Email, tmporder.Tel, "https://carbass.ir/Verify?NidOrder=" + NidOrder, out Authority);
                //int Status = zp.PaymentRequest(Merchant, ProcessedPrice, "خرید از سایت کاربیس", tmporder.Email, tmporder.Tel, "http://localhost:2000/Verify?NidOrder=" + NidOrder, out Authority);//debug

                if (Status == 100)
                {
                    return Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                    //return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + Authority);//debug
                }
                else
                {
                    TempData["dargahRedirectError"] = "در انتقال به درگاه خطایی رخ داده است.لطفا مجدد امتحان کنید";
                    return RedirectToAction("CheckoutDetail", "Home");
                }
            }
            else
            {
                TempData["dargahRedirectError"] = "خطایی در سرور رخ داده است.لطفا مجدد امتحان نمایید";
                return RedirectToAction("CheckoutDetail", "Home");
            }
        }
        public ActionResult Verify(Guid NidOrder)
        {
            dataTransfer = new DataTransfer();
            Order tmpOrder = dataTransfer.GetOrderByNidOrder(NidOrder);
            List<Cart> carts = dataTransfer.GetAllCartsByNidOrder(NidOrder);
            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    int Amount = decimal.ToInt32(tmpOrder.TotalPrice / 10);
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                    //ZarinpalTest.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinpalTest.PaymentGatewayImplementationServicePortTypeClient();//debug

                    int Status = zp.PaymentVerification(Merchant, Request.QueryString["Authority"].ToString(), Amount, out RefID);
                    tmpOrder.state = Status;
                    tmpOrder.RefId = RefID;
                    if (dataTransfer.UpdateOrder(tmpOrder))
                    {
                        foreach (var cart in carts)
                        {
                            cart.State = 1;
                            if(dataTransfer.UpdateCart(cart))
                            {
                                var cartandfav = dataTransfer.GetCartAndFavoriteCount(tmpOrder.NidUser);
                                if (Request.Cookies.AllKeys.Contains("AudioShopCart"))
                                {
                                    Response.Cookies["AudioShopCart"].Value = cartandfav.Item1.ToString();
                                }
                                if (Request.Cookies.AllKeys.Contains("AudioShopFavorites"))
                                {
                                    Response.Cookies["AudioShopFavorites"].Value = cartandfav.Item2.ToString();
                                }
                            }
                        }
                        var tmpship = dataTransfer.GetShipByNidOrder(tmpOrder.NidOrder);
                        tmpship.State = 1;//paid
                        dataTransfer.UpdateShip(tmpship);
                    }
                }
                else
                {
                    tmpOrder.state = -100;
                    //dataTransfer.AddUnsuccessfullOrder(new UnsuccessfullOrder() {  NidUnsuccessfullOrder = Guid.NewGuid(), Address = tmpOrder.Address, CityId = tmpOrder.CityId, CreateDate = tmpOrder.CreateDate, Description = tmpOrder.Description, Email = tmpOrder.Email, Firstname = tmpOrder.Firstname, Lastname = tmpOrder.Lastname, NidOrder = tmpOrder.NidOrder, NidUser = tmpOrder.NidUser, RefId = tmpOrder.RefId, state = tmpOrder.state, StateId = tmpOrder.StateId, Tel = tmpOrder.Tel, TotalPrice = tmpOrder.TotalPrice, Zipcode = tmpOrder.Zipcode});
                    dataTransfer.UpdateOrder(tmpOrder);
                }
            }
            else
            {
                tmpOrder.state = -101;
                //dataTransfer.AddUnsuccessfullOrder(new UnsuccessfullOrder() { NidUnsuccessfullOrder = Guid.NewGuid(), Address = tmpOrder.Address, CityId = tmpOrder.CityId, CreateDate = tmpOrder.CreateDate, Description = tmpOrder.Description, Email = tmpOrder.Email, Firstname = tmpOrder.Firstname, Lastname = tmpOrder.Lastname, NidOrder = tmpOrder.NidOrder, NidUser = tmpOrder.NidUser, RefId = tmpOrder.RefId, state = tmpOrder.state, StateId = tmpOrder.StateId, Tel = tmpOrder.Tel, TotalPrice = tmpOrder.TotalPrice, Zipcode = tmpOrder.Zipcode });
                dataTransfer.UpdateOrder(tmpOrder);
            }
            return View(new CheckoutViewModel() { Carts = carts, Order = tmpOrder });
        }
        //generals
        public static string RenderViewToString(ControllerContext context, string viewName, object model)//done
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