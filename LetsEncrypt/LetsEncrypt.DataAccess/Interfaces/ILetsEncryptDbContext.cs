using System.Threading;
using System.Threading.Tasks;
using LetsEncrypt.Model;
using Microsoft.EntityFrameworkCore;

namespace LetsEncrypt.DataAccess.Interfaces
{
    public interface ILetsEncryptDbContext
    {
        DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }
        DbSet<CertificateEntry> CertificateEntries { get; set; }
        DbSet<LoggingEntry> LoggingEntries { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}