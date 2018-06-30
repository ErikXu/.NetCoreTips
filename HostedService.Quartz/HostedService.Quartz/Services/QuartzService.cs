using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HostedService.Quartz.Services
{
    public class QuartzService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        public QuartzService(ILogger<QuartzService> logger, IScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始Quartz调度...");
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("停止Quartz调度...");
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}