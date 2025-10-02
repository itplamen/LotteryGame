namespace LotteryGame.Orchestrators.Services
{
    using System.Threading.Tasks;
   
    using AutoMapper;
    
    using Google.Protobuf.WellKnownTypes;
    
    using Grpc.Core;
    
    using LotteryGame.Orchestrators.Models.Protos.TicketPurchase;
    using WagerService.Api.Models.Protos.Tickets;
    using WalletService.Api.Models.Protos.Funds;

    public class TicketPurchaseOrchestrator : TicketPurchase.TicketPurchaseBase
    {
        private readonly IMapper mapper;
        private readonly Funds.FundsClient fundsClient;
        private readonly Tickets.TicketsClient wagerClient;

        public TicketPurchaseOrchestrator(IMapper mapper, Funds.FundsClient fundsClient, Tickets.TicketsClient wagerClient)
        {
            this.mapper = mapper;
            this.fundsClient = fundsClient;
            this.wagerClient = wagerClient;
        }

        public override async Task<PurchaseResponse> Purchase(PurchaseRequest request, ServerCallContext context)
        {
            PriceResponse pricePerTicket = await wagerClient.GetPriceAsync(new Empty());
            EnoughFundsRequest enoughFundsRequest = new EnoughFundsRequest()
            {
                PlayerId = request.PlayerId,
                CostAmount = pricePerTicket.Price * request.NumberOfTickets
            };

            BaseResponse enoughFundsResponse = await fundsClient.HasEnoughFundsAsync(enoughFundsRequest);

            if (!enoughFundsResponse.Success)
            {
                PurchaseResponse response = mapper.Map<PurchaseResponse>(enoughFundsResponse);
                return response;
            }

            for (int i = 1; i <= request.NumberOfTickets; i++)
            {

                await PurchaseTicket(request.PlayerId, pricePerTicket.Price);

            }


            return null; 
        }

        private async Task<PurchaseResponse> PurchaseTicket(int playerId, long amount)
        {
            ReserveRequest reserveRequest = new ReserveRequest()
            {
                PlayerId =  playerId,
                Amount = amount
            };

            ReserveResponse reserveResponse = await fundsClient.ReserveAsync(reserveRequest);

            if (!reserveResponse.Success)
            {
                PurchaseResponse response = mapper.Map<PurchaseResponse>(reserveResponse);
                return response;
            }

            // TODO: Get DrawId from Draw Service

            TicketCreateRequest ticketCreateRequest = new TicketCreateRequest()
            {
                PlayerId = playerId,
                DrawId = "1",
                ReservationId = reserveResponse.ReservationId
            };
            
            TicketResponse ticketResponse =  await wagerClient.CreateAsync(ticketCreateRequest);

            if (!ticketResponse.Success)
            {
                RefundRequest refundRequest = new RefundRequest() 
                { 
                    ReservationId = reserveResponse.ReservationId 
                };

                await fundsClient.RefundAsync(refundRequest);

                PurchaseResponse response = mapper.Map<PurchaseResponse>(ticketResponse);
                return response;
            }

            CaptureRequest captureRequest = new CaptureRequest()
            {
                ReservationId = reserveResponse.ReservationId,
                TicketId = ticketResponse.Ticket.Id   
            };

            BaseResponse captureResponse = await fundsClient.CaptureAsync(captureRequest);
            if (!captureResponse.Success)
            {
                await wagerClient.UpdateAsync(new TicketUpdateRequest()
                {
                    TicketId = ticketResponse.Ticket.Id,
                    Status = TicketStatus.Cancelled
                });

                RefundRequest refundRequest = new RefundRequest()
                {
                    ReservationId = reserveResponse.ReservationId
                };

                await fundsClient.RefundAsync(refundRequest);

                PurchaseResponse response = mapper.Map<PurchaseResponse>(captureResponse);
                return response;
            }

            await wagerClient.UpdateAsync(new TicketUpdateRequest()
            {
                TicketId = ticketResponse.Ticket.Id,
                Status = TicketStatus.Confirmed
            });


            return new PurchaseResponse();
        }
    }
}
    