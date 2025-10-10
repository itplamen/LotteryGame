namespace DrawService.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DrawService.Core.Contracts;
    using LotteryGame.Common.Models.Dto;

    public class StartDrawWorker : BaseWorker
    {
        private readonly IDrawOperations operations;

        public StartDrawWorker(IDrawOperations operations, IConfiguration configuration, ILogger<StartDrawWorker> logger)
            : base(configuration, logger)
        {
            this.operations = operations;
        }

        protected override async Task<IEnumerable<string>> GetDrawIds() => await operations.GetDrawsReadyToStart();

        protected override async Task<ResponseDto<IEnumerable<BaseDto>>> ProcessDraw(string drawId)
        {
            ResponseDto response = await operations.Start(drawId);

            return new ResponseDto<IEnumerable<BaseDto>>()
            {
                ErrorMsg = response.ErrorMsg,
                Data = new List<BaseDto>() { new BaseDto(drawId) }
            };
        }
    }
}
