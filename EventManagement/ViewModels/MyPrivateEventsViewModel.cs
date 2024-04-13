using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class MyPrivateEventsViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        public bool IsEventFinished { get; set; }
        public int NumberOfGuests { get; set; }
        public FoodType FoodType { get; set; }
        public double Price { get; set; }
        public string ContactInformation { get; set; }
        public string HallName { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }

        public string Duration { get; set; }
    }
}
