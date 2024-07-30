using AutoMapper;
//using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.MappingProfiles;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<Vehicle, VehicleResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(x => x.Id))
            .ForMember(d => d.Brand, o => o.MapFrom(x => x.Variant.Model.Brand.Name))
            .ForMember(d => d.Model, o => o.MapFrom(x => x.Variant.Model.Name))
            .ForMember(d => d.Variant, o => o.MapFrom(x => x.Variant.Name));

        CreateMap<Vehicle, VehicleSearchResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(x => x.Id))
            .ForMember(d => d.Brand, o => o.MapFrom(x => x.Variant.Model.Brand.Name))
            .ForMember(d => d.Model, o => o.MapFrom(x => x.Variant.Model.Name))
            .ForMember(d => d.RentalPricePerDay, o => o.MapFrom(x => x.RentalPricePerDay))
            .ForMember(d => d.Gearbox, o => o.MapFrom(x => x.Variant.Gearbox))
            .ForMember(d => d.Category, o => o.MapFrom(x => x.Variant.Model.Category))
            .ForMember(d => d.NumberOfSeats, o => o.MapFrom(x => x.Variant.Model.NumberOfSeats))
            .ForMember(d => d.Power, o => o.MapFrom(x => x.Variant.Power));

        //CreateMap<CreateBrandCommand, Brand>();
    }
}
