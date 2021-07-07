using LetsEncrypt.DataAccess.Extensions;
using LetsEncrypt.Model;
using Microsoft.EntityFrameworkCore;

namespace LetsEncrypt.DataAccess
{
    public class LetsEncryptDbContext : DbContext
    {
        public DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }
        public DbSet<CertificateEntry> CertificateEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigurationSetting>(entityBuilder =>
            {
                entityBuilder.ToTable("CONFIGURATION_SETTINGS");
                entityBuilder.HasIdAndSysId();
                entityBuilder.Property(p => p.Name).IsRequired();
                entityBuilder.Property(p => p.Value).IsRequired();
            });

            modelBuilder.Entity<CertificateEntry>(entityBuilder =>
            {
                entityBuilder.ToTable("CERTIFICATE_ENTRIES");
                entityBuilder.HasIdAndSysId();
                entityBuilder.Property(p => p.CountryName).IsRequired();
                entityBuilder.Property(p => p.State).IsRequired();
                entityBuilder.Property(p => p.Locality).IsRequired();
                entityBuilder.Property(p => p.Organization).IsRequired();
                entityBuilder.Property(p => p.OrganizationUnit).IsRequired();
                entityBuilder.Property(p => p.DomainName).IsRequired();
            });
        }
    }
}