namespace LotteryGame.Common.Models.Dto
{
    public class ResponseDto
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            ErrorMsg = errorMessage;
        }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMsg);

        public string ErrorMsg { get; set; } = string.Empty;
    }

    public class ResponseDto<TResponse>
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            ErrorMsg = errorMessage;
        }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMsg);

        public string ErrorMsg { get; set; } = string.Empty;

        public TResponse Data { get; set; }
    }
}
