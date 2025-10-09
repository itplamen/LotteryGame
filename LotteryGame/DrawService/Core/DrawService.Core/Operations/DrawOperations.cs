namespace DrawService.Core.Operations
{
    using Microsoft.Extensions.Configuration;

    using AutoMapper;
    
    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawOperations : IDrawOperations
    {
        private readonly long ticketPriceInCents;
        private readonly int drawScheduleTime;
        private readonly int minTicketsPerPlayer;
        private readonly int maxTicketsPerPlayer;
        private readonly int minPlayersInDraw;
        private readonly int maxPlayersInDraw;
        private readonly IMapper mapper;
        private readonly IRepository<Draw> repository;
        private readonly OperationPipeline<DrawOperationContext> operationPipeline;

        public DrawOperations(
            IMapper mapper, 
            IRepository<Draw> repository, 
            IConfiguration configuration,
            OperationPipeline<DrawOperationContext> operationPipeline)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.ticketPriceInCents = long.Parse(configuration["TicketPriceInCents"]);
            this.drawScheduleTime = int.Parse(configuration["DrawScheduleTime"]);
            this.minTicketsPerPlayer = int.Parse(configuration["MinTicketsPerPlayer"]);
            this.maxTicketsPerPlayer = int.Parse(configuration["MaxTicketsPerPlayer"]);
            this.minPlayersInDraw = int.Parse(configuration["MinPlayersInDraw"]);
            this.maxPlayersInDraw = int.Parse(configuration["MaxPlayersInDraw"]);
            this.operationPipeline = operationPipeline;
        }

        public async Task<ResponseDto<DrawDto>> GetOpenDraw(int playerId)
        {
            IEnumerable<Draw> openDraws = await repository.FindAsync(x => x.Status == DrawStatus.Pending);
            openDraws = openDraws.Where(x => !x.PlayerTickets.ContainsKey(playerId));

            if (!openDraws.Any())
            {
                return new ResponseDto<DrawDto>("No open draws available");
            }

            DrawDto openDrawDto = mapper.Map<DrawDto>(openDraws.First());
            openDrawDto.MinTicketsPerPlayer = minTicketsPerPlayer;
            openDrawDto.MaxTicketsPerPlayer = maxTicketsPerPlayer;
            openDrawDto.MinPlayersInDraw = minPlayersInDraw;
            openDrawDto.MaxPlayersInDraw = maxPlayersInDraw;

            return new ResponseDto<DrawDto>() { Data = openDrawDto };
        }

        public async Task<IEnumerable<string>> GetDrawsForSettlement()
        {
            IEnumerable<Draw> draws = await repository.FindAsync(x => x.Status == DrawStatus.InProgress);

            return draws?.Select(x => x.Id)?.ToList() ?? new List<string>();
        } 

        public async Task<ResponseDto<DrawDto>> Create()
        { 
            var draw = new Draw()
            {
                Status = DrawStatus.Pending,
                TicketPriceInCents = ticketPriceInCents,
                DrawDate = DateTime.UtcNow.AddMilliseconds(drawScheduleTime),
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer,
                MinPlayersInDraw = minPlayersInDraw,
                MaxPlayersInDraw = maxPlayersInDraw
            };

            await repository.AddAsync(draw);

            return new ResponseDto<DrawDto>() { Data = mapper.Map<DrawDto>(draw) };
        }

        public async Task<ResponseDto<DrawDto>> Join(string drawId, int playerId, IEnumerable<string> ticketIds)
        {
            var context = new DrawOperationContext() 
            {
                DrawId = drawId,
                PlayerId = playerId,
                Status = DrawStatus.Pending,
                TicketIds = ticketIds,
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer,
                Join = true
            };

            var validationResult = await operationPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<DrawDto>(validationResult.ErrorMsg);
            }

            Draw draw = context.Draw;
            draw.PlayerTickets[playerId] = ticketIds.ToList();

            await repository.UpdateAsync(draw);

            return new ResponseDto<DrawDto>() { Data = mapper.Map<DrawDto>(draw) };
        }

        public async Task<ResponseDto> Start(string drawId)
        {
            var context = new DrawOperationContext() 
            {
                DrawId = drawId,
                Status = DrawStatus.Pending,
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer,
                Start = true
            };

            var validationResult = await operationPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            Draw draw = context.Draw;

            draw.Status = DrawStatus.InProgress;
            draw.DrawDate = DateTime.UtcNow;

            await repository.UpdateAsync(draw);

            return new ResponseDto();
        }
    }
}
