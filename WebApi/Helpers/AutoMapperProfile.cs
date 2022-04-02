using AutoMapper;
using DICS_WebApi.Dtos;
using CoreClass.Model;
using DICS_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DICS_WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<DefectCode, DefectCodeDto>();
            CreateMap<DefectCodeDto, DefectCode>();
        }
    }
}
