namespace DrawService.Api.Mapping
{
    using AutoMapper;
    
    using DrawService.Core.Models;
    using DrawService.Data.Models;

    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<Prize, PrizeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId));

            CreateMap<Draw, DrawDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TicketPriceInCents, opt => opt.MapFrom(src => src.TicketPriceInCents))
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.MinTicketsPerPlayer))
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer))
                .ForMember(dest => dest.MinPlayersInDraw, opt => opt.MapFrom(src => src.MinPlayersInDraw))
                .ForMember(dest => dest.MaxPlayersInDraw, opt => opt.MapFrom(src => src.MaxPlayersInDraw));
        }
    }
}
