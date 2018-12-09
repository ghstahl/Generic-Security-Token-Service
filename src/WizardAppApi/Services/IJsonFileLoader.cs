using System.Threading.Tasks;

namespace WizardAppApi.Services
{
    public interface IJsonFileLoader
    {
        Task<object> LoadAsync(string pathFragment);

    }
}