using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;

namespace AudioShopFrontend.Services
{
    interface IDataTransfer
    {
        //index
        List<CategoryLiteDTO> GetcategoryList();
        //category
        CategoryDTO GetCategoryByNidCategory(int Nidcategory);
        CategoryDTO GetCategoryByCategoryName(string CategoryName);
        List<ProductDTO> GetLatestProducts(int pagesize = 10,int toskip = 0);
        List<ProductDTO> GetPopularProducts(int pagesize = 10, int toskip = 0);
        List<ProductDTO> GetSpecialProducts(int pagesize = 10, int toskip = 0);
        List<Category> GetAllCategory(int pagesize = 10, int toskip = 0);
        //product
        Product GetProductByID(Guid NidProduct);
        ProductDTO GetProductDtoByID(Guid NidProduct);
        List<ProductDTO> GetSimilarProducts(Guid NidProduct);
        //User
        User GetUserByUsername(string Username);
        User GetUserByNidUser(Guid NidUser);
        bool UpdateUser(User user);
        bool CheckUsername(string Username);
        bool AddUser(User User);
        Tuple<int, int> GetCartAndFavoriteCount(Guid NidUser);
        //favorites
        List<ProductDTO> GetUserFavorites(Guid UserId);
        List<Favorite> GetAllFavoritesByNidUser(Guid NidUser,int pagesize = 10);
        int AddFavorite(Favorite favorite);
        Favorite GetFavoriteById(Guid NidFav);
        int RemoveFavorite(Favorite favorite);
        //cart
        List<Cart> GetAllCartByNidUser(Guid NidUser,int pagesize = 10);
        int AddCart(Cart cart);
        int RemoveCart(Cart cart);
        decimal CartPriceAggregateByNidUser(Guid NidUser);
        Cart GetCartByNidCart(Guid NidCart);
        Cart GetCartByNidUserAndProduct(Guid NidUser,Guid NidProduct);
        int UpdateCartQuantity(Guid NidCart, int Quantity);
        //search
        List<ProductDTO> SearchProduct(string input,int Nidcategory = 0);
        //orders
        List<Order> GetUsersOrder(Guid NidUser);
        bool AddOrder(Order order);
        Order GetOrderByNidOrder(Guid NidOrder);
        bool UpdateOrder(Order order);
        //comment
        bool AddComment(Comment comment);
        //settings
        List<Setting> GetAllSettings(int pagesize = 100);
        List<Category_BrandDTO> GetCategory_BrandByNidcategory(int Nidcategory);
        List<Category_TypeDTO> GetCategory_TypeByNidcategory(int Nidcategory);
        List<ProductDTO> GetProductsByNidcategory(int Nidcategory,int pagesize = 10,int toSkip = 0,decimal MinPrice = 0,decimal MaxPrice = 0,string NidBrands = "",string NidTypes = "",int FilterType = 0);
        Tuple<decimal, decimal> GetMinMaxCategoryPrice(int Nidcategory);
    }
}
