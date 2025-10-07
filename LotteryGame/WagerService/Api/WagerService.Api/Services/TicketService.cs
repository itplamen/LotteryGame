namespace WagerService.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;
   
    using LotteryGame.Common.Models.Dto;
    using WagerService.Api.Models.Protos.Tickets;
    using WagerService.Core.Contracts;
    using WagerService.Core.Models;

    public class TicketService : Tickets.TicketsBase
    {
        private readonly IMapper mapper;
        private readonly ITicketOperations ticketOperations;

        public TicketService(IMapper mapper, ITicketOperations ticketOperations)
        {
            this.mapper = mapper;
            this.ticketOperations = ticketOperations;
        }

        public override async Task<TicketProtoResponse> Create(TicketCreateProtoRequest request, ServerCallContext context)
        {
            TicketCreateRequestDto requestDto = mapper.Map<TicketCreateRequestDto>(request);
            ResponseDto<IEnumerable<TicketDto>> responseDto = await ticketOperations.Create(requestDto);
            TicketProtoResponse response = mapper.Map<TicketProtoResponse>(responseDto);

            return response;
        }

        public override async Task<TicketProtoResponse> Update(TicketUpdateProtoRequest request, ServerCallContext context)
        {
            TicketUpdateRequestDto requestDto = mapper.Map<TicketUpdateRequestDto>(request);
            ResponseDto<IEnumerable<TicketDto>> responseDto = await ticketOperations.Update(requestDto);
            TicketProtoResponse response = mapper.Map<TicketProtoResponse>(responseDto);

            return response;
        }
    }
}
