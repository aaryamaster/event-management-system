using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class CancelBookingByAdminViewModel
    {
        public AdminEvent AdminEvent { get; set; }
        public Ticket Ticket { get; set; }

        public string Username { get; set; }    
    }
}
