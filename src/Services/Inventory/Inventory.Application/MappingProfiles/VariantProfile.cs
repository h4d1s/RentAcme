using AutoMapper;
using Inventory.Application.Features.Brands.Dtos;
using Inventory.Application.Features.Models.Dtos;
using Inventory.Application.Features.Variants.Dtos;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;

namespace Inventory.Application.MappingProfiles;

public class VariantProfile : Profile
{
    public VariantProfile()
    {
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
    }
}

