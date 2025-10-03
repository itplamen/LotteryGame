namespace DrawService.Core.Operations
{
    using Microsoft.Extensions.Configuration;

    using AutoMapper;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    
    public class DrawOperations : IDrawOperations
    {
        private readonly int drawDays;
        private readonly IMapper mapper;
        private readonly IRepository<Draw> repository;

        public DrawOperations(IMapper mapper, IRepository<Draw> repository, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.drawDays = int.Parse(configuration["DrawDays"]);
        }

        public async Task<ResponseDto<DrawDto>> Create()
        { 
            var draw = new Draw()
            {
                DrawDate = DateTime.UtcNow.AddDays(drawDays),
                Status = DrawStatus.Pending,
            };

            await repository.AddAsync(draw);

            return mapper.Map<ResponseDto<DrawDto>>(draw);
        }

        public async Task<ResponseDto<DrawDto>> Start(string drawId, IEnumerable<string> ticketIds)
        {
            if (ticketIds == null || !ticketIds.Any())
            {
                return new ResponseDto<DrawDto>("Missing tickets for draw");
            }

            Draw draw = await repository.GetByIdAsync(drawId);
            if (draw == null)
            {
                return new ResponseDto<DrawDto>("Draw not found");
            }

            if (draw.Status != DrawStatus.Pending)
            {
                return new ResponseDto<DrawDto>("Draw cannot be started");
            }

            draw.Status = DrawStatus.InProgress;
            draw.TicketIds = ticketIds.ToList();

            await repository.UpdateAsync(draw);

            return mapper.Map<ResponseDto<DrawDto>>(draw);
        }
    }
}
