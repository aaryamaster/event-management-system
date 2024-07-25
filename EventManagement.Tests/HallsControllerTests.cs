using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Controllers;
using EventManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventManagement.Tests
{
    public class HallsControllerTests
    {
        private readonly HallsController _controller;
        private readonly EventManagementContext _context;

        public HallsControllerTests()
        {
            var options = new DbContextOptionsBuilder<EventManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EventManagementContext(options);
            _controller = new HallsController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Halls.Add(new Hall
            {
                HallId = 1,
                HallName = "Main Hall",
                Venue = "Venue A",
                HallPrice = 200.00,
                Location = "Center",
                GuestLimit = 100
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfHalls()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Hall>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewResult_WithHall()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Hall>(viewResult.Model);
            Assert.Equal(1, model.HallId);
        }

        [Fact]
        public async Task Create_PostValidHall_RedirectsToIndex()
        {
            // Arrange
            var newHall = new Hall
            {
                HallName = "New Hall",
                Venue = "Venue B",
                HallPrice = 300.00,
                Location = "North",
                GuestLimit = 150
            };

            // Act
            var result = await _controller.Create(newHall);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var hall = _context.Halls.SingleOrDefault(h => h.HallName == "New Hall");
            Assert.NotNull(hall);
        }

        [Fact]
        public async Task Edit_ValidId_UpdatesHall()
        {
            // Arrange
            var hall = await _context.Halls.FindAsync(1);
            hall.HallName = "Updated Hall";

            // Act
            var result = await _controller.Edit(1, hall);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var updatedHall = await _context.Halls.FindAsync(1);
            Assert.Equal("Updated Hall", updatedHall.HallName);
        }

        [Fact]
        public async Task Delete_ValidId_RemovesHall()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var hall = await _context.Halls.FindAsync(1);
            Assert.Null(hall);
        }
    }
}