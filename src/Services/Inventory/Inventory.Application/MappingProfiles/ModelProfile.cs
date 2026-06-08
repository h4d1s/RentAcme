using AutoMapper;
using Inventory.Application.Features.Brands.Dtos;
using Inventory.Application.Features.Models.Dtos;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;

namespace Inventory.Application.MappingProfiles;

public class ModelProfile : Profile
{
    public ModelProfile()
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
    }
}
