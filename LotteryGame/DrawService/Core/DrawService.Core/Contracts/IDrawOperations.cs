namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;

    public interface IDrawOperations
    {
        Task<ResponseDto<DrawDto>> Create();

        Task<ResponseDto<DrawDto>> Start(string drawId, IEnumerable<string> ticketIds);
    }
}
