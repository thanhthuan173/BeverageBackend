using AutoMapper;
using BeverageBackend.Dto;
using BeverageBackend.Models;

namespace BeverageBackend.Helper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Customer, CustomerDto>();
        }
    }
}
