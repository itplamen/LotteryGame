namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;

    public interface IDrawOperations
    {
        Task<ResponseDto<DrawDto>> GetOpenDraw(int playerId);

        Task<ResponseDto<DrawDto>> Create();

        Task<ResponseDto<DrawDto>> Start(string drawId);

        Task<ResponseDto<DrawDto>> Join(string drawId, int playerId, IEnumerable<string> ticketIds);
    }
}
