using LetsEncrypt.Dashboard.Processing;
using LetsEncrypt.Managers.Interfaces;
using System.Diagnostics;

namespace LetsEncrypt.Dashboard.Workers;

public class CertificateWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CertificateWorker> _logger;

    public CertificateWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CertificateWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
            var configurationManager = serviceScope.ServiceProvider.GetRequiredService<IConfigurationManager>();
            var certificateEntryManager = serviceScope.ServiceProvider.GetRequiredService<ICertificateEntryManager>();
            var certificateProcessor = serviceScope.ServiceProvider.GetRequiredService<CertificateProcessor>();

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var delay = 0;

            try
            {
                var stopwatch = Stopwatch.StartNew();
                delay = await configurationManager.GetWorkerDelay();
                var certificateEntries = await certificateEntryManager.GetCertificateEntries();

                foreach (var certificateEntry in certificateEntries)
                {
                    _logger.LogInformation("Verifying '{certificateDomainName}'", certificateEntry.DomainName);

                    if (certificateEntry.ExpiresOn < DateTime.UtcNow.Date.AddDays(28))
                    {
                        await certificateProcessor.Process(certificateEntry);
                        await certificateEntryManager.UpdateCertificateEntry(certificateEntry);
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