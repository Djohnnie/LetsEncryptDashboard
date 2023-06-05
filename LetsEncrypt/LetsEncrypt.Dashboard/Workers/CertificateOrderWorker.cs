using LetsEncrypt.Managers.Interfaces;
using System.Diagnostics;

namespace LetsEncrypt.Dashboard.Workers;

public class CertificateOrderWorker : BackgroundService
{
    private readonly IConfigurationManager _configurationManager;
    private readonly ICertificateEntryManager _certificateEntryManager;
    private readonly ILogger<CertificateOrderWorker> _logger;

    public CertificateOrderWorker(
        IConfigurationManager configurationManager,
        ICertificateEntryManager certificateEntryManager,
        ILogger<CertificateOrderWorker> logger)
    {
        _configurationManager = configurationManager;
        _certificateEntryManager = certificateEntryManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var delay = 0;

            try
            {
                var stopwatch = Stopwatch.StartNew();
                delay = await _configurationManager.GetWorkerDelay();
                var certificateEntries = await _certificateEntryManager.GetCertificateEntries();

                foreach (var certificateEntry in certificateEntries)
                {
                    _logger.LogInformation("Processing: {certificateDomainName}", certificateEntry.DomainName);
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
