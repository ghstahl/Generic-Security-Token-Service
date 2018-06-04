using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Language.AST;
using Microsoft.Extensions.Options;

namespace P7.GraphQLCore.Stores
{
    public class InMemoryGraphQLFieldAuthority : IGraphQLFieldAuthority
    {
        private IOptions<GraphQLFieldAuthorityConfig> _settings;
        private List<GraphQLFieldAuthorityRecord> _listGraphQLFieldAuthorityRecords;

        public InMemoryGraphQLFieldAuthority(IOptions<GraphQLFieldAuthorityConfig> settings)
        {
            _settings = settings;
            if (_settings != null && _settings.Value.Records != null)
            {

                foreach (var record in _settings.Value.Records)
                {
                    if (record.Claims == null)
                    {
                        record.Claims = new List<ClaimConfigHandle>();
                    }

                    var query = from item in record.Claims
                        let c = new Claim(item.Type, item.Value)
                        select c;
                    var claims = query.ToList();
                    AddClaims(record.OperationType, record.FieldPath, claims);

                }
            }
        }

        private List<GraphQLFieldAuthorityRecord> GraphQLFieldAuthorityRecords
        {
            get
            {
                return _listGraphQLFieldAuthorityRecords ??
                       (_listGraphQLFieldAuthorityRecords = new List<GraphQLFieldAuthorityRecord>());
            }
        }
        public async Task<FetchRequireClaimsResult<IEnumerable<Claim>>> FetchRequiredClaimsAsync(OperationType operationType, string fieldPath)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                        where item.OperationType == operationType && fieldPath == item.FieldPath
                        select item;
            GraphQLFieldAuthorityRecord record;
            var result = new FetchRequireClaimsResult<IEnumerable<Claim>>()
            {
                StatusCode = query.Any() ? GraphQLFieldAuthority_CODE.FOUND : GraphQLFieldAuthority_CODE.NOT_FOUND,
                Value = !query.Any() ? new List<Claim>() : query.FirstOrDefault().Claims
            };

            return result;
        }

        public void AddClaims(OperationType operationType, string fieldPath, List<Claim> claims)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                        where item.OperationType == operationType && item.FieldPath == fieldPath
                        select item;
            GraphQLFieldAuthorityRecord record;
            if (claims == null)
            {
                claims = new List<Claim>();
            }
            if (!query.Any())
            {
                record = new GraphQLFieldAuthorityRecord()
                {
                    OperationType = operationType,
                    FieldPath = fieldPath,
                    Claims = claims
                };
                GraphQLFieldAuthorityRecords.Add(record);
            }
            else
            {
                record = query.FirstOrDefault();
                var result = record.Claims.Union(claims).ToList();
                record.Claims = result;
            }
        }
        public async Task AddClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims)
        {
             AddClaims(operationType, fieldPath, claims);
        }
        public async Task RemoveClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                        where item.OperationType == operationType
                        select item;
            GraphQLFieldAuthorityRecord record;
            if (claims == null)
            {
                claims = new List<Claim>();
            }
            if (query.Any())
            {
                record = query.FirstOrDefault();
                var result = record.Claims.Except(claims).ToList();
                record.Claims = result;
            }
        }
    }
}