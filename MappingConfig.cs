using AutoMapper;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
                CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
                CreateMap<Coupon, CouponDto>().ReverseMap();

        }
    }
}
