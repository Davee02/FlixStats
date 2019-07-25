using System;
using System.Threading.Tasks;
using FlixStats.Data.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FlixStats.Services.Schedule.Jobs
{
    public class DeleteOldResultsJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeleteOldResultsJob> _logger;


        public DeleteOldResultsJob(ILogger<DeleteOldResultsJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("Starting the Job.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<INetflixViewedItemRepository>();

                int deletedCount = await repository.DeleteOldResultsAsync();

                _logger.LogInformation($"Deleted {deletedCount} entities");
            }
            _logger.LogDebug("Ending the Job.");
        }
    }
}
