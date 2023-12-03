using Accounts.Domain.Models;
using Accounts.DTOModels;
using AutoMapper;

namespace Accounts.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CustomerInfoObj, CustomerInfoDTO>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            CreateMap<CustomerInfoDTO, CustomerInfoObj>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}

