namespace LotteryGame.Orchestrators.Services
{
    using System.Threading.Tasks;
   
    using AutoMapper;
    using DrawService.Api.Models.Protos.Draws;
    
    using Grpc.Core;
    
    using LotteryGame.Orchestrators.Models.Protos.TicketPurchase;
    using WagerService.Api.Models.Protos.Tickets;
    using WalletService.Api.Models.Protos.Funds;

    public class TicketPurchaseOrchestrator : TicketPurchase.TicketPurchaseBase
    {
        private readonly int maxRetries;
        private readonly int retryDelayMs;
        private readonly IMapper mapper;
        private readonly Funds.FundsClient fundsClient;
        private readonly Tickets.TicketsClient wagerClient;
        private readonly Draws.DrawsClient drawClient;

        public TicketPurchaseOrchestrator(
            IMapper mapper, 
            Funds.FundsClient fundsClient, 
            Tickets.TicketsClient wagerClient, 
            Draws.DrawsClient drawClient, 
            IConfiguration configuration)
        {
            this.mapper = mapper;
            this.fundsClient = fundsClient;
            this.wagerClient = wagerClient;
            this.drawClient = drawClient;
            this.maxRetries = int.Parse(configuration["MaxRetries"]);
            this.retryDelayMs = int.Parse(configuration["RetryDelayMs"]);
        }

        public override async Task<PurchaseResponse> Purchase(PurchaseRequest request, ServerCallContext context)
        {
            FetchDrawRequest fetchDrawRequest = new FetchDrawRequest()
            {
                PlayerId = request.PlayerId
            };

            FetchDrawResponse fetchDrawResponse = await drawClient.FetchDrawAsync(fetchDrawRequest);
            if (!fetchDrawResponse.Success)
            {
                return mapper.Map<PurchaseResponse>(fetchDrawResponse);
            }

            if (request.NumberOfTickets < fetchDrawResponse.MinTicketsPerPlayer ||
                request.NumberOfTickets > fetchDrawResponse.MaxTicketsPerPlayer)
            {
                return new PurchaseResponse()
                {
                    Success = false,
                    ErrorMsg = $"Number of tickets must be between {fetchDrawResponse.MinTicketsPerPlayer} and {fetchDrawResponse.MaxTicketsPerPlayer}"
                };
            }

            ReserveResponse reserveFundsResponse = await ReserveFuns(request, fetchDrawResponse.TicketPriceInCents);
            if (!reserveFundsResponse.Success)
            {
                return mapper.Map<PurchaseResponse>(reserveFundsResponse); ;
            }

            return await PurchaseTickets(request, fetchDrawResponse.DrawId, reserveFundsResponse.ReservationId);
        }

        private async Task<ReserveResponse> ReserveFuns(PurchaseRequest request, long ticketPriceInCents)
        {
            EnoughFundsRequest enoughFundsRequest = new EnoughFundsRequest()
            {
                PlayerId = request.PlayerId,
                CostAmount = ticketPriceInCents * request.NumberOfTickets
            };

            BaseResponse enoughFundsResponse = await fundsClient.HasEnoughFundsAsync(enoughFundsRequest);

            if (!enoughFundsResponse.Success)
            {
                return mapper.Map<ReserveResponse>(enoughFundsResponse);
            }

            ReserveRequest reserveRequest = new ReserveRequest()
            {
                PlayerId = request.PlayerId,
                Amount = enoughFundsRequest.CostAmount
            };

            return await fundsClient.ReserveAsync(reserveRequest);
        }

        private async Task<PurchaseResponse> PurchaseTickets(PurchaseRequest request, string drawId, int reservationId)
        {
            TicketCreateRequest ticketCreateRequest = new TicketCreateRequest()
            {
                PlayerId = request.PlayerId,
                DrawId = drawId,
                ReservationId = reservationId,
                NumberOfTickets = request.NumberOfTickets
            };

            TicketResponse createdTicketResponse = await wagerClient.CreateAsync(ticketCreateRequest);
            if (!createdTicketResponse.Success)
            {
                return await CompensateFunds(reservationId);
            }

            BaseResponse captureResponse = await fundsClient.CaptureAsync(new FundsRequest() { ReservationId = reservationId });
            if (!captureResponse.Success)
            {
                await CancelTickets(createdTicketResponse, reservationId);
                return await CompensateFunds(reservationId);
            }

            var ticketUpdateRequest = new TicketUpdateRequest()
            {
                Status = TicketStatus.Confirmed
            };
            ticketUpdateRequest.TicketIds.AddRange(createdTicketResponse.Tickets.Select(x => x.Id));

            TicketResponse confirmResponse = await RetryAsync(async () => await wagerClient.UpdateAsync(ticketUpdateRequest));

            if (!confirmResponse.Success)
            {
                return new PurchaseResponse()
                {
                    Success = false,
                    ErrorMsg = "Could not purchase tickets"
                };
            }

            return new PurchaseResponse() { Success = true };
        }

        private async Task CancelTickets(TicketResponse ticketResponse, int reservationId)
        {
            var ticketUpdateRequest = new TicketUpdateRequest
            {
                Status = TicketStatus.Cancelled
            };
            ticketUpdateRequest.TicketIds.AddRange(ticketResponse.Tickets.Select(x => x.Id));

            var updateResponse = await RetryAsync(async () => await wagerClient.UpdateAsync(ticketUpdateRequest));
            if (!updateResponse.Success)
            {
                // log & alert — orphan tickets exist
            }
        }

        private async Task<PurchaseResponse> CompensateFunds(int reservationId)
        {
            FundsRequest fundsRequest = new FundsRequest()
            {
                ReservationId = reservationId
            };

            BaseResponse refundResponse = await RetryAsync(async () => await fundsClient.RefundAsync(fundsRequest));

            if (refundResponse.Success)
            {
                return new PurchaseResponse() { Success = true };
            }

            // TODO: Log refundResponse

            return new PurchaseResponse() { Success = false, ErrorMsg = "Ticket purchase failed" };
        }

        private async Task<T> RetryAsync<T>(Func<Task<T>> action)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch
                {
                    if (i == maxRetries - 1) throw;
                    await Task.Delay(retryDelayMs * (i + 1)); 
                }
            }

            throw new InvalidOperationException("Retry failed");
            // TODO: log to orchestrator DB
        }
    }
}
    