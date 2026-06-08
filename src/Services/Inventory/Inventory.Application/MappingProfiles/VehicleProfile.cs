using AutoMapper;
using Inventory.Application.Features.Brands.Dtos;
using Inventory.Application.Features.Models.Dtos;
using Inventory.Application.Features.Variants.Dtos;
using Inventory.Application.Features.Vehicles.Dtos;
using Inventory.Application.Models.Vehicles;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

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

        CreateMap<Brand, BrandCacheDto>()
            .ReverseMap();

        CreateMap<Model, ModelCacheDto>()
            .ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand));
        CreateMap<ModelCacheDto, Model>()
            .ConstructUsing(src => new Model(
                src.Name,
                src.YearOfProduction,
                src.NumberOfSeats,
                src.Category,
                src.Brand.Id
            ))
            .ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand));

        CreateMap<Variant, VariantCacheDto>()
            .ForMember(d => d.Model, opt => opt.MapFrom(s => s.Model));
        CreateMap<VariantCacheDto, Variant>()
            .ConstructUsing(src => new Variant(
                src.Name,
                src.Gearbox,
                src.FuelType,
                src.Power,
                src.EngineSize,
                src.Model.Id
            ))
            .ForMember(d => d.Model, opt => opt.MapFrom(s => s.Model));

        CreateMap<Vehicle, VehicleCacheDto>()
            .ForMember(d => d.Variant, opt => opt.MapFrom(s => s.Variant));
        CreateMap<VehicleCacheDto, Vehicle>()
            .ConstructUsing(src => new Vehicle(
                src.RentalPricePerDay,
                src.RegistrationPlates,
                src.MileageKm,
                src.Status,
                src.Variant.Id
            ))
            .ForMember(d => d.Variant, opt => opt.MapFrom(s => s.Variant));
    }
}
