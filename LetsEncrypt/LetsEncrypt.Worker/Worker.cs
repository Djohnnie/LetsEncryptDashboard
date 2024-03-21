using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Worker.Processing;
using System.Diagnostics;

namespace LetsEncrypt.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICertificateEntryManager _certificateEntryManager;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
                var certificateEntryManager = serviceScope.ServiceProvider.GetRequiredService<ICertificateEntryManager>();
                var certificateProcessor = serviceScope.ServiceProvider.GetRequiredService<CertificateProcessor>();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var delay = 0;

                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    delay = _configuration.GetValue<int>("WORKER_INTERVAL") * 1000 * 60;
                    var certificateEntries = await certificateEntryManager.GetCertificateEntries();

                    foreach (var certificateEntry in certificateEntries)
                    {
                        _logger.LogInformation("Verifying '{certificateDomainName}'", certificateEntry.DomainName);

                        if (certificateEntry.ExpiresOn == null || certificateEntry.ExpiresOn < DateTime.UtcNow.Date.AddDays(30))
                        {
                            try
                            {
                                await certificateProcessor.Process(certificateEntry);
                                certificateEntry.LastError = null;
                            }
                            catch (Exception ex)
                            {
                                certificateEntry.LastError = ex.Message;
                                _logger.LogError("Error while processing '{certificateEntry}': {exceptionMessage}", certificateEntry.DomainName, ex.Message);
                            }

                            await certificateEntryManager.UpdateCertificateEntry(certificateEntry);

                            await Task.Delay(1000);
                        }
                    }

                    stopwatch.Stop();

                    delay -= (int)stopwatch.ElapsedMilliseconds;
                    delay = delay < 0 ? 0 : delay;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unhandled exception: {exceptionMessage}", ex.Message);
                }

                _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}