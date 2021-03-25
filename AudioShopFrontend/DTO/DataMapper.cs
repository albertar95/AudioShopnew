using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using AudioShopFrontend.Models;
using AudioShopFrontend.DTO;
using AudioShopFrontend.App_Start;

namespace AudioShopFrontend.DTO
{
    public class DataMapper
    {
         static MapperConfiguration config = MapperConfig.Configure();

        //build the mapper
         IMapper mapper = config.CreateMapper();
        public  CategoryLiteDTO MapToCategoryLite(Category category)
        {
            try
            {
                return mapper.Map<CategoryLiteDTO>(category);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CategoryDTO MapToCategoryDto(Category category)
        {
            try
            {
                return mapper.Map<CategoryDTO>(category);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Category_BrandDTO MapToCategory_BrandDTO(Category_Brands category_brand)
        {
            try
            {
                return mapper.Map<Category_BrandDTO>(category_brand);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Category_TypeDTO MapToCategory_TypeDTO(Category_Types category_type)
        {
            try
            {
                return mapper.Map<Category_TypeDTO>(category_type);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public ProductDTO MapToProductDto(Product product)
        {
            try
            {
                return mapper.Map<ProductDTO>(product);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}