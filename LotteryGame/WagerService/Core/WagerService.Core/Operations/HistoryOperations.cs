namespace WagerService.Core.Operations
{
    using AutoMapper;

    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WagerService.Core.Contracts;
    using WagerService.Core.Models;
    using WagerService.Core.Validation.Contexts;

    public class HistoryOperations : IHistoryOperations
    {
        private readonly IMapper mapper;
        private readonly OperationPipeline<BaseTicketOperationContext> operationPipeline;

        public HistoryOperations(IMapper mapper, OperationPipeline<BaseTicketOperationContext> operationPipeline)
        {
            this.mapper = mapper;
            this.operationPipeline = operationPipeline;
        }

        public async Task<ResponseDto<IEnumerable<TicketDto>>> GetHistory(IEnumerable<string> ticketIds)
        {
            var context = new BaseTicketOperationContext()
            {
                TicketIds = ticketIds
            };

            ResponseDto validationResult = await operationPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<IEnumerable<TicketDto>> { ErrorMsg = validationResult.ErrorMsg };
            }

            return new ResponseDto<IEnumerable<TicketDto>>()
            {
                Data = mapper.Map<IEnumerable<TicketDto>>(context.Tickets)
            };
        }
    }
}
