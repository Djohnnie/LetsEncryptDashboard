using Certes.Acme.Resource;
using Certes.Acme;
using Certes;
using System.Security.Cryptography.X509Certificates;
using LetsEncrypt.Model;

namespace LetsEncrypt.Dashboard.Processing;

public class CertificateProcessor
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

        if(notAfter > DateTime.UtcNow.AddDays(28))
        {
            certificateEntry.ExpiresOn = notAfter;

            return;
        }

        var acmeContext = await LoadAccount(certificateEntry);
    }

    private DateTime GetCertificateNotAfter(CertificateEntry certificateEntry)
    {
        throw new NotImplementedException();
    }

    private (bool, string) CertificateIsAboutToExpire()
    {
        var certPath = Path.Combine(_configuration.CertificatePath, $"{_configuration.DomainName}.pfx");
        var certPass = _configuration.CertificatePassword;

        if (!File.Exists(certPath))
        {
            return (true, string.Empty);
        }

        bool isAboutToExpire = false;
        string notAfter = string.Empty;

        X509Certificate2Collection collection = new X509Certificate2Collection();
        collection.Import(certPath, certPass, X509KeyStorageFlags.PersistKeySet);
        foreach (var cert in collection)
        {
            isAboutToExpire = isAboutToExpire || cert.NotAfter < DateTime.Today.AddDays(7);
            notAfter = $"{cert.NotAfter:dd-MM-yyyy}";
        }

        return (isAboutToExpire, notAfter);
    }

    private async Task GenerateOrder(IOrderContext order)
    {
        Log(" 7. Generating Certificate...");
        var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);

        var cert = await order.Generate(new CsrInfo
        {
            CountryName = _configuration.Country,
            State = _configuration.State,
            Locality = _configuration.Locality,
            Organization = _configuration.Organization,
            OrganizationUnit = _configuration.Unit,
            CommonName = _configuration.DomainName,
        }, privateKey);

        Log(" 8. Building PFX...");
        var pfxBuilder = cert.ToPfx(privateKey);
        var pfx = pfxBuilder.Build(_configuration.DomainName, _configuration.CertificatePassword);
        File.WriteAllBytes(Path.Combine(_configuration.CertificatePath, $"{_configuration.DomainName}.pfx"), pfx);
    }

    private async Task ValidateOrder(IOrderContext order)
    {
        Log($" 4. Validating domain {_configuration.DomainName}...");
        var authz = (await order.Authorizations()).First();
        var httpChallenge = await authz.Http();
        var keyAuthz = httpChallenge.KeyAuthz;

        Log(" 5. Writing challenge file");
        var tokens = keyAuthz.Split('.');
        await File.WriteAllTextAsync(Path.Combine(_configuration.ChallengePath, tokens[0]), keyAuthz);

        var chall = await httpChallenge.Validate();

        while (chall.Status == ChallengeStatus.Pending)
        {
            await Task.Delay(10000);
            chall = await httpChallenge.Validate();
        }

        if (chall.Status == ChallengeStatus.Valid)
        {
            Log($" 6. Domain {_configuration.DomainName} is valid!");
        }

        if (chall.Status == ChallengeStatus.Invalid)
        {
            Log($" 6. Domain {_configuration.DomainName} is NOT valid! {chall.Error.Detail}");
        }
    }

    private async Task<IOrderContext> CreateOrder(AcmeContext acme)
    {
        Log($" 3. Creating order {_configuration.DomainName}...");
        return await acme.NewOrder(new[] { _configuration.DomainName });
    }

    private async Task<AcmeContext> LoadAccount(CertificateEntry certificateEntry)
    {
        AcmeContext acme;

        var server = certificateEntry.IsStaging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2;
        Log($" 1. Setting Environment {server}...");

        if (string.IsNullOrEmpty(_configuration.AccountPem))
        {
            Log(" 2. Creating account...");
            acme = new AcmeContext(server);
            var account = await acme.NewAccount(certificateEntry.Email, true);
            _configuration.AccountPem = acme.AccountKey.ToPem();
        }
        else
        {
            Log(" 2. Using existing account...");
            var accountKey = KeyFactory.FromPem(_configuration.AccountPem);
            acme = new AcmeContext(server, accountKey);
        }

        return acme;
    }
}