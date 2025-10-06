namespace DrawService.Api.Services
{
    using System.Threading.Tasks;

    using AutoMapper;
    
    using Grpc.Core;

    using DrawService.Api.Models.Protos.Draws;
    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public class DrawsService : Draws.DrawsBase
    {
        private readonly IMapper mapper;
        private readonly IDrawOperations drawOperations;

        public DrawsService(IMapper mapper, IDrawOperations drawOperations)
        {
            this.mapper = mapper;
            this.drawOperations = drawOperations;
        }

        public override async Task<FetchDrawResponse> FetchDraw(FetchDrawRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> openDrawDto = await drawOperations.GetOpenDraw(request.PlayerId);

            if (!openDrawDto.IsSuccess)
            {
                openDrawDto = await drawOperations.Create();
            }

            FetchDrawResponse response = mapper.Map<FetchDrawResponse>(openDrawDto);
            return response;
        }

        public override async Task<FetchDrawResponse> JoinDraw(JoinDrawRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> responseDto = await drawOperations.Join(request.DrawId, request.PlayerId, request.TicketIds);
            FetchDrawResponse response = mapper.Map<FetchDrawResponse>(responseDto);

            return response;
        }

        public override async Task<DrawResponse> StartDraw(StartDrawRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await drawOperations.Start(request.DrawId);
            DrawResponse response = mapper.Map<DrawResponse>(responseDto);

            return response;
        }
    }
}
