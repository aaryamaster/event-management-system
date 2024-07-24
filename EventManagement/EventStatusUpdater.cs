using EventManagement.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagement
{
    public class EventStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventStatusUpdater> _logger;

        public EventStatusUpdater(IServiceProvider serviceProvider, ILogger<EventStatusUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EventStatusUpdater is running.");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<EventManagementContext>();
                        _logger.LogInformation("Database context acquired.");

                        var events = dbContext.Events.Where(e => e.Date < DateTime.Now);
                        if (events != null && events.Any())
                        {
                            foreach (var e in events)
                            {
                                e.IsEventFinished = true;
                                _logger.LogInformation($"Event {e.EventId} marked as finished."); // Adjusted property name
                            }
                            await dbContext.SaveChangesAsync();
                            _logger.LogInformation("Database changes saved.");
                        }
                        else
                        {
                            _logger.LogInformation("No events to update.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating event statuses.");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
