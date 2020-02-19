using AutoMapper;
using IdentityServer.Core.Entities;
using IdentityServer.Infrastructure.RequestModels;

namespace IdentityServer.Infrastructure.Mappings
{
    public class SignInRequestMapping : Profile
    {
        public SignInRequestMapping()
        {
            CreateMap<SignInRequest, Account>()
                .ForMember(d => d.PasswordHash,
                    m => m.MapFrom(e => e.Password)
                );
        }
    }
}