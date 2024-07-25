using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EventManagement.Controllers;
using EventManagement.Models;
using EventManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using EventManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Moq;

namespace EventManagement.Tests
{
    public class NewHomeControllerTests
    {
        private ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // Configure in-memory database
            serviceCollection.AddDbContext<EventManagementContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Configure Identity services
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
            serviceCollection.AddScoped<SignInManager<ApplicationUser>>(sp =>
                new SignInManager<ApplicationUser>(
                    sp.GetRequiredService<UserManager<ApplicationUser>>(),
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<ApplicationUser>>().Object
                ));

            // Configure logger
            serviceCollection.AddSingleton<ILogger<NewHomeController>>(new Mock<ILogger<NewHomeController>>().Object);

            // Configure IHttpContextAccessor
            serviceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

            return serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void Index_ReturnsViewResult_WithFeaturedEvents()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var context = serviceProvider.GetRequiredService<EventManagementContext>();

            // Seed the database with test data
            context.AdminEvents.Add(new AdminEvent
            {
                EventName = "Test Event",
                Description = "Test Description",
                Featured = true,
                IsEventFinished = false,
                ContactInformation = "Test Contact Info",
                Duration = "2 hours",
                ImagePath = "test-image.jpg"
            });
            context.SaveChanges();

            // Create the controller with the service provider
            var controller = new TestNewHomeController(
                context,
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>(),
                serviceProvider.GetRequiredService<ILogger<NewHomeController>>()
            );

            // Act
            var result = controller.Index() as ViewResult;

            // Add logging to diagnose issues
            if (result == null)
            {
                Assert.True(false, "The result of Index action is null.");
            }

            var model = result?.Model as List<FeaturedEventViewModel>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal("Test Event", model.First().EventName);
        }
    }

    // Test-specific version of the NewHomeController
    public class TestNewHomeController : NewHomeController
    {
        public TestNewHomeController(
            EventManagementContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<NewHomeController> logger)
            : base(context, userManager, signInManager, logger)
        {
        }
    }
}