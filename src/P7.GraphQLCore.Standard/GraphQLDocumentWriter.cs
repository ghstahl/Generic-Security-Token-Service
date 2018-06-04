using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Http;
using P7.Core.Writers;

namespace P7.GraphQLCore
{
    public class GraphQLDocumentWriter : JsonDocumentWriter, IDocumentWriter
    {
        public GraphQLDocumentWriter(IGraphQLJsonDocumentWriterOptions options) : base(options)
        {
        }

        public string Write(object value)
        {
            return base.SerializeObject(value);
        }
    }
}
