using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Scheduler.Stores;

namespace P7.Core.Scheduler
{
    public class QuoteOfTheDayTas3k : IScheduledTask
    {
        public string Schedule => "* */6 * * *";
    }

    public class QuoteOfTheDayTask : IScheduledTask
    {
        private IQuoteOfTheDataStore QuoteOfTheDataStore { get; set; }
        public QuoteOfTheDayTask(IQuoteOfTheDataStore store)
        {
            QuoteOfTheDataStore = store;
        }
        public string Schedule => "* */6 * * *";

        public async Task Invoke(CancellationToken cancellationToken)
        {
            try
            {
                var httpClient = new HttpClient();

                var quoteJson = JObject.Parse(await httpClient.GetStringAsync("http://quotes.rest/qod.json"));

                QuoteOfTheDay.Current = JsonConvert.DeserializeObject<QuoteOfTheDay>(quoteJson["contents"]["quotes"][0].ToString());
                await QuoteOfTheDataStore.SetQuoteAsync(QuoteOfTheDay.Current);
            }
            catch ( Exception e)
            {
                throw e;
            }
            
        }
    }
}