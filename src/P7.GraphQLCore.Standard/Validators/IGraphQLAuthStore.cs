using GraphQL.Language.AST;

namespace P7.GraphQLCore.Validators
{
    public interface IGraphQLAuthStore
    {
        bool Contains(OperationType operationType, string fieldName);
    }
}