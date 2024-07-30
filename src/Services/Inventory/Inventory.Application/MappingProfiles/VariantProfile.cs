using AutoMapper;
using Inventory.Application.Features.Variants.Commands.CreateVariant;
using Inventory.Application.Features.Variants.Commands.UpdateVariant;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.MappingProfiles;

public class VariantProfile : Profile
{
    public VariantProfile()
    {
        CreateMap<Variant, CreateVariantCommand>().ReverseMap();
        CreateMap<Variant, UpdateVariantCommand>().ReverseMap();
    }
}
