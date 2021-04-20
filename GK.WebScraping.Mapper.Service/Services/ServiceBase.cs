using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GK.WebScraping.Mapper.Service
{
    public abstract class ServiceBase : BackgroundService
    {
        public readonly ILogger<ReaderService> _logger;
        public ServiceBase(ILogger<ReaderService> logger)
        {
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Init();
            this.Start();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Stop();
            this.Dispose();
            await base.StopAsync(cancellationToken);
        }

        protected abstract void Init();
        protected abstract void Start();
        protected abstract void Stop();
    }
}
