using EventManagement.Models;

namespace EventManagement.ViewModels
{
    public class ConfirmBookingViewModel
    {
        public PrivateEvent PrivateEvent { get; set; }
        public double DurationCharge { get; set; }
        public double FoodCharge { get; set; }
        public double TotalTax { get; set; }
        public double TotalPrice { get; set; }
    }
}
