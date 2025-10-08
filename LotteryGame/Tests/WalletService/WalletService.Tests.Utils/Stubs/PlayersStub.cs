namespace WalletService.Tests.Utils.Stubs
{
    using WalletService.Data.Models;

    public static class PlayersStub
    {
        public static List<Player> GetPlayers()
        {
            return Enumerable.Range(1, 15)
                .Select(id => new Player()
                {
                    Id = id,
                    Name = $"Player {id}{(id > 1 ? " (CPU)" : "")}",
                    CreatedOn = new DateTime(2025, 10, 04)
                })
                .ToList();
        }
    }
}
