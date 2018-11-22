using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.Core.Utils;

namespace IdentityServerRequestTracker.Usage.Services
{
    public class InMemoryClientUsageStore : IClientUsageStore
    {
        private List<IAggregatedClientUsageRecordReadWrite> _clientRecords;

        private List<IAggregatedClientUsageRecordReadWrite> ClientRecords =>
            _clientRecords ?? (_clientRecords = new List<IAggregatedClientUsageRecordReadWrite>());

        public InMemoryClientUsageStore()
        {
        }

        public async Task TrackAsync(ClientUsageRecord record)
        {
            Guard.ArgumentNotNull(nameof(record), record);
            Guard.ArgumentNotNull(nameof(record.ClientId), record.ClientId);
            Guard.ArgumentNotNull(nameof(record.GrantType), record.GrantType);
            Guard.ArgumentNotNull(nameof(record.DateTime), record.DateTime);
            Guard.ArgumentNotNull(nameof(record.EndPointKey), record.EndPointKey);

            DateTime dateTimeUtcCurrentDay =
                new DateTime(record.DateTime.Year, record.DateTime.Month, record.DateTime.Day);
            DateTime dateTimeUtcNextDay = dateTimeUtcCurrentDay.AddDays(1);

            lock (ClientRecords)
            {
                var query = from item in ClientRecords
                    let c = item as IAggregatedClientUsageRecordWrite
                    where (c.ClientId == record.ClientId)
                          && (c.GrantType == record.GrantType)
                          && (c.EndPointKey == record.EndPointKey)
                          && (dateTimeUtcCurrentDay == c.DateRange.Item1 && dateTimeUtcNextDay == c.DateRange.Item2)
                    select item;
                IAggregatedClientUsageRecordReadWrite currentAggregatedClientUsageRecord = query.FirstOrDefault();
                if (currentAggregatedClientUsageRecord == null)
                {
                    currentAggregatedClientUsageRecord = new AggregatedClientUsageRecord()
                    {
                        ClientId = record.ClientId,
                        EndPointKey = record.EndPointKey,
                        GrantType = record.GrantType,
                        DateRange = (dateTimeUtcCurrentDay, dateTimeUtcNextDay)
                    };
                    ClientRecords.Add(currentAggregatedClientUsageRecord);
                }

                IAggregatedClientUsageRecordWrite writer = currentAggregatedClientUsageRecord;

                writer.Count += 1;
            }
        }

        public async Task<IEnumerable<IAggregatedClientUsageRecordRead>> GetClientUsageRecordsAsync(
            string clientId, string grantType, (DateTime?, DateTime?) range)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(clientId),clientId);
            DateTime dateTimeUtcCurrentDay;
            DateTime dateTimeUtcNextDay;
            if (range.Item1 == null || range.Item2 == null)
            {
                dateTimeUtcCurrentDay = DateTime.UtcNow;
                dateTimeUtcCurrentDay = new DateTime(
                    dateTimeUtcCurrentDay.Year, 
                    dateTimeUtcCurrentDay.Month, 
                    dateTimeUtcCurrentDay.Day);
                dateTimeUtcNextDay = dateTimeUtcCurrentDay.AddDays(1);

            }
            else
            {
                DateTime item1 = (DateTime) range.Item1;
                DateTime item2 = (DateTime)range.Item2;
                dateTimeUtcCurrentDay =
                    new DateTime(item1.Year, item1.Month, item1.Day);
                dateTimeUtcNextDay =
                    new DateTime(item2.Year, item2.Month, item2.Day);
                dateTimeUtcNextDay = dateTimeUtcNextDay.AddDays(1);
            }

            if (string.IsNullOrEmpty(grantType))
            {
                var query = from item in ClientRecords
                    let c = item as IAggregatedClientUsageRecordWrite
                    where (c.ClientId == clientId)
                          && (dateTimeUtcCurrentDay == c.DateRange.Item1 && dateTimeUtcNextDay == c.DateRange.Item2)
                    select item as IAggregatedClientUsageRecordRead;
                return query;
            }
            else
            {
                var query = from item in ClientRecords
                    let c = item as IAggregatedClientUsageRecordWrite
                    where (c.ClientId == clientId)
                          && (c.GrantType == grantType)
                          && (dateTimeUtcCurrentDay == c.DateRange.Item1 && dateTimeUtcNextDay == c.DateRange.Item2)

                    select item as IAggregatedClientUsageRecordRead;
                return query;
            }
        }
    }
}