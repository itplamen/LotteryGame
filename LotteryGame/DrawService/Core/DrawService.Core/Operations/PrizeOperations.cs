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

    public class PrizeOperations : IPrizeOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<Draw> drawRepository;
        private readonly IRepository<Prize> prizeRepository;
        private readonly IPrizeDeterminationStrategy prizeDeterminationStrategy;
        private readonly OperationPipeline<PrizeOperationContext> operationPipeline;

        public PrizeOperations(
            IMapper mapper, 
            IRepository<Draw> drawRepository,
            IRepository<Prize> prizeRepository, 
            IPrizeDeterminationStrategy prizeDeterminationStrategy,
            OperationPipeline<PrizeOperationContext> operationPipeline)
        {
            this.mapper = mapper;
            this.drawRepository = drawRepository;
            this.prizeRepository = prizeRepository;
            this.prizeDeterminationStrategy = prizeDeterminationStrategy;
            this.operationPipeline = operationPipeline;
        }

        public async Task<ResponseDto<IEnumerable<PrizeDto>>> DeterminePrizes(string drawId)
        {
            var context = new PrizeOperationContext() 
            { 
                DrawId = drawId, 
                Status = DrawStatus.InProgress 
            };
            var validationResult = await operationPipeline.ExecuteAsync(context);

            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<IEnumerable<PrizeDto>>(validationResult.ErrorMsg);
            }

            var draw = context.Draw;

            IEnumerable<Prize> prizes = prizeDeterminationStrategy.DeterminePrizes(draw);
            IEnumerable<Prize> created = await prizeRepository.AddRangeAsync(prizes);

            draw.PrizeIds = created.Select(x => x.Id).ToList();
            draw.Status = DrawStatus.Completed;

            await drawRepository.UpdateAsync(draw);

            return new ResponseDto<IEnumerable<PrizeDto>>()
            {
                Data = mapper.Map<IEnumerable<PrizeDto>>(created)
            };
        }
    }
}
