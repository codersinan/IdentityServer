using AutoMapper;
using IdentityServer.Core.Entities;
using IdentityServer.Infrastructure.RequestModels;

namespace IdentityServer.Infrastructure.Mappings
{
    public class SignUpRequestMapping : Profile
    {
        public SignUpRequestMapping()
        {
            CreateMap<SignUpRequest, Account>()
                .ForMember(d => d.PasswordHash,
                    m => m.MapFrom(e => e.Password)
                );
        }
    }
}