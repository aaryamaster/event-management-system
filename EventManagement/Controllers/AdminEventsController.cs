using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EventManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.IO;
using EventManagement.Models;
using EventManagement.ViewModels;
using NuGet.Versioning;

namespace EventManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminEventsController : Controller
    {
        private readonly EventManagementContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminEventsController(EventManagementContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult ConfirmBooking()
        {
            return View();
        }
        // GET: AdminEvents
        public async Task<IActionResult> Index()
        {
            var eventList = await _context.AdminEvents
                .Include(c => c.Hall)
                .Select(e => new AdminOrganizedEventViewModel
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Date = e.Date,
                    HallName = e.Hall.HallName,
                    ImagePath = e.ImagePath,
                    Description = e.Description,
                    NumberOfGuests=e.Hall.GuestLimit,
                    Duration = e.Duration,
                    EventPrice = e.EventPrice,
                    ContactInformation = e.ContactInformation,
                    IsEventFinished = e.IsEventFinished,
                    TicketsLeft = (double)(e.Hall.GuestLimit - _context.Tickets.Where(t => t.EventId == e.EventId).Sum(t => t.NumberOfTickets))
                })
                .ToListAsync();

            return View(eventList);
        }


        // GET: AdminEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminEvent = await _context.AdminEvents
                .Include(c => c.Hall)
                .Include(t => t.Tickets)
                .Include(u => u.Users)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (adminEvent == null)
            {
                return NotFound();
            }

            var eventUsersViewModel = new AdminEventUserViewModel
            {
                EventId = adminEvent.EventId,
                EventName = adminEvent.EventName,
                HallName = adminEvent.Hall.HallName,
                EventPrice = adminEvent.EventPrice,
                IsEventFinished = adminEvent.IsEventFinished,
                UserTickets = adminEvent.Tickets
                    .Select(ticket => new UserTicketViewModel
                    {
                        UserName = ticket.User.UserName,
                        NumberOfTickets = (int)ticket.NumberOfTickets,
                        FullName = $"{ticket.User.FirstName} {ticket.User.LastName}",
                        TicketId = ticket.TicketId
                    })
                    .ToList()
            };

            return View(eventUsersViewModel);
        }


        public IActionResult GetAvailableHalls(DateTime date)
        {
            var availableHalls = _context.Halls.Where(h => !h.Events.Any(e => e.Date.Date == date.Date)).OrderBy(h => h.Location)
                                                .Select(h => new { hallId = h.HallId, hallName = h.HallName, location = h.Location, venue = h.Venue });
            var unavailableHalls = _context.Halls.Where(h => h.Events.Any(e => e.Date.Date == date.Date)).OrderBy(h => h.Location)
                                                 .Select(h => new { hallId = h.HallId, hallName = h.HallName, location = h.Location, venue = h.Venue });

            return Json(new { availableHalls, unavailableHalls });
        }
        // GET: AdminEvents/Create
        public IActionResult Create()
        {
            ViewBag.Halls = _context.Halls.OrderBy(h => h.Location);
            //ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallId");
            return View();
        }

        // POST: AdminEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventPrice,ArtistData,ImagePath,EventId,Featured,EventType,EventName,Description,Date,ContactInformation,Duration,HallId")] AdminEvent adminEvent, IFormFile ImageFile)
        {
            ModelState.Clear();
            try
            {
                adminEvent.Hall = _context.Halls.FirstOrDefault(h => h.HallId == adminEvent.HallId);
                var currentUserId = _userManager.GetUserId(User);
                var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

                if (currentUser != null)
                {
                    adminEvent.Users = new List<ApplicationUser> { currentUser };
                }

                if (ModelState.IsValid)
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);

                        }

                        adminEvent.ImagePath = "Images/" + fileName;
                    }

                    _context.Add(adminEvent);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Halls = _context.Halls;
                return View(adminEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create method: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AdminEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AdminEvents == null)
            {
                return NotFound();
            }
            if (IsEventFinished((int)id))
            {
                return RedirectToAction("EventFinished", "NewHome");
            }
            var adminEvent = await _context.AdminEvents.FindAsync(id);
            adminEvent.Hall = _context.Halls.Find(adminEvent.HallId);

            if (adminEvent == null)
            {
                return NotFound();
            }
            ViewBag.Halls = _context.Halls.OrderBy(h => h.Location);
            return View(adminEvent);
        }

        // POST: AdminEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventPrice,ArtistData,Featured,ImagePath,EventId,EventType,EventName,Description,Date,ContactInformation,Duration,HallId")] AdminEvent adminEvent, IFormFile ImageFile)
        {
            if (id != adminEvent.EventId)
            {
                return NotFound();
            }
            ModelState.Clear();
            var paths = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", adminEvent.ImagePath.TrimStart('~', '/'));
            if (System.IO.File.Exists(paths))
            {
                System.IO.File.Delete(paths);
                Console.WriteLine("Image deleted successfully.");
            }
            else
            {
                Console.WriteLine("The specified image file does not exist.");
            }
            adminEvent.Hall = _context.Halls.FirstOrDefault(h => h.HallId == adminEvent.HallId);
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", adminEvent.ImagePath.TrimStart('~', '/'));

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                            Console.WriteLine("Image deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("The specified image file does not exist.");
                        }
                        adminEvent.ImagePath = "Images/" + fileName;
                    }
                    _context.Update(adminEvent);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in Edit method: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Halls = _context.Halls;
            return View(adminEvent);
        }

        // GET: AdminEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AdminEvents == null)
            {
                return NotFound();
            }
            if(IsEventFinished((int)id)){
                return RedirectToAction("EventFinished", "NewHome");
            }
            var adminEvent = await _context.AdminEvents
                .Include(c => c.Hall)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (adminEvent == null)
            {
                return NotFound();
            }

            return View(adminEvent);
        }

        // POST: AdminEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdminEvents == null)
            {
                return Problem("Entity set 'EventManagementContext.AdminEvents'  is null.");
            }
            var adminEvent = await _context.AdminEvents.FindAsync(id);

            if (adminEvent != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", adminEvent.ImagePath.TrimStart('~', '/'));
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    Console.WriteLine("Image deleted successfully.");
                }
                else
                {
                    Console.WriteLine("The specified image file does not exist.");
                }
                _context.AdminEvents.Remove(adminEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminEventExists(int id)
        {
            return (_context.AdminEvents?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
        public IActionResult CancelBooking(int EventId, int TicketId)
        {
            if (IsEventFinished(EventId))
            {
                return RedirectToAction("EventFinished", "NewHome");
            }
            var adminEvent = _context.AdminEvents.Include(e => e.Hall).FirstOrDefault(e=>e.EventId==EventId);
            var ticket = _context.Tickets.Find(TicketId);
            var username = _context.Users.Find(ticket.UserId).UserName;
            var viewModel = new CancelBookingByAdminViewModel()
            {
                AdminEvent = adminEvent,
                Ticket = ticket,
                Username = username
            };
            if (adminEvent == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        public IActionResult CancelConfirmed(int EventId, int TicketId)
        {
          
            var ticket = _context.Tickets.Find(TicketId);
            if (ticket == null)
            {
                return NotFound();
            }
            var @event = _context.Events.Find(EventId);
            if (@event == null)
            {
                return NotFound();
            }
            var user = _context.Users.Include(e => e.Events).First(u => u.Id == ticket.UserId);
            _context.Tickets.Remove(ticket);
            user.Events.Remove(@event);
            _context.SaveChanges();
            ViewBag.UserName = user.UserName;
            return View();

        }
        public bool IsEventFinished(int id)
        {
            return _context.AdminEvents.Find(id).IsEventFinished;
        }
    }
}
