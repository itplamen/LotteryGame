namespace DrawService.Core.Models
{
    public class ResponseDto
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            Errors = new List<string>() { errorMessage };
        }

        public bool IsSuccess => Errors == null || !Errors.Any();

        public IEnumerable<string> Errors { get; set; }
    }

    public class ResponseDto<TResponse>
        where TResponse : BaseDto
    {
        public ResponseDto() { }

        public ResponseDto(string errorMessage)
        {
            Errors = new List<string>() { errorMessage };
        }

        public bool IsSuccess => Data != null && (Errors == null || !Errors.Any());

        public IEnumerable<string> Errors { get; set; }

        public TResponse Data { get; set; }
    }
}
