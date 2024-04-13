using System.ComponentModel.DataAnnotations;
using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class BookEventViewModel
    {
        public AdminEvent AdminEvent { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage ="Please enter number of tickets")]
        [RegularExpression(@"^\d+$",ErrorMessage ="Enter a integer value")]
        public int NumberOfTickets { get; set; }
        public decimal Price { get; set; }
        public double TicketsLeft { get; set; }
    }
}
