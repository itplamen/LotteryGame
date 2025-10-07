namespace LotteryGame.Orchestrators.Api.Mapping
{
    using AutoMapper;
   
    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using WagerService.Api.Models.Protos.Tickets;
    using WalletService.Api.Models.Protos.Funds;

    public class OrchestratorsProfile : Profile
    {
        public OrchestratorsProfile()
        {
            CreateMap<FetchDrawProtoResponse, OrchestratorResponse<AvailableDrawResponse>>()
                .ForMember(dest => dest.Data.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForMember(dest => dest.Data.DrawDate, opt => opt.MapFrom(src => src.DrawDate.ToDateTime()))
                .ForMember(dest => dest.Data.CurrentPlayersInDraw, opt => opt.MapFrom(src => src.CurrentPlayersInDraw))
                .ForMember(dest => dest.Data.MinPlayersInDraw, opt => opt.MapFrom(src => src.MinPlayersInDraw))
                .ForMember(dest => dest.Data.MaxPlayersInDraw, opt => opt.MapFrom(src => src.MaxPlayersInDraw))
                .ForMember(dest => dest.Data.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer))
                .ForMember(dest => dest.Data.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer));

            CreateMap<FetchDrawProtoResponse, OrchestratorResponse<DrawParticipationResponse>>()
                    .ForMember(dest => dest.Data.DrawId, opt => opt.MapFrom(src => src.DrawId))
                    .ForMember(dest => dest.Data.DrawDate, opt => opt.MapFrom(src => src.DrawDate.ToDateTime()));

            CreateMap<TicketProto, OrchestratorResponse<PurchaseTicket>>()
                    .ForMember(dest => dest.Data.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Data.Number, opt => opt.MapFrom(src => src.TicketNumber))
                    .ForMember(dest => dest.Data.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<TicketProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                    .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                    .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                    .ForMember(dest => dest.Data.Tickets, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<BaseProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                   .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                   .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

        }
    }
}
