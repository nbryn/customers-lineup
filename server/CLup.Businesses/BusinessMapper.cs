using System;

using AutoMapper;

using CLup.Businesses.DTO;
namespace CLup.Businesses
{
    public class BusinessMapper : Profile
    {
        public BusinessMapper()
        {
            CreateMap<Business, BusinessDTO>()
                .ForMember(b => b.Type, s => s.MapFrom(m => m.Type.ToString()));

            CreateMap<BusinessRequest, Business>()
                .ForMember(b => b.Type, s => s.MapFrom(m => BusinessType.Parse(typeof(BusinessType), m.Type)))
                .ForMember(b => b.OwnerEmail, s => s.MapFrom(m => m.OwnerEmail))
                .ForMember(u => u.CreatedAt, s => s.MapFrom(m => DateTime.Now))
                .ForMember(u => u.UpdatedAt, s => s.MapFrom(m => DateTime.Now));
        }
    }
}
