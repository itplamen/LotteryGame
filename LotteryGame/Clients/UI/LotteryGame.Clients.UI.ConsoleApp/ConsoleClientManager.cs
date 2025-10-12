namespace LotteryGame.Clients.UI.ConsoleApp
{
    using LotteryGame.Clients.Core.Wrapper.Contracts;

    public class ConsoleClientManager : IClientManager
    {
        public void Write(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Console.Write(value);
            }
        }

        public void WriteLine(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Console.WriteLine(value);
            }
        }

        public string ReadLine()
        {
            string input = Console.ReadLine();

            return input ?? string.Empty;
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
