using AutoMapper;
using WebApi.Dtos;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Defect, DefectCodeDto>();
            CreateMap<DefectCodeDto, Defect>();
        }
    }
}
