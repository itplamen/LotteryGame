namespace DrawService.Workers
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public class SettlementWorker : BaseWorker
    {    
        private readonly IDrawOperations drawOperations;
        private readonly IPrizeOperations prizeOperations;

        public SettlementWorker(IDrawOperations drawOperations, IPrizeOperations prizeOperations, IConfiguration configuration, ILogger<SettlementWorker> logger)
            : base(configuration, logger)
        {
            this.drawOperations = drawOperations;
            this.prizeOperations = prizeOperations;
        }

        protected override async Task<IEnumerable<string>> GetDrawIds() => await drawOperations.GetDrawsForSettlement();

        protected override async Task<ResponseDto<IEnumerable<BaseDto>>> ProcessDraw(string drawId)
        {
            ResponseDto<IEnumerable<PrizeDto>> response = await prizeOperations.DeterminePrizes(drawId);

            return new ResponseDto<IEnumerable<BaseDto>>()
            {
                ErrorMsg = response.ErrorMsg,
                Data = response.Data
            };
        }
    }
}
