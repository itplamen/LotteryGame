namespace WagerService.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;
    
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

        public override async Task<TicketResponse> Create(TicketCreateRequest request, ServerCallContext context)
        {
            TicketCreateRequestDto requestDto = mapper.Map<TicketCreateRequestDto>(request);
            ResponseDto<TicketDto> responseDto = await ticketOperations.Create(requestDto);
            TicketResponse response = mapper.Map<TicketResponse>(responseDto);

            return response;
        }

        public override async Task<TicketResponse> Update(TicketUpdateRequest request, ServerCallContext context)
        {
            TicketUpdateRequestDto requestDto = mapper.Map<TicketUpdateRequestDto>(request);
            ResponseDto<TicketDto> responseDto = await ticketOperations.Update(requestDto);
            TicketResponse response = mapper.Map<TicketResponse>(responseDto);

            return response;
        }
    }
}
