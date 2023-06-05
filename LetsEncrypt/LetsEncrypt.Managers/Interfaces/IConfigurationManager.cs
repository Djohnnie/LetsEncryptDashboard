using System.Threading.Tasks;

namespace LetsEncrypt.Managers.Interfaces;

public interface IConfigurationManager
{
    Task<int> GetWorkerDelay();
}