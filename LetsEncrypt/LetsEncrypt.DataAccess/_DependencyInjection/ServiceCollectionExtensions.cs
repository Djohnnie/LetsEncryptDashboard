using LetsEncrypt.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace LetsEncrypt.DataAccess.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<ILetsEncryptDbContext, LetsEncryptDbContext>();
        }
    }
}