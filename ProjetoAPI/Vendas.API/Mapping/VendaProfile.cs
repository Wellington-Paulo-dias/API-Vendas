using AutoMapper;
using Vendas.Domain.DTOs;
using Vendas.Domain.Entities;

namespace Vendas.API.Mapping
{
    public class VendaProfile : Profile
    {
        public VendaProfile()
        {
            
            CreateMap<Venda, VendaDTO>()               
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
                .ReverseMap();

           
            CreateMap<ItemVenda, ItemVendaDTO>()
                .ForMember(dest => dest.ValorTotal, opt => opt.MapFrom(src => src.ValorTotal))
                .ReverseMap();
        }
       
    }
}
