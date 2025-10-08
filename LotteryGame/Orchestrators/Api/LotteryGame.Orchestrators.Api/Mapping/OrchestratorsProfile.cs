namespace LotteryGame.Orchestrators.Api.Mapping
{
    using AutoMapper;
   
    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using LotteryGame.Orchestrators.Models.ReserveFunds;
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
                .ForPath(dest => dest.Data.MinTicketsPerPlayer, opt => opt.MapFrom(src => src.MinTicketsPerPlayer))
                .ForPath(dest => dest.Data.MaxTicketsPerPlayer, opt => opt.MapFrom(src => src.MaxTicketsPerPlayer))
                .ForPath(dest => dest.Data.TicketPriceInCents, opt => opt.MapFrom(src => src.TicketPriceInCents));

            CreateMap<FetchDrawProtoResponse, OrchestratorResponse<DrawParticipationResponse>>()
                .ForPath(dest => dest.Data.DrawId, opt => opt.MapFrom(src => src.DrawId))
                .ForPath(dest => dest.Data.DrawDate, opt => opt.MapFrom(src => src.DrawDate.ToDateTime()));

            CreateMap<BaseProtoResponse, OrchestratorResponse<ReserveFundsResponse>>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

            CreateMap<ReserveProtoResponse, OrchestratorResponse<ReserveFundsResponse>>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForPath(dest => dest.Data.ReservationId, opt => opt.MapFrom(src => src.ReservationId));

            CreateMap<TicketProto, OrchestratorResponse<PurchaseTicket>>()
                .ForPath(dest => dest.Data.Id, opt => opt.MapFrom(src => src.Id))
                .ForPath(dest => dest.Data.Number, opt => opt.MapFrom(src => src.TicketNumber))
                .ForPath(dest => dest.Data.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<TicketProto, PurchaseTicket>()
                .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForPath(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForPath(dest => dest.Number, opt => opt.MapFrom(src => src.TicketNumber));

            CreateMap<TicketProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForPath(dest => dest.Data.Tickets, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<BaseProtoResponse, OrchestratorResponse<PurchaseTicketsResponse>>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

            CreateMap<PurchaseRequest, OrchestratorRequest<ReserveFundsRequest>>()
                .ForPath(dest => dest.Payload.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForPath(dest => dest.Payload.NumberOfTickets, opt => opt.MapFrom(src => src.NumberOfTickets));

            CreateMap<PurchaseRequest, OrchestratorRequest<PurchaseTicketsRequest>>()
                .ForPath(dest => dest.Payload.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForPath(dest => dest.Payload.NumberOfTickets, opt => opt.MapFrom(src => src.NumberOfTickets));

            CreateMap<PurchaseTicketsRequest, DrawParticipationRequest>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.DrawId, opt => opt.MapFrom(src => src.DrawId));

            CreateMap<OrchestratorRequest<PurchaseTicketsRequest>, OrchestratorRequest<DrawParticipationRequest>>()
                .ConstructUsing((src, ctx) =>
                    new OrchestratorRequest<DrawParticipationRequest>(
                        ctx.Mapper.Map<DrawParticipationRequest>(src.Payload)
                    )
                );
        }
    }
}
