using AutoMapper;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Features.Brands.Commands.UpdateBrand;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.MappingProfiles;

public class BrandProfile : Profile
{
    public BrandProfile()
    {
        CreateMap<Brand, CreateBrandCommand>().ReverseMap();
        CreateMap<Brand, UpdateBrandCommand>().ReverseMap();
    }
}
