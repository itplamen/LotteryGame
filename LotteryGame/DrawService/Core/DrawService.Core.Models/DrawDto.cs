namespace DrawService.Core.Models
{
    using DrawService.Data.Models;

    public class DrawDto : BaseDto
    {
        public DateTime DrawDate { get; set; }

        public DrawStatus Status { get; set; }
    }
}
