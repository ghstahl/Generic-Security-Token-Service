using GraphQL.Language.AST;

namespace P7.GraphQLCore.Validators
{
    public interface IGraphQLAuthorizationCheck
    {
        bool ShouldDoAuthorizationCheck(OperationType operationTye, string fieldName);
    }
}