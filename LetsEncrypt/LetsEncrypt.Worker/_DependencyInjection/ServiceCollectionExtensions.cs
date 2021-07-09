using LetsEncrypt.Managers.DependencyInjection;
using LetsEncrypt.Worker.Processing;
using LetsEncrypt.Worker.Processing.Interfaces;
using LetsEncrypt.Worker.Workers;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace LetsEncrypt.Worker.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorker(this IServiceCollection services)
        {
            services.AddManagers();
            services.AddHostedService<CertificateOrderWorker>();

            services.AddScoped<ICertificateProcessor, CertificateProcessor>();
        }
    }
}