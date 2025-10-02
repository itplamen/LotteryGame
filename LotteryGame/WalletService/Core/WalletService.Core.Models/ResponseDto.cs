namespace WalletService.Core.Models
{
    public class ResponseDto
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            ErrorMsg = errorMessage;
        }

        public bool IsSuccess => !string.IsNullOrEmpty(ErrorMsg);

        public string ErrorMsg { get; set; }
    }

    public class ResponseDto<TResponse>
        where TResponse : BaseDto
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            ErrorMsg = errorMessage;
        }

        public bool IsSuccess => !string.IsNullOrEmpty(ErrorMsg);

        public string ErrorMsg { get; set; }

        public TResponse Data { get; set; }
    }
}
