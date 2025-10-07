namespace LotteryGame.Common.Models.Dto
{
    public class BaseDto
    {
        public BaseDto() { }

        public BaseDto(string id)
        {
            Id = id;
        }

        public string Id { get; set; } = string.Empty;
    }
}
