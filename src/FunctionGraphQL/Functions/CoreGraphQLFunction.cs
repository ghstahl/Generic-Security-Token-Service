using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FunctionsCore.Modules;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using IdentityModel.Client;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using P7.GraphQLCore;
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
            string body;
            using (var streamReader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
            {
                body = await streamReader.ReadToEndAsync().ConfigureAwait(true);
            }
            var query = JsonConvert.DeserializeObject<GraphQLQuery>(body);

            var inputs = query.Variables.ToInputs();
            var queryToExecute = query.Query;

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.UserContext = new GraphQLUserContext(_httpContextAccessor);
                _.Schema = _schema;
                _.Query = queryToExecute;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
                _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                _.ValidationRules = _pluginValidationRules.Concat(DocumentValidator.CoreRules());

            }).ConfigureAwait(false);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            var json = _writer.Write(result);
            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

          //  var rr = new ObjectResult(obj) { StatusCode = (int)httpResult };
            _httpContextAccessor.HttpResponseMessage.Content = new JsonContent(obj);
        }
    }
}