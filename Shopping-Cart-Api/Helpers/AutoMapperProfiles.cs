using AutoMapper;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.DTOs;

namespace Shopping_Cart_Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CartDetail, CartDetailDto>()
                .ForMember(dest => dest.ShoppingCartId, act => act.MapFrom(src => src.ShoppingCartId))
                .ForMember(dest => dest.UserId, act => act.MapFrom(src => src.ShoppingCart.UserId))
                .ForMember(dest => dest.ProductDetailId, act => act.MapFrom(src => src.ProductDetailId))
                .ForMember(dest => dest.Size, act => act.MapFrom(src => src.ProductDetail.Size))
                .ForMember(dest => dest.Stock, act => act.MapFrom(src => src.ProductDetail.Quantity))
                .ForMember(dest => dest.ProductId, act => act.MapFrom(src => src.ProductDetail.ProductId))
                .ForMember(dest => dest.ProductName, act => act.MapFrom(src => src.ProductDetail.Products.ProductName))
                .ForMember(dest => dest.Price, act => act.MapFrom(src => src.ProductDetail.Products.Price))
                .ForMember(dest => dest.Path, act => act.MapFrom(src => src.ProductDetail.Products.Image.Select(p => p.Path).FirstOrDefault()));
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductName, act => act.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Price, act => act.MapFrom(src => src.Price));
            CreateMap<ProductDetail, ProductDetailDto>()
                .ForMember(dest => dest.ProductId, act => act.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, act => act.MapFrom(src => src.Products.ProductName))
                .ForMember(dest => dest.Price, act => act.MapFrom(src => src.Products.Price))
                .ForMember(dest => dest.Quantity, act => act.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Size, act => act.MapFrom(src => src.Size))
                .ForMember(dest => dest.Images, act => act.MapFrom(src => src.Products.Image));
        }
    }
}