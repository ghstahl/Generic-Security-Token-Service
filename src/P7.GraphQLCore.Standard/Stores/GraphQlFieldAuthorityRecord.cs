using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using GraphQL.Language.AST;
using Newtonsoft.Json;

namespace P7.GraphQLCore.Stores
{
    public partial class ClaimConfigHandle
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class GraphQLFieldAuthorityConfigRecord
    {
        public OperationType OperationType { get; set; }
        public string FieldPath { get; set; }
        public List<ClaimConfigHandle> Claims { get; set; }
    }
    public class GraphQLFieldAuthorityRecord
    {
        public OperationType OperationType { get; set; }
        public string FieldPath { get; set; }
        public List<Claim> Claims { get; set; }
    }

    public class GraphQLFieldAuthorityConfig
    {
        public const string WellKnown_SectionName = "graphQLFieldAuthority";
        public List<GraphQLFieldAuthorityConfigRecord> Records { get; set; }
    }
}
