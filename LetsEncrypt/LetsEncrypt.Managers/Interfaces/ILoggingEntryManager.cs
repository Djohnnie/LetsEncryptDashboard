using System.Collections.Generic;
using System.Threading.Tasks;
using LetsEncrypt.Model;

namespace LetsEncrypt.Managers.Interfaces;

public interface ILoggingEntryManager
{
    Task<IList<LoggingEntry>> GetLoggingEntries();
}