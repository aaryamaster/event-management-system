using EventManagement.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagement
{
    public class EventStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EventStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EventManagementContext>();
                    //var date = new DateTime(2024, 3, 31, 15, 31, 0);
                    var events = dbContext.Events.Where(e => e.Date < DateTime.Now);
                    if (events != null)
                    {
                        foreach (var e in events)
                        {
                            e.IsEventFinished = true;
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}

