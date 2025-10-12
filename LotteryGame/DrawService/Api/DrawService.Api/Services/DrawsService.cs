namespace DrawService.Api.Services
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Google.Protobuf.WellKnownTypes;

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

        public override async Task<DrawOptionsProtoResponse> GetDrawOptions(Empty request, ServerCallContext context)
        {
            ResponseDto<DrawDto> openDrawDto = await drawOperations.GetDrawOptions();
            DrawOptionsProtoResponse response = mapper.Map<DrawOptionsProtoResponse>(openDrawDto);

            return response;
        }

        public override async Task<GetPlayerDrawProtoResponse> GetPlayerDraw(GetPlayerDrawProtoRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> openDrawDto = await drawOperations.GetOpenDraw(request.PlayerId);
            GetPlayerDrawProtoResponse response = mapper.Map<GetPlayerDrawProtoResponse>(openDrawDto);
            
            return response;
        }

        public async override Task<GetPlayerDrawProtoResponse> CreateDraw(Empty request, ServerCallContext context)
        {
            ResponseDto<DrawDto> createdDrawDto = await drawOperations.Create();
            GetPlayerDrawProtoResponse response = mapper.Map<GetPlayerDrawProtoResponse>(createdDrawDto);

            return response;
        }

        public override async Task<GetPlayerDrawProtoResponse> JoinDraw(JoinDrawProtoRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> responseDto = await drawOperations.Join(request.DrawId, request.PlayerId, request.TicketIds);
            GetPlayerDrawProtoResponse response = mapper.Map<GetPlayerDrawProtoResponse>(responseDto);

            return response;
        }
    }
}
