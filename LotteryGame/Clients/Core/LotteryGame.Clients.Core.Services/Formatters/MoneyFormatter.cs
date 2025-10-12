namespace LotteryGame.Clients.Core.Services.Formatters
{
    public static class MoneyFormatter
    {
        public static decimal ToDecimal(long cents) => Math.Round(cents / 100m, 2);

        public static long ToCents(decimal amount) => (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);

        public static string ToLabelValue(decimal amount) => $"${amount:F2}";
    }
}
