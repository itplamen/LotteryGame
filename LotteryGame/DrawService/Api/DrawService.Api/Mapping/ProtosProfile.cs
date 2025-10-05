namespace DrawService.Api.Mapping
{
    using AutoMapper;
    
    using Google.Protobuf.WellKnownTypes;
    
    using DrawService.Api.Models.Protos.Draws;
    using DrawService.Core.Models;
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
        }
    }
}
