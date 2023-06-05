using System;
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
}