namespace DrawService.Data.Models
{
    public class PlayerTicketInfo
    {
        public int PlayerId { get; set; }

        public List<string> Tickets { get; set; } = new List<string>();
    }
}
