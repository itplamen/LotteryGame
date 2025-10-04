namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public interface IDrawOperations
    {
        Task<ResponseDto<DrawDto>> GetOpenDraw(int playerId);

        Task<ResponseDto<DrawDto>> Create();

        Task<ResponseDto> Join(string drawId, int playerId, IEnumerable<string> ticketIds);

        Task<ResponseDto> Start(string drawId);
    }
}
