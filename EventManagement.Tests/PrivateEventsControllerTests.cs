using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Areas.Identity.Data;
using EventManagement.Controllers;
using EventManagement.Models;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class PrivateEventsControllerTests
    {
        private ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // Configure in-memory database
            serviceCollection.AddDbContext<EventManagementContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Configure UserManager with a mock object
            serviceCollection.AddScoped<UserManager<ApplicationUser>>(sp =>
                new UserManager<ApplicationUser>(
                    new Mock<IUserStore<ApplicationUser>>().Object,
                    null,
                    new PasswordHasher<ApplicationUser>(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                ));

            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithUserOrganizedEvents()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var context = serviceProvider.GetRequiredService<EventManagementContext>();

            // Seed the database with test data
            var hall = new Hall
            {
                HallId = 1,
                HallName = "Main Hall",
                Location = "First Floor",
                Venue = "City Center",
                GuestLimit = 100,
                HallPrice = 5000
            };

            context.Halls.Add(hall);

            var user = new ApplicationUser
            {
                Id = "user1",
                UserName = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            context.Users.Add(user);

            var privateEvent = new PrivateEvent
            {
                EventId = 1,
                EventName = "Private Event 1",
                Date = DateTime.Now,
                NumberOfGuests = 50,
                FoodType = FoodType.Vegetarian,
                Description = "Test Description",
                Duration = "2",
                ContactInformation = "Test Contact Info",
                IsEventFinished = false,
                Hall = hall,
                Users = new List<ApplicationUser> { user }
            };

            context.PrivateEvents.Add(privateEvent);
            context.SaveChanges();

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var controller = new PrivateEventsController(context, userManager);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as IList<UserOrganizedEventViewModel>;
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal("Private Event 1", model.First().EventName);
        }
    }
}