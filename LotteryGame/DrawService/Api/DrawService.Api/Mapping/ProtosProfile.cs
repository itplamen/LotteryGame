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
                .ForMember(dest => dest.DrawDate, opt => opt.Ignore())
                .ForMember(dest => dest.DrawId, opt => opt.Ignore())
                .ForMember(dest => dest.TicketPriceInCents, opt => opt.Ignore())
                .ForMember(dest => dest.MinTicketsPerPlayer, opt => opt.Ignore())
                .ForMember(dest => dest.MaxTicketsPerPlayer, opt => opt.Ignore())
                .ForMember(dest => dest.MinPlayersInDraw, opt => opt.Ignore())
                .ForMember(dest => dest.MaxPlayersInDraw, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentPlayersInDraw, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.Data != null)
                    {
                        dest.DrawId = src.Data.Id;
                        dest.TicketPriceInCents = src.Data.TicketPriceInCents;
                        dest.MinTicketsPerPlayer = src.Data.MinTicketsPerPlayer;
                        dest.MaxTicketsPerPlayer = src.Data.MaxTicketsPerPlayer;
                        dest.MinPlayersInDraw = src.Data.MinPlayersInDraw;
                        dest.MaxPlayersInDraw = src.Data.MaxPlayersInDraw;
                        dest.CurrentPlayersInDraw = src.Data.CurrentPlayersInDraw;
                        dest.Status = src.Data.Status switch
                        {
                            DrawStatus.Pending => DrawStatusProto.Pending,
                            DrawStatus.InProgress => DrawStatusProto.InProgress,
                            DrawStatus.Completed => DrawStatusProto.Completed,
                            DrawStatus.Cancelled => DrawStatusProto.Cancelled,
                            _ => throw new ArgumentOutOfRangeException("Invalid draw status")
                        };
                        dest.DrawDate = Timestamp.FromDateTime(src.Data.DrawDate.ToUniversalTime());
                    }
                    else
                    {
                        dest.DrawId = string.Empty;
                        dest.TicketPriceInCents = 0;
                        dest.MinTicketsPerPlayer = 0;
                        dest.MaxTicketsPerPlayer = 0;
                        dest.MinPlayersInDraw = 0;
                        dest.MaxPlayersInDraw = 0;
                        dest.CurrentPlayersInDraw = 0;
                        dest.Status = DrawStatusProto.Pending;
                        dest.DrawDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    }
                });
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
