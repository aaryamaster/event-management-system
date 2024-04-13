using EventManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.ViewModels
{
    public class UserOrganizedEventViewModel
    {
        public string Username { get; set; }
        public int EventId { get; set; }
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Hall")]
        public string HallName { get; set; }
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }
        [Display(Name = "Food Type")]
        public FoodType FoodType { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        [Display(Name = "Contact Infomation")]
        public string ContactInformation { get; set; }
        public bool IsEventFinished { get; set; }
    }

}
