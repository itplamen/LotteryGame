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
                .ForMember(dest => dest.AmountInCents, opt => opt.MapFrom(src => src.AmountInCents))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId));

            CreateMap<Draw, DrawDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TicketPriceInCents, opt => opt.MapFrom(src => src.TicketPriceInCents))
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.MinTicketsPerPlayer))
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer))
                .ForMember(dest => dest.MinPlayersInDraw, opt => opt.MapFrom(src => src.MinPlayersInDraw))
                .ForMember(dest => dest.MaxPlayersInDraw, opt => opt.MapFrom(src => src.MaxPlayersInDraw))
                .ForMember(dest => dest.CurrentPlayersInDraw, opt => opt.MapFrom(src => src.CurrentPlayersInDraw));

            CreateMap<Draw, HistoryDto>()
                .ForMember(dest => dest.DrawDate, opt => opt.MapFrom(src => src.DrawDate))
                .ForMember(dest => dest.DrawStatus, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.CurrentPlayersInDraw))
                .ForMember(dest => dest.HouseProfitInCents, opt => opt.MapFrom(src => src.HouseProfitInCents));
        }
    }
}
