using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Data.Models;

namespace WalletService.Tests.Utils.Stubs
{
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
