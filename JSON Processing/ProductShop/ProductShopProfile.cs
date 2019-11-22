using AutoMapper;
using ProductShop.Dto_s;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<Product, ExportProductDto>()
                .ForMember(x => x.Seller, y => y.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            CreateMap<User, UserWithSalesDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(s => s.ProductsSold));
        }
    }
}
