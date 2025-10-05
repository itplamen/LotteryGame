namespace WagerService.Api.Mappping
{
    using AutoMapper;
    
    using WagerService.Api.Models.Protos.Tickets;
    using WagerService.Core.Models;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<TicketCreateRequest, TicketCreateRequestDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId))
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.NumberOfTickets, opt => opt.MapFrom(src => src.NumberOfTickets))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId));

            CreateMap<TicketDto, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.TicketNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapTicketStatus(src.Status)));

            CreateMap<IEnumerable<TicketDto>, TicketResponse>()
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src));
        }

        private static TicketStatus MapTicketStatus(Data.Models.TicketStatus status)
        {
            return status switch
            {
                Data.Models.TicketStatus.Confirmed => TicketStatus.Confirmed,
                Data.Models.TicketStatus.Pending => TicketStatus.Pending,
                Data.Models.TicketStatus.Cancelled => TicketStatus.Cancelled,
                Data.Models.TicketStatus.Settled => TicketStatus.Settled,
                _ => throw new ArgumentOutOfRangeException("Invalid ticket status")
            };
        }
    }
}
