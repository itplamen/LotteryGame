namespace DrawService.Core.Validation.Contexts
{
    using DrawService.Data.Models;

    public class BaseDrawContext
    {
        public string DrawId { get; set; }

        public DrawStatus Status { get; set; }

        public Draw Draw { get; set; }
    }
}