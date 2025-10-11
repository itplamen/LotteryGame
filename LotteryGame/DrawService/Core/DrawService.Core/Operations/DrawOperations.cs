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
        private readonly OperationPipeline<JoinDrawOperationContext> joinDrawPipeline;
        private readonly OperationPipeline<StartDrawOperationContext> startDrawPipeline;

        public DrawOperations(
            IMapper mapper, 
            IRepository<Draw> repository, 
            IConfiguration configuration,
            OperationPipeline<JoinDrawOperationContext> joinDrawPipeline,
            OperationPipeline<StartDrawOperationContext> startDrawPipeline)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.ticketPriceInCents = long.Parse(configuration["TicketPriceInCents"]);
            this.drawScheduleTime = int.Parse(configuration["DrawScheduleTime"]);
            this.minTicketsPerPlayer = int.Parse(configuration["MinTicketsPerPlayer"]);
            this.maxTicketsPerPlayer = int.Parse(configuration["MaxTicketsPerPlayer"]);
            this.minPlayersInDraw = int.Parse(configuration["MinPlayersInDraw"]);
            this.maxPlayersInDraw = int.Parse(configuration["MaxPlayersInDraw"]);
            this.joinDrawPipeline = joinDrawPipeline;
            this.startDrawPipeline = startDrawPipeline;
        }

        public Task<ResponseDto<DrawDto>> GetDrawOptions()
        {
            var dto = CreateDrawDto();
            return Task.FromResult(new ResponseDto<DrawDto> { Data = dto });
        }

        public async Task<ResponseDto<DrawDto>> GetOpenDraw(int playerId)
        {
            IEnumerable<Draw> openDraws = await repository.FindAsync(x => 
                x.Status == DrawStatus.Pending &&
                !x.PlayerTickets.Any(y => y.PlayerId == playerId));

            if (!openDraws.Any())
            {
                return new ResponseDto<DrawDto>("No open draws available");
            }

            DrawDto dto = CreateDrawDto(openDraws.First());
            return new ResponseDto<DrawDto>() { Data = dto };
        }

        public async Task<IEnumerable<string>> GetDrawsReadyToStart()
        {
            IEnumerable<Draw> draws = await repository.FindAsync(x =>
                x.DrawDate <= DateTime.UtcNow &&
                x.Status == DrawStatus.Pending);

            return draws?.Where(x => 
                x.CurrentPlayersInDraw >= x.MinPlayersInDraw && 
                x.CurrentPlayersInDraw <= x.MaxPlayersInDraw)?
                .Select(x => x.Id)?.ToList() ?? new List<string>();
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
            var context = new JoinDrawOperationContext() 
            {
                DrawId = drawId,
                PlayerId = playerId,
                Status = DrawStatus.Pending,
                TicketIds = ticketIds,
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer
            };

            var validationResult = await joinDrawPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<DrawDto>(validationResult.ErrorMsg);
            }

            Draw draw = context.Draw;
            draw.PlayerTickets.Add(new PlayerTicketInfo() { PlayerId = playerId, Tickets = ticketIds.ToList() });

            await repository.UpdateAsync(draw);

            return new ResponseDto<DrawDto>() { Data = mapper.Map<DrawDto>(draw) };
        }

        public async Task<ResponseDto> Start(string drawId)
        {
            var context = new StartDrawOperationContext() 
            {
                DrawId = drawId,
                Status = DrawStatus.Pending,
                MinTicketsPerPlayer = minTicketsPerPlayer,
                MaxTicketsPerPlayer = maxTicketsPerPlayer
            };

            var validationResult = await startDrawPipeline.ExecuteAsync(context);
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

        private DrawDto CreateDrawDto(Draw draw = null)
        {
            DrawDto dto = draw != null ? mapper.Map<DrawDto>(draw) : new DrawDto();
            dto.TicketPriceInCents = ticketPriceInCents;
            dto.DrawDate = DateTime.UtcNow.AddMilliseconds(drawScheduleTime);
            dto.MinTicketsPerPlayer = minTicketsPerPlayer;
            dto.MaxTicketsPerPlayer = maxTicketsPerPlayer;
            dto.MinPlayersInDraw = minPlayersInDraw;
            dto.MaxPlayersInDraw = maxPlayersInDraw;

            return dto;
        }
    }
}
