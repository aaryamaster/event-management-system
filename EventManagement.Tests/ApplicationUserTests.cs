using System.ComponentModel.DataAnnotations;
using EventManagement.Areas.Identity.Data;
using EventManagement.Models;
using Xunit;

namespace EventManagement.Tests
{
    public class ApplicationUserTests
    {
        [Fact]
        public void FirstName_ShouldBeSetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser();
            var firstName = "John";

            // Act
            user.FirstName = firstName;

            // Assert
            Assert.Equal(firstName, user.FirstName);
        }

        [Fact]
        public void LastName_ShouldBeSetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser();
            var lastName = "Doe";

            // Act
            user.LastName = lastName;

            // Assert
            Assert.Equal(lastName, user.LastName);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test@domain.com")]
        public void Email_ShouldBeSetCorrectly(string email)
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.Email = email;

            // Assert
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public void Events_ShouldBeSetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser();
            var events = new List<Event> { new Event(), new Event() };

            // Act
            user.Events = events;

            // Assert
            Assert.Equal(events, user.Events);
        }

        [Fact]
        public void Tickets_ShouldBeSetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser();
            var tickets = new List<Ticket> { new Ticket(), new Ticket() };

            // Act
            user.Tickets = tickets;

            // Assert
            Assert.Equal(tickets, user.Tickets);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid-email")]
        public void Email_ShouldBeInvalid(string email)
        {
            // Arrange
            var user = new ApplicationUser { Email = email };
            var validationContext = new ValidationContext(user) { MemberName = "Email" };
            var validationResults = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(user.Email, validationContext, validationResults);

            // Assert
            if (email == null)
            {
                Assert.Contains(validationResults, v => v.ErrorMessage == "The Email field is required.");
            }
            else
            {
                Assert.Contains(validationResults, v => v.ErrorMessage == "The Email field is not a valid e-mail address.");
            }
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test@domain.com")]
        public void Email_ShouldBeValid(string email)
        {
            // Arrange
            var user = new ApplicationUser { Email = email };

            // Act
            var result = ValidateModel(user);

            // Assert
            Assert.DoesNotContain(result, v => v.MemberNames.Contains("Email") && v.ErrorMessage.Contains("valid email"));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
    }
}