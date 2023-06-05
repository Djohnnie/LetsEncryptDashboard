using LetsEncrypt.DataAccess.Extensions;
using LetsEncrypt.DataAccess.Interfaces;
using LetsEncrypt.Model;
using LetsEncrypt.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace LetsEncrypt.DataAccess;

public class LetsEncryptDbContext : DbContext, ILetsEncryptDbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }
    public DbSet<CertificateEntry> CertificateEntries { get; set; }
    public DbSet<LoggingEntry> LoggingEntries { get; set; }

    public LetsEncryptDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetValue<string>("CONNECTION_STRING"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationSetting>(entityBuilder =>
        {
            entityBuilder.ToTable("CONFIGURATION_SETTINGS");
            entityBuilder.HasIdAndSysId();
            entityBuilder.Property(p => p.Name)
                .IsRequired();
            entityBuilder.Property(p => p.Value)
                .IsRequired();
        });

        modelBuilder.Entity<CertificateEntry>(entityBuilder =>
        {
            entityBuilder.ToTable("CERTIFICATE_ENTRIES");
            entityBuilder.HasIdAndSysId();
            entityBuilder.Property(p => p.CountryName)
                .IsRequired();
            entityBuilder.Property(p => p.State)
                .IsRequired();
            entityBuilder.Property(p => p.Locality)
                .IsRequired();
            entityBuilder.Property(p => p.Organization)
                .IsRequired();
            entityBuilder.Property(p => p.OrganizationUnit)
                .IsRequired();
            entityBuilder.Property(p => p.DomainName)
                .IsRequired();
        });

        modelBuilder.Entity<LoggingEntry>(entityBuilder =>
        {
            entityBuilder.ToTable("LOGGING_ENTRIES");
            entityBuilder.HasIdAndSysId();
            entityBuilder.Property(p => p.Level)
                .HasConversion(new EnumToStringConverter<LoggingLevel>())
                .IsRequired();
            entityBuilder.Property(p => p.Message)
                .IsRequired();
            entityBuilder.Property(p => p.DateTime)
                .IsRequired();
            entityBuilder.HasIndex(p => p.DateTime);
        });
    }
}