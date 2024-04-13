using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Drawing.Printing;
using System.Diagnostics.Eventing.Reader;
using EventManagement.ViewModels;
using System.Collections;
using Newtonsoft.Json;
using EventManagement.Models;

namespace EventManagement.Controllers
{
    [Authorize]

    public class PrivateEventsController : Controller
    {
        private readonly EventManagementContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private static Dictionary<string, double> _priceDetails;
        public PrivateEventsController(EventManagementContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PrivateEvents
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var userOrganizedEvents = await _context.PrivateEvents
                .Include(h => h.Hall).Include(u=>u.Users)
                .Select(e => new UserOrganizedEventViewModel
                {
                    Username = e.Users.First().UserName,
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Date = e.Date,
                    HallName = e.Hall.HallName,
                    NumberOfGuests = e.NumberOfGuests,
                    FoodType = e.FoodType,
                    Description = e.Description,
                    Duration = e.Duration,
                    ContactInformation = e.ContactInformation,
                    IsEventFinished = e.IsEventFinished
                })
                .ToListAsync();

            return View(userOrganizedEvents);
        }



        public bool HasAccess(int id)
        {
            if (User.IsInRole("Admin"))
                return true;
            string userId = _userManager.GetUserId(User);
            var requestedEvent = _context.Users.Where(u => u.Id == userId).SelectMany(u => u.Events.OfType<PrivateEvent>().Select(e => e.EventId)).ToList();

            if (requestedEvent == null || !requestedEvent.Contains(id))
            {
                return false;
            }
            return true;
        }
        // GET: PrivateEvents/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (!HasAccess((int)id))
        //    {
        //        return RedirectToAction("AccessDenied", "NewHome");
        //    }
        //    if (id == null || _context.PrivateEvents == null)
        //    {
        //        return NotFound();
        //    }

        //    var privateEvent = await _context.PrivateEvents
        //        .Include(p => p.Hall)
        //        .FirstOrDefaultAsync(m => m.EventId == id);

        //    if (privateEvent == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.FoodCharge = GetFoodCharge(privateEvent.FoodType) * privateEvent.NumberOfGuests;
        //    ViewBag.DurationCharge = CalculateDurationCharge(privateEvent.Duration);
        //    ViewBag.User = _context.Users.Find(_userManager.GetUserId(User));
        //    ViewBag.Tickets = _context.Tickets.Find(privateEvent.EventId);
        //    ViewBag.Hall = _context.Halls.Find(privateEvent.HallId);
        //    return View(privateEvent);
        //}
        //public IActionResult GetHallId(string hallName)
        //{
        //    var hallId = _context.Halls
        //        .Where(h => h.HallName == hallName)
        //        .Select(h => h.HallId)
        //        .FirstOrDefault();

        //    return Json(new { hallId });
        //}

        public IActionResult GetGuestLimit(int hallId)
        {
            var guestLimit = _context.Halls.Find(hallId).GuestLimit;

            return Json(new { guestLimit });
        }

        public IActionResult GetAvailableHalls(DateTime date)
        {
            var availableHalls = _context.Halls.Where(h => !h.Events.Any(e => e.Date.Date == date.Date)).OrderBy(h => h.Location)
                                                .Select(h => new { hallId = h.HallId, hallName = h.HallName, location = h.Location, venue = h.Venue });
            var unavailableHalls = _context.Halls.Where(h => h.Events.Any(e => e.Date.Date == date.Date)).OrderBy(h => h.Location)
                                                 .Select(h => new { hallId = h.HallId, hallName = h.HallName, location = h.Location, venue = h.Venue });

            return Json(new { availableHalls, unavailableHalls });
        }

        // GET: PrivateEvents/Create
        public IActionResult Create()
        {
            //ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallId");
            ViewBag.Halls = _context.Halls.OrderBy(h=>h.Location);

            return View();
        }

        // POST: PrivateEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("NumberOfGuests,FoodType,EventId,EventType,EventName,Description,Date,ContactInformation,Duration,HallId")] PrivateEvent privateEvent)
        //{
        //    ModelState.Clear(); // Clear previous ModelState errors

        //    try
        //    {
        //        // Check if HallId is valid
        //        if (privateEvent.HallId == 0)
        //        {
        //            // Handle the case where HallId is not set
        //            ModelState.AddModelError("HallId", "Hall is required.");
        //            // You may also redirect back to the Create view with an error message
        //            ViewBag.HallName = new SelectList(_context.Halls.Select(h => h.HallName).ToList());
        //            return View(privateEvent);
        //        }

        //        // Set the Hall property
        //        privateEvent.Hall = _context.Halls.FirstOrDefault(h => h.HallId == privateEvent.HallId);

        //        var currentUserId = _userManager.GetUserId(User);
        //        var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

        //        if (currentUser != null)
        //        {
        //            // Associate the existing user with the private event
        //            privateEvent.Users = new List<ApplicationUser> { currentUser };
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            // Add the privateEvent to the context
        //            var ticket = new Ticket
        //            {
        //                //EventId = privateEvent.EventId,
        //                Event = privateEvent,
        //                Price = CalculateTicketPrice(privateEvent),
        //                UserId = currentUserId,
        //                User = currentUser
        //                // NumberOfTickets = privateEvent.NumberOfGuests
        //            };
        //            privateEvent.Tickets = new List<Ticket> { ticket };
        //            _context.Add(ticket);
        //            _context.Add(privateEvent);
        //            await _context.SaveChangesAsync();

        //            // Redirect to the Index page or another action
        //            return RedirectToAction(nameof(Receipt), privateEvent);
        //        }
        //        else
        //        {
        //            // If ModelState is not valid, log the validation errors
        //            foreach (var modelState in ViewData.ModelState.Values)
        //            {
        //                foreach (var error in modelState.Errors)
        //                {
        //                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        //                }
        //            }

        //            // If there are validation errors, return to the Create view with the model
        //            ViewBag.HallName = new SelectList(_context.Halls.Select(h => h.HallName).ToList());
        //            return View(privateEvent);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log any unexpected exceptions
        //        Console.WriteLine($"Exception in Create method: {ex.Message}");
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumberOfGuests,FoodType,EventId,EventType,EventName,Description,Date,ContactInformation,Duration,HallId")] PrivateEvent privateEvent)
        {
           // ModelState.Clear(); // Clear previous ModelState errors

            try
            {
                privateEvent.Hall = await _context.Halls.FirstOrDefaultAsync(h => h.HallId == privateEvent.HallId);
                _priceDetails = CalculateTicketPrice(privateEvent);

                var viewModel = new ConfirmBookingViewModel
                {
                    PrivateEvent = privateEvent,
                    FoodCharge = _priceDetails["FoodCharge"],
                    DurationCharge = _priceDetails["DurationCharge"],
                    TotalTax = _priceDetails["GST"] + _priceDetails["OtherCharges"],
                    TotalPrice = _priceDetails["TicketPrice"]
                };
                    
                return View("Receipt", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create method: {ex.Message}");
                return View(privateEvent);
            }
        }

        public ActionResult Receipt(string viewModelJson)
        {
            if (!string.IsNullOrEmpty(viewModelJson))
            {
                // Deserialize the view model JSON back to a ConfirmBookingViewModel object
                var viewModel = JsonConvert.DeserializeObject<ConfirmBookingViewModel>(viewModelJson);

                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }


        //[HttpPost]
        //public IActionResult AbortBooking(int id)
        //{
        //    var privateEvent = _context.PrivateEvents.Find(id);
        //    _context.PrivateEvents.Remove(privateEvent);
        //    _context.SaveChanges();
        //    return View();
        //}

        //public IActionResult ConfirmBooking()
        //{
        //    _context.SaveChanges();

        //    return View();
        //}
        public async Task<IActionResult> ConfirmBooking(string viewModelJson)
        {
            if (!string.IsNullOrEmpty(viewModelJson))
            {
                // Deserialize the view model JSON back to a ConfirmBookingViewModel object
                var privateEvent = JsonConvert.DeserializeObject<PrivateEvent>(viewModelJson);
                if (PrivateEventExists(privateEvent.EventId)){
                    var ticket = _context.Tickets.FirstOrDefault(t => t.EventId == privateEvent.EventId);
                    ticket.Price = _priceDetails["TicketPrice"];
                    privateEvent.Tickets = new List<Ticket> { ticket };

                    _context.Update(ticket);
                    _context.Update(privateEvent);
                    await _context.SaveChangesAsync();
                    return View();

                }
                privateEvent.Hall = _context.Halls.FirstOrDefault(h => h.HallId == privateEvent.HallId);

                var currentUserId = _userManager.GetUserId(User);

                var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

                if (currentUser != null)
                {
                    privateEvent.Users = new List<ApplicationUser> { currentUser };
                }
                var newticket = new Ticket
                {
                    Event = privateEvent,
                    Price = _priceDetails["TicketPrice"],
                    UserId = currentUserId,
                    User = currentUser
                };
                privateEvent.Tickets = new List<Ticket> { newticket };
                await _context.AddAsync(privateEvent);

                await _context.SaveChangesAsync();
                return View();

            }
            else
            {
                return NotFound();
            }
        }


        public Dictionary<string,double> CalculateTicketPrice(PrivateEvent privateEvent)
        {
            // Base price from Hall
            double basePrice = privateEvent.Hall.HallPrice;

            // Additional charge per guest based on food type
            double foodCharge = GetFoodCharge(privateEvent.FoodType) * privateEvent.NumberOfGuests;

            // Additional charge based on duration
            double durationCharge = CalculateDurationCharge(privateEvent.Duration);

            // Calculate total price before taxes
            double totalPriceBeforeTaxes = basePrice + foodCharge + durationCharge;

            // Calculate GST (18%)
            double gst = 0.18 * totalPriceBeforeTaxes;

            // Calculate other charges (10%)
            double otherCharges = 0.10 * totalPriceBeforeTaxes;

            // Calculate final ticket price
            double ticketPrice = totalPriceBeforeTaxes + gst + otherCharges;

            Dictionary<string, double> priceDetails = new Dictionary<string, double>
            {
                { "BasePrice", basePrice },
                { "FoodCharge", foodCharge },
                { "DurationCharge", durationCharge },
                { "TotalPriceBeforeTaxes", totalPriceBeforeTaxes },
                { "GST", gst },
                { "OtherCharges", otherCharges },
                { "TicketPrice", ticketPrice }
            };
            return priceDetails;
        }

        private double GetFoodCharge(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.Vegetarian:
                    return 200;
                case FoodType.NonVegetarian:
                    return 300;
                case FoodType.Jain:
                    return 180;
                case FoodType.Mixed:
                    return 500;
                case FoodType.None:
                    return 0;
                default:
                    return 0;
            }
        }

        private double CalculateDurationCharge(string duration)
        {
            if (double.TryParse(duration.Trim(), out double hours))
            {
                // Additional charge per hour
                double hourCharge = 5000;
                return hours * hourCharge;
            }

            return 0;
        }

        public IActionResult MyEvents()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index");

            var userId = _userManager.GetUserId(User);
            var privateEvents = _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Events.OfType<PrivateEvent>())
                .Include(p => p.Hall)
                .Include(t => t.Tickets)
                .ToList();

            var privateEventViewModels = privateEvents.Select(item => new MyPrivateEventsViewModel
            {
                EventId = item.EventId,
                EventName = item.EventName,
                Date = item.Date,
                IsEventFinished = item.IsEventFinished,
                NumberOfGuests = item.NumberOfGuests,
                FoodType = item.FoodType,
                Price = item.Tickets.FirstOrDefault().Price,
                ContactInformation = item.ContactInformation,
                HallName = item.Hall.HallName,
                Venue = item.Hall.Venue,
                Location = item.Hall.Location,
                Duration = item.Duration
            });

            return View(privateEventViewModels);
        }


        // GET: PrivateEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess((int)id))
            {
                return RedirectToAction("AccessDenied", "NewHome");
            }
            if (id == null || _context.PrivateEvents == null)
            {
                return NotFound();
            }
            if (IsEventFinished((int)id))
            {
                return RedirectToAction("EventFinished", "NewHome");
            } 

            var privateEvent = await _context.PrivateEvents.FindAsync(id);
            privateEvent.Hall = _context.Halls.Find(privateEvent.HallId);
            if (privateEvent == null)
            {
                return NotFound();
            }
            ViewBag.Halls = _context.Halls.OrderBy(h => h.Location);
            return View(privateEvent);
        }

        // POST: PrivateEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumberOfGuests,FoodType,EventId,EventType,EventName,Description,Date,ContactInformation,Duration,HallId")] PrivateEvent privateEvent)
        {
            if (id != privateEvent.EventId)
            {
                return NotFound();
            }

            try
            {
                privateEvent.Hall = _context.Halls.FirstOrDefault(h => h.HallId == privateEvent.HallId);
                _priceDetails = CalculateTicketPrice(privateEvent);

                var viewModel = new ConfirmBookingViewModel
                {
                    PrivateEvent = privateEvent,
                    FoodCharge = _priceDetails["FoodCharge"],
                    DurationCharge = _priceDetails["DurationCharge"],
                    TotalTax = _priceDetails["GST"] + _priceDetails["OtherCharges"],
                    TotalPrice = _priceDetails["TicketPrice"]
                };

                return View("Receipt", viewModel);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Edit method: {ex.Message}");
                ViewBag.HallName = _context.Halls.OrderBy(h => h.Location);
                return View(privateEvent);
            }
            
        }

        // GET: PrivateEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess((int)id))
            {
                return RedirectToAction("AccessDenied", "NewHome");
            }
            if (IsEventFinished((int)id) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("EventFinished", "NewHome");
            }
            if (id == null || _context.PrivateEvents == null)
            {
                return NotFound();
            }

            var privateEvent = await _context.PrivateEvents
                .Include(p => p.Hall)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (privateEvent == null)
            {
                return NotFound();
            }

            return View(privateEvent);
        }

        // POST: PrivateEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess((int)id))
            {
                return RedirectToAction("AccessDenied", "NewHome");
            }
            if (_context.PrivateEvents == null)
            {
                return Problem("Entity set 'EventManagementContext.PrivateEvents'  is null.");
            }
            var privateEvent = await _context.PrivateEvents.FindAsync(id);
            if (privateEvent != null)
            {
                _context.PrivateEvents.Remove(privateEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyEvents));
        }

        public bool IsEventFinished(int id)
        {
            return _context.PrivateEvents.Find(id).IsEventFinished;
        }
        private bool PrivateEventExists(int id)
        {
            return (_context.PrivateEvents?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
