namespace DrawService.Core.Operations
{
    using AutoMapper;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class HistoryOperations : IHistoryOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<Prize> prizeRepository;
        private readonly OperationPipeline<HistoryOperationContext> historyPipeline;

        public HistoryOperations(IMapper mapper, IRepository<Prize> prizeRepository, OperationPipeline<HistoryOperationContext> historyPipeline)
        {
            this.mapper = mapper;
            this.prizeRepository = prizeRepository;
            this.historyPipeline = historyPipeline;
        }

        public async Task<ResponseDto<HistoryDto>> GetHistory(string drawId)
        {
            var context = new HistoryOperationContext()
            {
                DrawId = drawId,
                Status = DrawStatus.Completed
            };

            var validationResult = await historyPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<HistoryDto>(validationResult.ErrorMsg);
            }

            Draw draw = context.Draw;

            IEnumerable<Prize> prizes = await prizeRepository.FindAsync(x => x.DrawId == drawId);

            HistoryDto historyDto = mapper.Map<HistoryDto>(draw);
            historyDto.Prizes = mapper.Map<IEnumerable<PrizeDto>>(prizes);

            return new ResponseDto<HistoryDto>() { Data = historyDto };
        }
    }
}
