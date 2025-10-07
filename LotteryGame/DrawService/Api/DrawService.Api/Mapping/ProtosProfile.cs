namespace DrawService.Api.Mapping
{
    using AutoMapper;

    using Google.Protobuf.WellKnownTypes;

    using DrawService.Api.Models.Protos.Draws;
    using DrawService.Api.Models.Protos.Prizes;
    using DrawService.Core.Models;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<ResponseDto<DrawDto>, FetchDrawProtoResponse>()
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.Data.Id))
                .ForMember(dest => dest.TicketPriceInCents, opt => opt.MapFrom(src => src.Data.TicketPriceInCents))
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MinTicketsPerPlayer))
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MaxTicketsPerPlayer))
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MinTicketsPerPlayer))
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.Data.MaxTicketsPerPlayer))
                .ForMember(dest => dest.MinPlayersInDraw, opt => opt.MapFrom(src => src.Data.MinPlayersInDraw))
                .ForMember(dest => dest.MaxPlayersInDraw, opt => opt.MapFrom(src => src.Data.MaxPlayersInDraw))
                .ForMember(dest => dest.CurrentPlayersInDraw, opt => opt.MapFrom(src => src.Data.CurrentPlayersInDraw))
                .ForMember(dest => dest.DrawDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Data.DrawDate.ToUniversalTime())));

            CreateMap<ResponseDto, DrawProtoResponse>()
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess));

            CreateMap<Prize, PrizeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<PrizeDto, PrizeProto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<ResponseDto<IEnumerable<PrizeDto>>, DeterminePrizeProtoResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.Prizes, opt => opt.MapFrom(src => src.Data));
        }
    }
}
