using EventManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.ViewModels
{
    public class AdminOrganizedEventViewModel
    {
        public int EventId { get; set; }
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Hall")]
        public string HallName { get; set; }
        [Display(Name = "Display")]
        public string ImagePath { get; set; }
        [Display(Name ="Ticket Limit")]
        public int NumberOfGuests { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        [Display(Name = "Event Price")]
        public double EventPrice { get; set; }
        [Display(Name = "Contact Information")]
        public string ContactInformation { get; set; }
        public bool IsEventFinished { get; set; }
        [Display(Name ="Tickets Left")]
        public double TicketsLeft { get; set; }
    }
}
