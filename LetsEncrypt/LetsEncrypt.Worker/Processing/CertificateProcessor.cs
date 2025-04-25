using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using LetsEncrypt.Model;
using System.Security.Cryptography.X509Certificates;

namespace LetsEncrypt.Worker.Processing;

internal class CertificateProcessor
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CertificateProcessor> _logger;

    public CertificateProcessor(
        IConfiguration configuration,
        ILogger<CertificateProcessor> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Process(CertificateEntry certificateEntry)
    {
        var notAfter = GetCertificateNotAfter(certificateEntry);

        if (notAfter > DateTime.UtcNow.AddDays(30))
        {
            certificateEntry.ExpiresOn = notAfter;

            return;
        }

        var acmeContext = await LoadAccount(certificateEntry);
        var order = await CreateOrder(acmeContext, certificateEntry);
        await ValidateOrder(order, certificateEntry);
        await GenerateOrder(order, certificateEntry);

        certificateEntry.RenewedOn = DateTime.UtcNow;
    }

    private DateTime GetCertificateNotAfter(CertificateEntry certificateEntry)
    {
        var notAfter = DateTime.MinValue;

        var certificatePath = _configuration.GetValue<string>("CERTIFICATE_PATH");

        var certPath = Path.Combine(certificatePath, $"{certificateEntry.DomainName}.pfx");
        var certPass = _configuration.GetValue<string>("CERTIFICATE_PASSWORD");

        if (File.Exists(certPath))
        {
            var collection = X509CertificateLoader.LoadPkcs12CollectionFromFile(certPath, certPass, X509KeyStorageFlags.PersistKeySet);

            foreach (var cert in collection)
            {
                notAfter = cert.NotAfter;
            }
        }

        return notAfter;
    }

    private async Task GenerateOrder(IOrderContext order, CertificateEntry certificateEntry)
    {
        _logger.LogInformation(" 7. Generating Certificate...");
        var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);

        var certificatePassword = _configuration.GetValue<string>("CERTIFICATE_PASSWORD");
        var certificatePath = _configuration.GetValue<string>("CERTIFICATE_PATH");

        var cert = await order.Generate(new CsrInfo
        {
            CommonName = certificateEntry.DomainName,
            Organization = certificateEntry.Organization,
            OrganizationUnit = certificateEntry.OrganizationUnit,
            CountryName = certificateEntry.CountryName,
            State = certificateEntry.State,
            Locality = certificateEntry.Locality
        }, privateKey);

        _logger.LogInformation(" 8. Building PFX...");
        var pfxBuilder = cert.ToPfx(privateKey);
        var pfx = pfxBuilder.Build(certificateEntry.DomainName, certificatePassword);
        await File.WriteAllBytesAsync(Path.Combine(certificatePath, $"{certificateEntry.DomainName}.pfx"), pfx);

        var x509Cert = X509CertificateLoader.LoadPkcs12(pfx, certificatePassword, X509KeyStorageFlags.Exportable);
        var x509CertRawData = x509Cert.GetRawCertData();
        await File.WriteAllBytesAsync(Path.Combine(certificatePath, $"{certificateEntry.DomainName}.cer"), x509CertRawData);

        var x509CertPrivateKey = x509Cert.GetECDsaPrivateKey();
        var x509CertPrivateKeyPem = x509CertPrivateKey.ExportECPrivateKeyPem();
        await File.WriteAllTextAsync(Path.Combine(certificatePath, $"{certificateEntry.DomainName}.key"), x509CertPrivateKeyPem);
    }

    private async Task ValidateOrder(IOrderContext order, CertificateEntry certificateEntry)
    {
        _logger.LogInformation($" 4. Validating domain {certificateEntry.DomainName}...");
        var authz = (await order.Authorizations()).First();
        var httpChallenge = await authz.Http();
        var keyAuthz = httpChallenge.KeyAuthz;

        _logger.LogInformation(" 5. Writing challenge file");
        var challengePath = _configuration.GetValue<string>("CHALLENGE_PATH");
        var tokens = keyAuthz.Split('.');
        await File.WriteAllTextAsync(Path.Combine(challengePath, tokens[0]), keyAuthz);

        var chall = await httpChallenge.Validate();

        while (chall.Status == ChallengeStatus.Pending)
        {
            await Task.Delay(10000);
            chall = await httpChallenge.Validate();
        }

        if (chall.Status == ChallengeStatus.Valid)
        {
            _logger.LogInformation($" 6. Domain {certificateEntry.DomainName} is valid!");
        }

        if (chall.Status == ChallengeStatus.Invalid)
        {
            _logger.LogInformation($" 6. Domain {certificateEntry.DomainName} is NOT valid! {chall.Error.Detail}");
        }
    }

    private async Task<IOrderContext> CreateOrder(AcmeContext acme, CertificateEntry certificateEntry)
    {
        _logger.LogInformation($" 3. Creating order {certificateEntry.DomainName}...");
        return await acme.NewOrder(new[] { certificateEntry.DomainName });
    }

    private async Task<AcmeContext> LoadAccount(CertificateEntry certificateEntry)
    {
        AcmeContext acme;

        var server = certificateEntry.IsStaging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2;
        _logger.LogInformation($" 1. Setting Environment {server}...");

        if (string.IsNullOrEmpty(certificateEntry.AccountPem))
        {
            _logger.LogInformation(" 2. Creating account...");
            acme = new AcmeContext(server);
            var account = await acme.NewAccount(certificateEntry.Email, true);
            certificateEntry.AccountPem = acme.AccountKey.ToPem();
        }
        else
        {
            _logger.LogInformation(" 2. Using existing account...");
            var accountKey = KeyFactory.FromPem(certificateEntry.AccountPem);
            acme = new AcmeContext(server, accountKey);
        }

        return acme;
    }
}