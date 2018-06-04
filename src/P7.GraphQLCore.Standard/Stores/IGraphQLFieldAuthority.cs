using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Language.AST;

namespace P7.GraphQLCore.Stores
{
    public enum GraphQLFieldAuthority_CODE
    {
        FOUND = 0,
        NOT_FOUND = 1
    }
    public class FetchRequireClaimsResult<T>
    {
        public T Value { get; set; }
        public GraphQLFieldAuthority_CODE StatusCode { get; set; }
    }
    public interface IGraphQLFieldAuthority
    {
        Task<FetchRequireClaimsResult<IEnumerable<Claim>>> FetchRequiredClaimsAsync(OperationType operationType, string fieldPath);
    }
}
