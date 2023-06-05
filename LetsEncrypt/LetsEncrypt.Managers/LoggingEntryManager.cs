using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Model;
using LetsEncrypt.Model.Enums;

namespace LetsEncrypt.Managers;

public class LoggingEntryManager : ILoggingEntryManager
{
    public Task<IList<LoggingEntry>> GetLoggingEntries()
    {
        return Task.FromResult((IList<LoggingEntry>)new List<LoggingEntry>
        {
            new()
            {
                Level = LoggingLevel.Info,
                Message = "This is a random message",
                DateTime = DateTime.UtcNow
            },
            new()
            {
                Level = LoggingLevel.Debug,
                Message = "This is another random debug message",
                DateTime = DateTime.UtcNow
            },
            new()
            {
                Level = LoggingLevel.Error,
                Message = "This is yet another random error",
                DateTime = DateTime.UtcNow
            }
        });
    }
}