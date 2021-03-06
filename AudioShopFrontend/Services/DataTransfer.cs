using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace AudioShopFrontend.Services
{
    public class DataTransfer : IDataTransfer
    {
        ASDbEntities db = new ASDbEntities();
        DataMapper mapper = new DataMapper();
        static string hashkey { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public CategoryDTO GetCategoryByCategoryName(string CategoryName)
        {
            return mapper.MapToCategoryDto(db.Categories.Where(p => p.CategoryName == CategoryName).FirstOrDefault());
        }

        public CategoryDTO GetCategoryByNidCategory(int Nidcategory)
        {
            return mapper.MapToCategoryDto(db.Categories.Where(p => p.NidCategory == Nidcategory).FirstOrDefault());
        }

        public List<CategoryLiteDTO> GetcategoryList()
        {
            List<CategoryLiteDTO> result = new List<CategoryLiteDTO>();
            foreach (var cat in db.Categories.Where(p => p.IsSubmmited == true))
            {
                result.Add(mapper.MapToCategoryLite(cat));
            }
            return result;
        }

        public List<ProductDTO> GetLatestProducts(int pagesize = 10, int toskip = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            foreach (var pro in db.Products.Where(p => p.State == 0).OrderByDescending(q => q.CreateDate).Skip(toskip).Take(pagesize))
            {
                result.Add(mapper.MapToProductDto(pro));
            }
            return result;
        }

        public List<ProductDTO> GetPopularProducts(int pagesize = 10, int toskip = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            var MostWanted = db.Carts.Where(w => w.State == 1).GroupBy(p => p.NidProduct).Select(q => new { nidprod = q.Key, cnt = q.Count() });
            //var MostWanted = db.Orders.GroupBy(p => p.NidProducts).Select(q =>  new { nidprod = q.Key,cnt = q.Count()});
            foreach (var pro in MostWanted.OrderByDescending(q => q.cnt).Skip(toskip).Take(pagesize))
            {
                result.Add(GetProductDtoByID(pro.nidprod));
            }
            return result;
        }

        public Product GetProductByID(Guid NidProduct)
        {
            return db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault();
        }

        public List<ProductDTO> GetSpecialProducts(int pagesize = 10, int toskip = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            foreach (var pro in db.Products.Where(p => p.State == 0).OrderByDescending(q => q.Priority).OrderByDescending(w => w.CreateDate).Skip(toskip).Take(pagesize))
            {
                result.Add(mapper.MapToProductDto(pro));
            }
            return result;
        }

        public User GetUserByUsername(string Username)
        {
            try
            {
                return db.Users.Where(p => p.Username == Username).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string s = ex.InnerException.Message;
                return null;
            }
        }

        public static string Encrypt(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashkey));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashkey));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public bool CheckUsername(string Username)
        {
            return db.Users.Where(p => p.Username == Username).Any();
        }

        public bool AddUser(User User)
        {
            db.Users.Add(User);
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public List<ProductDTO> GetUserFavorites(Guid UserId)
        {
            return null;
        }

        public ProductDTO GetProductDtoByID(Guid NidProduct)
        {
            return mapper.MapToProductDto(db.Products.Where(p => p.NidProduct == NidProduct).FirstOrDefault());
        }

        public List<ProductDTO> SearchProduct(string input, int Nidcategory = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            if(Nidcategory != 0)
            { 
            foreach (var sr in db.Products.Where(p => p.Category.NidCategory == Nidcategory && p.ProductName.Contains(input)).Take(3))
            {
                result.Add(mapper.MapToProductDto(sr));
            }
            }
            else
            {
                foreach (var sr in db.Products.Where(p => p.ProductName.Contains(input)).Take(3))
                {
                    result.Add(mapper.MapToProductDto(sr));
                }
            }
            return result;
        }

        public User GetUserByNidUser(Guid NidUser)
        {
            try
            {
                return db.Users.Where(p => p.NidUser == NidUser).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Order> GetUsersOrder(Guid NidUser)
        {
            try
            {
                return db.Orders.Where(p => p.NidUser == NidUser).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Favorite> GetAllFavoritesByNidUser(Guid NidUser, int pagesize = 10)
        {
            return db.Favorites.Where(p => p.NidUser == NidUser).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public int AddFavorite(Favorite favorite)
        {
            if (!db.Favorites.Where(p => p.NidProduct == favorite.NidProduct && p.NidUser == favorite.NidUser).Any())
            {
                db.Favorites.Add(favorite);
                db.SaveChanges();
            }
            return db.Favorites.Where(p => p.NidUser == favorite.NidUser).Count();
        }

        public List<Cart> GetAllCartByNidUser(Guid NidUser, int pagesize = 10)
        {
            return db.Carts.Where(p => p.NidUser == NidUser && p.State == 0).OrderByDescending(q => q.CreateDate).Take(pagesize).ToList();
        }

        public int AddCart(Cart cart)
        {
            if(!db.Carts.Where(p => p.NidProduct == cart.NidProduct && p.NidUser == cart.NidUser && p.State == 0).Any())
            {
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return db.Carts.Where(p => p.NidUser == cart.NidUser && p.State == 0).Count();
        }

        public Tuple<int, int> GetCartAndFavoriteCount(Guid NidUser)
        {
            int tmpCart = db.Carts.Where(p => p.NidUser == NidUser && p.State == 0).Count();
            int tmpfav = db.Favorites.Where(p => p.NidUser == NidUser).Count();
            return new Tuple<int, int>(tmpCart,tmpfav);
        }

        public List<Category_BrandDTO> GetCategory_BrandByNidcategory(int Nidcategory)
        {
            List<Category_BrandDTO> result = new List<Category_BrandDTO>();
            foreach (var brd in db.Category_Brands.Where(p => p.NidCategory == Nidcategory))
            {
                result.Add(mapper.MapToCategory_BrandDTO(brd));
            }
            return result;
        }

        public List<Category_TypeDTO> GetCategory_TypeByNidcategory(int Nidcategory)
        {
            List<Category_TypeDTO> result = new List<Category_TypeDTO>();
            foreach (var typ in db.Category_Types.Where(p => p.NidCategory == Nidcategory))
            {
                result.Add(mapper.MapToCategory_TypeDTO(typ));
            }
            return result;
        }

        public List<ProductDTO> GetProductsByNidcategory(int Nidcategory, int pagesize = 10, int toSkip = 0, decimal MinPrice = 0, decimal MaxPrice = 0, string NidBrands = "", string NidTypes = "", int FilterType = 0)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            List<Guid> tmpBrands = new List<Guid>();
            List<Guid> tmptypes = new List<Guid>();
            if (FilterType != 0)
            {
                if(NidBrands != "")
                {
                    foreach (var brd in NidBrands.Split(','))
                    {
                        tmpBrands.Add(Guid.Parse(brd));
                    }
                }
                if(NidTypes != "")
                {
                    foreach (var typ in NidTypes.Split(','))
                    {
                        tmptypes.Add(Guid.Parse(typ));
                    }
                }
            }
            //1 price,2 brand,3 type
            //1:123,2:12,3:13,4:1,5:23,6:2,7:3
            switch (FilterType)
            {
                case 0:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 1:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && p.Price >= MinPrice && p.Price <= MaxPrice && tmpBrands.Contains(p.NidBrand) && tmptypes.Contains(p.NidType)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 2:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && p.Price >= MinPrice && p.Price <= MaxPrice && tmpBrands.Contains(p.NidBrand)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 3:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && p.Price >= MinPrice && p.Price <= MaxPrice && tmptypes.Contains(p.NidType)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 4:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && p.Price >= MinPrice && p.Price <= MaxPrice).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 5:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && tmpBrands.Contains(p.NidBrand) && tmptypes.Contains(p.NidType)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 6:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && tmpBrands.Contains(p.NidBrand)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                case 7:
                    foreach (var prod in db.Products.Where(p => p.NidCategory == Nidcategory && tmptypes.Contains(p.NidType)).OrderByDescending(q => q.CreateDate).Skip(toSkip).Take(pagesize))
                    {
                        result.Add(mapper.MapToProductDto(prod));
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public Tuple<decimal, decimal> GetMinMaxCategoryPrice(int Nidcategory)
        {
            var result = db.Products.Where(p => p.NidCategory == Nidcategory).GroupBy(q => q.Price).Select(w => w.Key);
            if(result.Any())
            return new Tuple<decimal, decimal>(result.Min(),result.Max());
            else
                return new Tuple<decimal, decimal>(0, 100000);
        }

        public bool AddComment(Comment comment)
        {
            db.Comments.Add(comment);
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public List<ProductDTO> GetSimilarProducts(Guid NidProduct)
        {
            var tmpProduct = GetProductDtoByID(NidProduct);
            List<ProductDTO> similars = new List<ProductDTO>();
            foreach (var cats in db.Products.Where(p => p.NidCategory == tmpProduct.NidCategory).Take(3))
            {
                similars.Add(mapper.MapToProductDto(cats));
            }
            var tmpName = tmpProduct.ProductName.Split(' ');
            foreach (var nm in tmpName)
            {
                foreach (var name in db.Products.Where(p => p.ProductName.Contains(nm)).Take(3))
                {
                    similars.Add(mapper.MapToProductDto(name));
                }
            }
            return similars;
        }

        public int RemoveCart(Cart cart)
        {
            db.Entry(cart).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return db.Carts.Where(p => p.NidUser == cart.NidUser && p.State == 0).Count();
        }

        public Cart GetCartByNidCart(Guid NidCart)
        {
            return db.Carts.Where(p => p.NidCart == NidCart && p.State == 0).FirstOrDefault();
        }

        public decimal CartPriceAggregateByNidUser(Guid NidUser)
        {
            if (db.Carts.Where(p => p.NidUser == NidUser && p.State == 0).Any())
                return db.Carts.Where(p => p.NidUser == NidUser && p.State == 0).Select(q => (q.Product.Price) * (q.Quantity ?? 1)).Sum();
            else
                return 0;
        }

        public Cart GetCartByNidUserAndProduct(Guid NidUser, Guid NidProduct)
        {
            return db.Carts.Where(p => p.NidUser == NidUser && p.NidProduct == NidProduct && p.State == 0).FirstOrDefault();
        }

        public decimal UpdateCartQuantity(Guid NidCart,int Quantity)
        {
            var tmpcart = db.Carts.Where(p => p.NidCart == NidCart).FirstOrDefault();
            tmpcart.Quantity = Quantity;
            db.Entry(tmpcart).State = System.Data.Entity.EntityState.Modified;
            if (db.SaveChanges() == 1)
                return Quantity*tmpcart.Product.Price;
            else
                return 0;
        }

        public Favorite GetFavoriteById(Guid NidFav)
        {
            return db.Favorites.Where(p => p.NidFav == NidFav).FirstOrDefault();
        }

        public int RemoveFavorite(Favorite favorite)
        {
            db.Entry(favorite).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return db.Favorites.Where(p => p.NidUser == favorite.NidUser).Count();
        }

        public List<Category> GetAllCategory(int pagesize = 10, int toskip = 0)
        {
            return db.Categories.Where(p => p.IsSubmmited == true && p.Products.Any()).OrderBy(w => w.NidCategory).Skip(toskip).Take(pagesize).ToList();
        }

        public List<Setting> GetAllSettings(int pagesize = 100)
        {
            return db.Settings.Take(pagesize).ToList();
        }

        public bool AddOrder(Order order)
        {
            db.Orders.Add(order);
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public Order GetOrderByNidOrder(Guid NidOrder)
        {
            return db.Orders.Where(p => p.NidOrder == NidOrder).FirstOrDefault();
        }

        public bool UpdateOrder(Order order)
        {
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public bool AddSetting(Setting setting)
        {
            db.Entry(setting).State = System.Data.Entity.EntityState.Added;
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public List<Cart> GetAllCartsByNidOrder(Guid NidOrder)
        {
            return db.Carts.Where(p => p.NidOrder == NidOrder).ToList();
        }

        public bool UpdateCart(Cart cart)
        {
            db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public bool AddShip(Ship ship)
        {
            db.Entry(ship).State = System.Data.Entity.EntityState.Added;
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }
        public bool UpdateShip(Ship ship)
        {
            db.Entry(ship).State = System.Data.Entity.EntityState.Modified;
            if (db.SaveChanges() == 1)
                return true;
            else
                return false;
        }

        public Ship GetShipByNidOrder(Guid NidOrder)
        {
            return db.Ships.Where(p => p.NidOrder == NidOrder).FirstOrDefault();
        }

        public Product GetProductByNumber(int Number)
        {
            return db.Products.Where(p => p.ProductNumber == Number).FirstOrDefault();
        }

        public ProductDTO GetProductDtoByNumber(int Number)
        {
            return mapper.MapToProductDto(db.Products.Where(p => p.ProductNumber == Number).FirstOrDefault());
        }

        public List<ProductDTO> GetSimilarProductsByNumber(int Number)
        {
            var tmpProduct = GetProductDtoByNumber(Number);
            List<ProductDTO> similars = new List<ProductDTO>();
            foreach (var cats in db.Products.Where(p => p.NidCategory == tmpProduct.NidCategory).Take(3))
            {
                similars.Add(mapper.MapToProductDto(cats));
            }
            var tmpName = tmpProduct.ProductName.Split(' ');
            foreach (var nm in tmpName)
            {
                foreach (var name in db.Products.Where(p => p.ProductName.Contains(nm)).Take(3))
                {
                    similars.Add(mapper.MapToProductDto(name));
                }
            }
            return similars;
        }
    }
}