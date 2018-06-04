using System.Security.Claims;
using GraphQL.Language.AST;

namespace P7.GraphQLCore.Validators
{
    public interface IGraphQLClaimsAuthorizationCheck
    {
        bool ShouldDoAuthorizationCheck(ClaimsPrincipal claimsPrincipal,OperationType operationTye, string fieldName);
    }
}