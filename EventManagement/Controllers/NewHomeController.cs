using EventManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventManagement.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SQLitePCL;
using EventManagement.ViewModels;

namespace EventManagement.Controllers
{
    [Authorize]
    public class NewHomeController : Controller
    {
        private readonly EventManagementContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public NewHomeController(
            EventManagementContext context,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManage)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManage;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Admin");
                return RedirectToAction("Dashboard");
            }

            var featuredEvents = _context.AdminEvents
                             .Where(e => e.Featured && !e.IsEventFinished)
                             .Select(e => new FeaturedEventViewModel
                             {
                                 EventName = e.EventName,
                                 Description = e.Description
                             })
                             .ToList();

            return View(featuredEvents);
        }
        public IActionResult BrowseEvent()
        {
            var adminEvents = _context.AdminEvents.Include(h => h.Hall).ToList();
            var tickets = _context.Tickets.ToList();
            var userId = _userManager.GetUserId(User);

            var viewModel = new BrowseEventsViewModel(adminEvents, tickets, userId);

            return View(viewModel);
        }
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin");
            
            var featuredEvents = _context.AdminEvents
                             .Where(e => e.Featured && !e.IsEventFinished)
                             .Select(e => new FeaturedEventViewModel
                             {
                                 EventName = e.EventName,
                                 Description = e.Description
                             })
                             .ToList();
            return View(featuredEvents);
        }
        public IActionResult MyEvents()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }
            var purchasedEvents = _context.Users
                                          .Where(u => u.Id == userId)
                                          .SelectMany(u => u.Events.OfType<AdminEvent>())
                                          .Include(p => p.Hall)
                                          .Include(t => t.Tickets)
                                          .Select(item => new MyPurchasedEventViewModel
                                          {
                                              EventId = item.EventId,
                                              EventName = item.EventName,
                                              Date = item.Date,
                                              IsEventFinished = item.IsEventFinished,
                                              NumberOfTickets = (int)item.Tickets.FirstOrDefault(t => t.UserId == userId).NumberOfTickets,
                                              TotalPrice = item.Tickets.FirstOrDefault(t => t.UserId == userId).Price,
                                              ContactInformation = item.ContactInformation,
                                              HallName = item.Hall.HallName,
                                              Venue = item.Hall.Venue,
                                              Location = item.Hall.Location,
                                              Duration = item.Duration,
                                              TicketId = item.Tickets.First(t=>t.UserId==userId).TicketId
                                          })
                                          .ToList();

            return View(purchasedEvents);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult BookEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (_context.Tickets.Any(t => t.EventId == id && t.UserId == userId))
            {
                return View("EventAlreadyBooked");
            }

            var adminEvent = _context.AdminEvents.Include(h => h.Hall).FirstOrDefault(c => c.EventId == id);

            if (adminEvent == null)
            {
                return NotFound();
            }

            var viewModel = new BookEventViewModel
            {
                AdminEvent = adminEvent,
                UserId = userId,
                TicketsLeft = (double)(_context.Halls.First(h => h.HallId == adminEvent.HallId).GuestLimit - _context.Tickets.Where(t => t.EventId == id).Sum(t => t.NumberOfTickets))
            };

            return View(viewModel);
        }
        //public JsonResult GetTicketPriceAndLimit(int eventId, int NumberOfTickets, double ticketPrice, int hallId)
        //{
        //    var TotalTickets = _context.Tickets.Where(t => t.EventId == eventId).Sum(t => t.NumberOfTickets);
        //    var ticketsLeft = _context.Halls.First(h => h.HallId == hallId).GuestLimit - TotalTickets;
        //    var price = 0.0;
        //    if(ticketsLeft >= NumberOfTickets)
        //    {
        //        price = NumberOfTickets * ticketPrice;
        //    }

        //    return Json(new {Price = price, TicketsLeft = ticketsLeft});
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmBooking(BookEventViewModel viewModel)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            var adminEvent = _context.AdminEvents.Include(e => e.Users).FirstOrDefault(e => e.EventId == viewModel.AdminEvent.EventId);

            Ticket ticket = new Ticket
            {
                EventId = viewModel.AdminEvent.EventId,
                UserId = viewModel.UserId,
                NumberOfTickets = viewModel.NumberOfTickets,
                Price = (double)viewModel.Price
            };

            ticket.Event = adminEvent;
            ticket.User = currentUser;
            adminEvent.Users.Add(currentUser);

            _context.Add(ticket);
            _context.SaveChanges();

            return View();

        }

        public IActionResult Delete(int EventId, int TicketId)
        {
            if(!HasAccess(EventId))
            {
                return View("AccessDenied");
            }
            if (IsEventFinished(EventId))
            {
                return View("EventFinished");
            }
            var adminEvent = _context.AdminEvents
                .Include(c => c.Hall).FirstOrDefault(m => m.EventId == EventId);
            var ticket = _context.Tickets.FirstOrDefault(_context => _context.TicketId == TicketId);
            if (adminEvent == null || ticket == null)
            {
                return NotFound();
            }
            var viewModel = new DeletePurchasedEventViewModel(){
                AdminEvent = adminEvent,
                Ticket = ticket
            };
            
            return View(viewModel);
        }
        public IActionResult DeleteConfirmed(int EventId, int TicketId)
        {
            if (!HasAccess(EventId))
            {
                return View("AccessDenied");
            }
            var ticket = _context.Tickets.Find(TicketId);
            if (ticket == null)
            {
                return NotFound();
            }
            _context.Tickets.Remove(ticket);
            var @event = _context.Events.Find(EventId);
            if(@event == null)
            {
                return NotFound();
            }
            var user = _context.Users.Include(e => e.Events).First(u => u.Id == _userManager.GetUserId(User));
            user.Events.Remove(@event);
            _context.SaveChanges();
            return View();

        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult EventFinished()
        {
            return View();
        }
        public bool HasAccess(int id)
        {
            string userId = _userManager.GetUserId(User);
            var requestedEvent = _context.Users.Where(u => u.Id == userId).SelectMany(u => u.Events.OfType<AdminEvent>().Select(e => e.EventId)).ToList();

            if (requestedEvent == null || !requestedEvent.Contains(id))
            {
                return false;
            }
            return true;
        }
        public bool IsEventFinished(int id)
        {
            return _context.AdminEvents.Find(id).IsEventFinished;
        }

    }
}
