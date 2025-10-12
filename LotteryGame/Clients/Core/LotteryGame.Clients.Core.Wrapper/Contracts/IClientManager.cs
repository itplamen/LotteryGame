namespace LotteryGame.Clients.Core.Wrapper.Contracts
{
    public interface IClientManager
    {
        void Write(string value);

        void WriteLine(string value);

        string ReadLine();

        void Clear();
    }
}
