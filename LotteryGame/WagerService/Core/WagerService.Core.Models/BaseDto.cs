namespace WagerService.Core.Models
{
    public class BaseDto
    {
        public BaseDto() { }

        public BaseDto(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
