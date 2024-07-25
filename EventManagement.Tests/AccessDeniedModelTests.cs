using EventManagement.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace EventManagement.Tests
{
    public class AccessDeniedModelTests
    {
        [Fact]
        public void OnGet_ReturnsPageResult()
        {
            // Arrange
            var model = new AccessDeniedModel();

            // Act
            model.OnGet();

            // Assert
            Assert.IsType<AccessDeniedModel>(model);
        }
    }
}