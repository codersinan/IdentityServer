using AutoMapper;
using IdentityServer.Core.Entities;
using IdentityServer.Infrastructure.ResponseModels;

namespace IdentityServer.Infrastructure.Mappings
{
    public class CurrentAccountMapping : Profile
    {
        public CurrentAccountMapping()
        {
            CreateMap<Account, CurrentAccount>()
                .ForMember(
                    d => d.Username,
                    m =>
                        m.MapFrom(e => e.Username)
                )
                .ForMember(
                    d => d.Email,
                    m =>
                        m.MapFrom(e => e.Username)
                )
                .ForMember(
                    d => d.CreatedAt,
                    m =>
                        m.MapFrom(e => e.CreatedAt)
                );
        }
    }
}