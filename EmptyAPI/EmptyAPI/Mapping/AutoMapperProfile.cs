using AutoMapper;
using EmptyAPI.Data.Entities;
using EmptyAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExcelData, DataReturnDto>();
        }
    }
}
