namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawMustExistPolicy<TContext> : IOperationPolicy<TContext>
        where TContext : BaseDrawContext
    {
        private readonly IRepository<Draw> drawRepository;

        public DrawMustExistPolicy(IRepository<Draw> drawRepository)
        {
            this.drawRepository = drawRepository;
        }

        public async Task<ResponseDto> ExecuteAsync(TContext context)
        {
            if (!string.IsNullOrEmpty(context.DrawId))
            {
                var draw = await drawRepository.GetByIdAsync(context.DrawId);
                if (draw != null)
                {
                    context.Draw = draw;

                    return new ResponseDto();
                }
            }

            return new ResponseDto("Draw not found");
        }
    }
}
