namespace WalletService.Tests.Utils.Stubs
{
    using WalletService.Data.Models;

    public static class WalletStubs
    {
        public static List<Wallet> GetWallets()
        {
            List<Player> players = PlayersStub.GetPlayers();

            return Enumerable.Range(1, 15)
                .Select(id => new Wallet()
                {
                    Id = id,
                    PlayerId = id,
                    Player = players.First(x => x.Id == id),
                    RealMoney = 15,
                    BonusMoney = 30,
                    CreatedOn = new DateTime(2025, 10, 04)
                })
                .ToList();
        }
    }
}
