namespace WagerService.Api.Mappping
{
    using AutoMapper;

    using LotteryGame.Common.Models.Dto;
    using WagerService.Api.Models.Protos.Tickets;
    using WagerService.Core.Models;
    using WagerService.Data.Models;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<TicketCreateProtoRequest, TicketCreateRequestDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId))
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.NumberOfTickets, opt => opt.MapFrom(src => src.NumberOfTickets))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId));

            CreateMap<TicketDto, TicketProto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.TicketNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapTicketStatus(src.Status)));

            CreateMap<IEnumerable<TicketDto>, TicketProtoResponse>()
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src));

            CreateMap<ResponseDto<IEnumerable<TicketDto>>, TicketProtoResponse>()
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

            CreateMap<TicketUpdateProtoRequest, TicketUpdateRequestDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TicketStatus)src.Status))
                .ForMember(dest => dest.TicketIds, opt => opt.MapFrom(src => src.TicketIds.ToList()));
        }

        private static TicketStatusProto MapTicketStatus(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Confirmed => TicketStatusProto.Confirmed,
                TicketStatus.Pending => TicketStatusProto.Pending,
                TicketStatus.Cancelled => TicketStatusProto.Cancelled,
                TicketStatus.Settled => TicketStatusProto.Settled,
                _ => throw new ArgumentOutOfRangeException("Invalid ticket status")
            };
        }
    }
}
