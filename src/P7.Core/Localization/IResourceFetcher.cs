using System.Threading.Tasks;

namespace P7.Core.Localization
{
    public interface IResourceFetcher
    {
        object GetResourceSet(ResourceQueryHandle input);
        Task<object> GetResourceSetAsync(ResourceQueryHandle input);
    }
}