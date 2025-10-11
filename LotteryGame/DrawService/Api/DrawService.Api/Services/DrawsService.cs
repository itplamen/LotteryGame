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

        public override async Task<FetchDrawProtoResponse> FetchDraw(FetchDrawProtoRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> openDrawDto = await drawOperations.GetOpenDraw(request.PlayerId);
            FetchDrawProtoResponse response = mapper.Map<FetchDrawProtoResponse>(openDrawDto);
            
            return response;
        }

        public async override Task<FetchDrawProtoResponse> CreateDraw(Empty request, ServerCallContext context)
        {
            ResponseDto<DrawDto> createdDrawDto = await drawOperations.Create();
            FetchDrawProtoResponse response = mapper.Map<FetchDrawProtoResponse>(createdDrawDto);

            return response;
        }

        public override async Task<FetchDrawProtoResponse> JoinDraw(JoinDrawProtoRequest request, ServerCallContext context)
        {
            ResponseDto<DrawDto> responseDto = await drawOperations.Join(request.DrawId, request.PlayerId, request.TicketIds);
            FetchDrawProtoResponse response = mapper.Map<FetchDrawProtoResponse>(responseDto);

            return response;
        }
    }
}
