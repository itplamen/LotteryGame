namespace DrawService.Core.Operations
{
    using Microsoft.Extensions.Configuration;

    using AutoMapper;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;

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

        public DrawOperations(IMapper mapper, IRepository<Draw> repository, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.ticketPriceInCents = long.Parse(configuration["TicketPriceInCents"]);
            this.drawScheduleTime = int.Parse(configuration["DrawScheduleTime"]);
            this.minTicketsPerPlayer = int.Parse(configuration["MinTicketsPerPlayer"]);
            this.maxTicketsPerPlayer = int.Parse(configuration["MaxTicketsPerPlayer"]);
            this.minPlayersInDraw = int.Parse(configuration["MinPlayersInDraw"]);
            this.maxPlayersInDraw = int.Parse(configuration["MaxPlayersInDraw"]);
        }

        public async Task<ResponseDto<DrawDto>> GetOpenDraw(int playerId)
        {
            IEnumerable<Draw> openDraws = await repository.FindAsync(x =>
                x.Status == DrawStatus.Pending && 
                !x.PlayerTickets.ContainsKey(playerId));

            if (!openDraws.Any())
            {
                return new ResponseDto<DrawDto>("No open draws available");
            }

            DrawDto openDrawDto = mapper.Map<DrawDto>(openDraws.First());
            openDrawDto.MinTicketsPerPlayer = minTicketsPerPlayer;
            openDrawDto.MaxTicketsPerPlayer = maxTicketsPerPlayer;

            return new ResponseDto<DrawDto>() { Data = openDrawDto };
        }

        public async Task<ResponseDto<DrawDto>> Create()
        { 
            var draw = new Draw()
            {
                Status = DrawStatus.Pending,
                TicketPriceInCents = ticketPriceInCents,
                DrawDate = DateTime.UtcNow.AddMilliseconds(drawScheduleTime)
            };

            await repository.AddAsync(draw);

            return mapper.Map<ResponseDto<DrawDto>>(draw);
        }

        public async Task<ResponseDto> Join(string drawId, int playerId, IEnumerable<string> ticketIds)
        {
            Draw draw = await repository.GetByIdAsync(drawId);
            if (draw == null)
            {
                return new ResponseDto("Draw not found");
            }

            if (draw.Status != DrawStatus.InProgress)
            {
                return new ResponseDto("Draw not started");
            }

            if (draw.PlayerTickets.ContainsKey(playerId))
            {
                return new ResponseDto("Player already joined the draw");
            }

            if (ticketIds.Count() < minTicketsPerPlayer || ticketIds.Count() > maxTicketsPerPlayer)
            {
                return new ResponseDto($"Invalid number of tickets. Min: {minTicketsPerPlayer}, Max: {maxTicketsPerPlayer}");
            }

            if (draw.PlayerTickets.Count == maxPlayersInDraw)
            {
                return new ResponseDto("Draw is full");
            }

            draw.PlayerTickets[playerId] = ticketIds.ToList();

            await repository.UpdateAsync(draw);

            return mapper.Map<ResponseDto>(draw);
        }

        public async Task<ResponseDto> Start(string drawId)
        {
            Draw draw = await repository.GetByIdAsync(drawId);
            if (draw == null)
            {
                return new ResponseDto("Draw not found");
            }

            if (draw.Status != DrawStatus.Pending)
            {
                return new ResponseDto("Invalid draw status");
            }

            if (draw.PlayerTickets.Count < minPlayersInDraw)
            {
                return new ResponseDto("Draw cannot be started");
            }

            draw.Status = DrawStatus.InProgress;
            draw.DrawDate = DateTime.UtcNow;

            await repository.UpdateAsync(draw);

            return mapper.Map<ResponseDto>(draw);
        }
    }
}
