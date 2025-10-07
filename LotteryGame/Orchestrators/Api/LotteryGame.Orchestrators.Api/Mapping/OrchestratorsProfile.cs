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
                .ForPath(dest => dest.Data.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForPath(dest => dest.Data.DrawDate, opt => opt.MapFrom(src => src.DrawDate.ToDateTime()))
                .ForPath(dest => dest.Data.CurrentPlayersInDraw, opt => opt.MapFrom(src => src.CurrentPlayersInDraw))
                .ForPath(dest => dest.Data.MinPlayersInDraw, opt => opt.MapFrom(src => src.MinPlayersInDraw))
                .ForPath(dest => dest.Data.MaxPlayersInDraw, opt => opt.MapFrom(src => src.MaxPlayersInDraw))
                .ForPath(dest => dest.Data.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer))
                .ForPath(dest => dest.Data.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer));

            CreateMap<FetchDrawProtoResponse, OrchestratorResponse<DrawParticipationResponse>>()
                    .ForPath(dest => dest.Data.DrawId, opt => opt.MapFrom(src => src.DrawId))
                    .ForPath(dest => dest.Data.DrawDate, opt => opt.MapFrom(src => src.DrawDate.ToDateTime()));

            CreateMap<TicketProto, OrchestratorResponse<PurchaseTicket>>()
                    .ForPath(dest => dest.Data.Id, opt => opt.MapFrom(src => src.Id))
                    .ForPath(dest => dest.Data.Number, opt => opt.MapFrom(src => src.TicketNumber))
                    .ForPath(dest => dest.Data.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<TicketProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                    .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                    .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                    .ForPath(dest => dest.Data.Tickets, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<BaseProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                   .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                   .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

        }
    }
}
