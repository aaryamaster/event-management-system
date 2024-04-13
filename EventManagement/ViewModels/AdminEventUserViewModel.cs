namespace EventManagement.ViewModels
{
    public class AdminEventUserViewModel
    {
        public string EventName { get; set; }
        public string HallName { get; set; }
        public Double EventPrice { get; set; }
        public List<UserTicketViewModel> UserTickets { get; set; }
        public bool IsEventFinished { get; set; }

        public int EventId { get; set; }
    }

    public class UserTicketViewModel
    {
        public string UserName { get; set; }
        public int NumberOfTickets { get; set; }
        public string FullName { get; set; }
        public int TicketId { get; set; }
    }

}
