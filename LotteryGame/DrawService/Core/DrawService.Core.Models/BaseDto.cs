namespace DrawService.Core.Models
{
    public class BaseDto
    {
        public BaseDto() { }

        public BaseDto(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
