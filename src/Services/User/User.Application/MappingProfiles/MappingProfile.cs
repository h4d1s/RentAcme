using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Features.Users.Commands.SignUp;
using User.Application.Models;

namespace User.Infrastructure.Services.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SignUpCommand, SignUpRequest>().ReverseMap();
    }
}
