namespace WagerService.Api.Mappping
{
    using AutoMapper;
    
    using WagerService.Api.Models.Protos.Tickets;
    using WagerService.Core.Models;

    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.TicketNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
