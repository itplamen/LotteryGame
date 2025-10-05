namespace DrawService.Api.Mapping
{
    using AutoMapper;
    
    using DrawService.Api.Models.Protos.Draws;
    using DrawService.Api.Models.Protos.Prizes;
    using DrawService.Core.Models;
    
    using Google.Protobuf.WellKnownTypes;
    using LotteryGame.Common.Models.Dto;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<ResponseDto<DrawDto>, FetchDrawResponse>()
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.Data.Id))
                .ForMember(dest => dest.TicketPriceInCents, opt => opt.MapFrom(src => src.Data.TicketPriceInCents))
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MinTicketsPerPlayer))
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MaxTicketsPerPlayer))
                .ForMember(dest => dest.DrawDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Data.DrawDate.ToUniversalTime())));

            CreateMap<ResponseDto, DrawResponse>()
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess));

            CreateMap<Data.Models.Prize, PrizeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<PrizeDto, Models.Protos.Prizes.Prize>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<ResponseDto<IEnumerable<PrizeDto>>, DeterminePrizeResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Prizes, opt => opt.MapFrom(src => src.Data));
        }
    }
}
