namespace WalletService.Api.Services
{
    using System.Threading.Tasks;

    using AutoMapper;
    
    using Grpc.Core;
    
    using WalletService.Api.Models.Protos.Funds;
    using WalletService.Core.Contracts;
    using WalletService.Core.Models;

    public class FundsService : Funds.FundsBase
    {
        private readonly IMapper mapper;
        private readonly IFundsOperations fundsOperations;

        public FundsService(IMapper mapper, IFundsOperations fundsOperations)
        {
            this.mapper = mapper;
            this.fundsOperations = fundsOperations;
        }

        public override async Task<EnoughFundsResponse> HasEnoughFunds(EnoughFundsRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.HasEnoughFunds(request.PlayerId, request.CostAmount);
            EnoughFundsResponse response = mapper.Map<EnoughFundsResponse>(responseDto);

            return response;
        }
    }
}
