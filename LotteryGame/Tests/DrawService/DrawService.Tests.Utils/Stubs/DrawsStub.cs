namespace DrawService.Tests.Utils.Stubs
{
    using DrawService.Data.Models;

    public static class DrawsStub
    {
        public static Draw CompletedDraw => new Draw() { Id = "draw1_completed", Status = DrawStatus.Completed };

        public static Draw InProgressDraWithoutTickets =>  new Draw()
        {
            Id = "draw1_in_progress",
            Status = DrawStatus.InProgress,
            PlayerTickets = new List<PlayerTicketInfo>()
        };
    }
}
