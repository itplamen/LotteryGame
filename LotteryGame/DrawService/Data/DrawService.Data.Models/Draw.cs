namespace DrawService.Data.Models
{
    public class Draw : BaseEntity
    {
        public DateTime DrawDate { get; set; }

        public DrawStatus Status { get; set; } 
        
        public ICollection<string> TicketIds { get; set; } = new HashSet<string>();

        public ICollection<string> PrizeIds { get; set; } = new HashSet<string>();
    }
}
