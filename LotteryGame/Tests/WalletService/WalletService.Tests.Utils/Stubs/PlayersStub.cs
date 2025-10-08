using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Data.Models;

namespace WalletService.Tests.Utils.Stubs
{
    public static class PlayersStub
    {
        public static List<Player> GetPlayers()
        {
            return Enumerable.Range(1, 15)
                .Select(id => new Player
                {
                    Id = id,
                    Name = $"Player {id}{(id > 1 ? " (CPU)" : "")}",
                    CreatedOn = new DateTime(2025, 10, 04)
                })
                .ToList();
        }
    }
}
