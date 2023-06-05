using System;
using System.Threading.Tasks;
using LetsEncrypt.DataAccess.Interfaces;
using LetsEncrypt.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LetsEncrypt.Managers;

public class ConfigurationManager : IConfigurationManager
{
    private readonly ILetsEncryptDbContext _dbContext;

    public ConfigurationManager(ILetsEncryptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> GetWorkerDelay()
    {
        return GetValue("WORKER_DELAY", Convert.ToInt32);
    }

    private async Task<T> GetValue<T>(string configurationName, Func<string, T> conversion)
    {
        var configurationValue =
            await _dbContext.ConfigurationSettings.SingleOrDefaultAsync(x => x.Name == configurationName);

        return conversion(configurationValue.Value);
    }
}