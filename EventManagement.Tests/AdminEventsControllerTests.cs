using System;
using System.Threading.Tasks;
using EventManagement.Areas.Identity.Data;
using EventManagement.Controllers;
using EventManagement.Models;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventManagement.Tests
{
    public class AdminEventsControllerTests
    {
        private readonly AdminEventsController _controller;
        private readonly EventManagementContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminEventsControllerTests()
        {
            // Setup in-memory database and UserManager
            var options = new DbContextOptionsBuilder<EventManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EventManagementContext(options);

            _userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(_context),
                null,
                new PasswordHasher<ApplicationUser>(),
                null,
                null,
                null,
                null,
                null,
                null
            );

            _controller = new AdminEventsController(_context, _userManager);

            // Seed the database
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Seed Hall
            _context.Halls.Add(new Hall
            {
                HallId = 1,
                HallName = "Main Hall",
                Location = "Center",
                Venue = "Venue A",
                GuestLimit = 100
            });

            // Seed AdminEvent
            _context.AdminEvents.Add(new AdminEvent
            {
                EventId = 1,
                EventName = "Test Event",
                Date = DateTime.Now,
                HallId = 1,
                EventPrice = 100.00, // double type
                Description = "This is a test event.",
                Duration = "2 hours",
                ContactInformation = "test@example.com",
                ImagePath = "Images/test-image.jpg", // Ensure this is set
                IsEventFinished = false
            });

            _context.SaveChanges();
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithModel()
        {
            // Arrange
            var validEventId = 1; // Ensure this ID exists in your test database

            // Act
            var result = await _controller.Details(validEventId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AdminEventUserViewModel>(viewResult.Model);

            Assert.Equal(validEventId, model.EventId);
            Assert.Equal("Test Event", model.EventName);
            Assert.Equal("Main Hall", model.HallName);
            Assert.Equal(100.00, model.EventPrice);
            Assert.False(model.IsEventFinished);
        }
    }
}