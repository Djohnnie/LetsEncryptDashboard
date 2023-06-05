using System.Collections.Generic;
using System.Threading.Tasks;
using LetsEncrypt.Model;

namespace LetsEncrypt.Managers.Interfaces;

public interface ICertificateEntryManager
{
    Task<IList<CertificateEntry>> GetCertificateEntries();
}