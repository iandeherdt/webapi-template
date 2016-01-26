using AutoMapper;
using WebApiTemplate.Entities;
using WebApiTemplate.Models;

namespace WebApiTemplate
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<Product, ProductDto>().ReverseMap();

        }
    }
}
