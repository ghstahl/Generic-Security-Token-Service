using GraphQL.Language.AST;

namespace P7.GraphQLCore.Validators
{
    public class OptOutGraphQLAuthorizationCheck : IGraphQLAuthorizationCheck
    {
        private IAllUsersOptOutGraphQLAuthStore _graphQLAuthStore;
        public OptOutGraphQLAuthorizationCheck(IAllUsersOptOutGraphQLAuthStore graphQLAuthStore)
        {
            _graphQLAuthStore = graphQLAuthStore;
        }


        public bool ShouldDoAuthorizationCheck(OperationType operationTye, string fieldName)
        {
            var contains = _graphQLAuthStore.Contains(operationTye, fieldName);
            return !contains;
        }
    }
}