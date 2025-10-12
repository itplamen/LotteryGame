namespace LotteryGame.Clients.Core.Services.Formatters
{
    public static class DatetimeFormatter
    {
        private const int SECONDS = 60;
        private const int MINUTES = 60;
        private const int HOURS = 24;

        public static string Format(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");

        public static string TimeRemains(DateTime dateTime)
        {
            DateTime now = DateTime.UtcNow;

            TimeSpan diff = dateTime - now;

            string display;

            if (diff.TotalSeconds < SECONDS)
            {
                display = $"in {diff.Seconds} seconds";
            }
            else if (diff.TotalMinutes < MINUTES)
            {
                display = $"in {diff.Minutes} minute{(diff.Minutes == 1 ? "" : "s")}";
            }
            else if (diff.TotalHours < HOURS)
            {
                display = $"in {diff.Hours} hour{(diff.Hours == 1 ? "" : "s")}";
            }
            else
            {
                display = $"in {diff.Days} day{(diff.Days == 1 ? "" : "s")}";
            }

            return display;
        }
    }
}
