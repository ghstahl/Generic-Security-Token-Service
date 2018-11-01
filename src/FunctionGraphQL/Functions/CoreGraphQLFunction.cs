using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FunctionsCore.Modules;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.GraphQLCore.Validators;

namespace FunctionGraphQL.Functions
{
    /// <summary>
    /// This represents the function entity for CoreGraphQLFunction.
    /// </summary>
    public class CoreGraphQLFunction : IGraphQLFunction
    {
       
        private IServiceProvider _serviceProvider;
        private IMyContextAccessor _myContextAccessor;
        private IFunctionHttpContextAccessor _httpContextAccessor;
        private ILogger _logger;
        private IDocumentExecuter _executer;
        private IDocumentWriter _writer;
        private ISchema _schema;
        private List<IPluginValidationRule> _pluginValidationRules;

        public CoreGraphQLFunction(
            IFunctionHttpContextAccessor httpContextAccessor,
            IMyContextAccessor myContextAccessor,
            IServiceProvider serviceProvider,
            IDocumentExecuter executer,
            IDocumentWriter writer,
            ISchema schema,
            IEnumerable<IPluginValidationRule> pluginValidationRules,
            ILogger<CoreGraphQLFunction> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _myContextAccessor = myContextAccessor;
            _serviceProvider = serviceProvider;

            _logger = logger;
            
            _executer = executer;
            _writer = writer;
            _schema = schema;

            _pluginValidationRules = pluginValidationRules.ToList();
          
        }

        public async Task InvokeAsync()
        {
           
        }
    }
}