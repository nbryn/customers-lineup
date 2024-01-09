﻿using System;
using AutoMapper;
using CLup.Domain.Businesses;
using CLup.Domain.Businesses.Enums;
using CLup.Domain.Businesses.ValueObjects;
using CLup.Domain.Shared.ValueObjects;

namespace CLup.Application.Businesses.Commands.CreateBusiness;

public sealed class CreateBusinessMapper : Profile
{
    public CreateBusinessMapper()
    {
        CreateMap<CreateBusinessCommand, Business>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.BusinessData,
                opts => opts.MapFrom(src => new BusinessData(src.Name, src.Capacity, src.TimeSlotLength)))
            .ForMember(dest => dest.Address, opts => opts.MapFrom(src => new Address(src.Street, src.Zip, src.City)))
            .ForMember(dest => dest.Coords, opts => opts.MapFrom(src => new Coords(src.Longitude, src.Latitude)))
            .ForMember(dest => dest.BusinessHours, opts => opts.MapFrom(src => new Interval(src.Opens, src.Closes)))
            .ForMember(dest => dest.Type, opts => opts.MapFrom(src => Enum.Parse(typeof(BusinessType), src.Type)));
    }
}