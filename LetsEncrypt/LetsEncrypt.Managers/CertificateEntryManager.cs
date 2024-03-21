using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetsEncrypt.DataAccess.Interfaces;
using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Model;
using Microsoft.EntityFrameworkCore;

namespace LetsEncrypt.Managers;

public class CertificateEntryManager : ICertificateEntryManager
{
    private readonly ILetsEncryptDbContext _dbContext;

    public CertificateEntryManager(ILetsEncryptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<CertificateEntry>> GetCertificateEntries()
    {
        var certificateEntries = await _dbContext.CertificateEntries
            .OrderBy(x => x.ExpiresOn).ToListAsync();
        return certificateEntries;
    }

    public async Task AddCertificateEntry(CertificateEntry certificateEntry)
    {
        _dbContext.CertificateEntries.Add(certificateEntry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCertificateEntry(CertificateEntry certificateEntry)
    {
        await _dbContext.CertificateEntries.Where(x => x.Id == certificateEntry.Id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.RenewedOn, certificateEntry.RenewedOn)
                .SetProperty(p => p.ExpiresOn, certificateEntry.ExpiresOn)
                .SetProperty(p => p.AccountPem, certificateEntry.AccountPem)
                .SetProperty(p => p.LastError, certificateEntry.LastError));
    }

    public async Task RemoveCertificateEntry(CertificateEntry certificateEntry)
    {
        await _dbContext.CertificateEntries.Where(x => x.Id == certificateEntry.Id).ExecuteDeleteAsync();
    }
}