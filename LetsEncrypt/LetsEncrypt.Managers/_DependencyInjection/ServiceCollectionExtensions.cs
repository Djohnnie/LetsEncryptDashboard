using LetsEncrypt.DataAccess.DependencyInjection;
using LetsEncrypt.Managers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LetsEncrypt.Managers.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddManagers(this IServiceCollection services)
    {
        services.AddDataAccess();

        services.AddScoped<ICertificateEntryManager, CertificateEntryManager>();
        services.AddScoped<ILoggingEntryManager, LoggingEntryManager>();
    }
}