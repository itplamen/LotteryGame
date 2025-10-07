namespace DrawService.Core.Operations
{
    using AutoMapper;
    
    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;

    public class PrizeOperations : IPrizeOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<Draw> drawRepository;
        private readonly IRepository<Prize> prizeRepository;
        private readonly IPrizeDeterminationStrategy prizeDeterminationStrategy;

        public PrizeOperations(IMapper mapper, IRepository<Draw> drawRepository, IRepository<Prize> prizeRepository, IPrizeDeterminationStrategy prizeDeterminationStrategy)
        {
            this.mapper = mapper;
            this.drawRepository = drawRepository;
            this.prizeRepository = prizeRepository;
            this.prizeDeterminationStrategy = prizeDeterminationStrategy;
        }

        public async Task<ResponseDto<IEnumerable<PrizeDto>>> DeterminePrizes(string drawId)
        {
            Draw draw = await drawRepository.GetByIdAsync(drawId);

            if (draw == null)
            {
                return new ResponseDto<IEnumerable<PrizeDto>>("Draw not found");
            }

            if (draw.Status != DrawStatus.InProgress)
            {
                return new ResponseDto<IEnumerable<PrizeDto>>("Invalid draw status");
            }

            if (draw.PlayerTickets == null || !draw.PlayerTickets.Any())
            {
                return new ResponseDto<IEnumerable<PrizeDto>>("No tickets for draw");
            }

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
