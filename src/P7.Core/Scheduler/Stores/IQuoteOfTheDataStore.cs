using System.Threading.Tasks;

namespace P7.Core.Scheduler.Stores
{
    public interface IQuoteOfTheDataStore
    {
        Task SetQuoteAsync(QuoteOfTheDay quote);
        Task<QuoteOfTheDay> GetQuoteAsync();
    }
}
