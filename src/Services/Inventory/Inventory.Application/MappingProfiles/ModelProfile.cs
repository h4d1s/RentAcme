using AutoMapper;
using Inventory.Application.Features.Brands.Commands.UpdateBrand;
using Inventory.Application.Features.Models.Commands.CreateModel;
using Inventory.Application.Features.Models.Commands.UpdateModel;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.MappingProfiles;

public class ModelProfile : Profile
{
    public ModelProfile()
    {
        CreateMap<Model, CreateModelCommand>().ReverseMap();
        CreateMap<Model, UpdateModelCommand>().ReverseMap();
    }
}
