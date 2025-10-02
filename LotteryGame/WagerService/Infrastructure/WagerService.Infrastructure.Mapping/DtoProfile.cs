namespace WagerService.Infrastructure.Mapping
{
    using AutoMapper;

    using WagerService.Core.Models;
    using WagerService.Data.Models;

    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.TicketNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
