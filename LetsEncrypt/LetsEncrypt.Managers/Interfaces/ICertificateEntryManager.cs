using System.Collections.Generic;
using System.Threading.Tasks;
using LetsEncrypt.Model;

namespace LetsEncrypt.Managers.Interfaces;

public interface ICertificateEntryManager
{
    Task<IList<CertificateEntry>> GetCertificateEntries();
    Task AddCertificateEntry(CertificateEntry certificateEntry);
    Task UpdateCertificateEntry(CertificateEntry certificateEntry);
    Task RemoveCertificateEntry(CertificateEntry certificateEntry);
}