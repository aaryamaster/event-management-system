using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.ViewModels
{
    public class BrowseEventsViewModel
    {
        public List<AdminEvent> AdminEvents { get; set; }
        public List<Ticket> Tickets { get; set; }
        public string UserId { get; set; }
        public Dictionary<int, double> TicketsLeft { get; set; }

        public BrowseEventsViewModel(List<AdminEvent> adminEvents, List<Ticket> tickets, string userId)
        {
            AdminEvents = adminEvents;
            Tickets = tickets;
            UserId = userId;
            TicketsLeft = new Dictionary<int, double>();

            foreach (var e in AdminEvents)
            {
                int eventId = e.EventId;
                int hallId = e.HallId;
                double totalTickets = (double)Tickets.Where(t => t.EventId == eventId).Sum(t => t.NumberOfTickets);
                double ticketsRemaining = AdminEvents.Where(h=>h.HallId==hallId).Select(h => h.Hall).First().GuestLimit - totalTickets;
                TicketsLeft[eventId] = ticketsRemaining;
            }
        }
    }
}
