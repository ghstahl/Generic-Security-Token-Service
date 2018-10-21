using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using P7.GraphQLCore;

namespace GraphQLCore.ExtensionGrants.GraphQL.Query
{
    public class ArbitraryResourceOwnerQuery : IQueryFieldRecordRegistration
    {
        private const string GrantType = "arbitrary_resource_owner";
        private IEndpointHandlerExtra _endpointHandlerExtra;
        public ArbitraryResourceOwnerQuery(IEndpointHandlerExtra endpointHandlerExtra)
        {
            _endpointHandlerExtra = endpointHandlerExtra;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            queryCore.FieldAsync<ArbitraryResourceOwnerResultType>(name: GrantType,
                description: $"mints a custom {GrantType} token.",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ArbitraryResourceOwnerInput>> { Name = "input" }),
                resolve: async context =>
                {
                    try
                    {

                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var input = context.GetArgument<ArbitraryResourceOwnerInputHandle>("input");

                        var formValues = new Dictionary<string, StringValues>()
                        {
                            {"grant_type", GrantType},
                            {"client_id", input.client_id},
                            {"client_secret", input.client_secret},
                            {"scope", input.scope},
                            {"arbitrary_claims", input.arbitrary_claims}
                        };
                        if (!string.IsNullOrWhiteSpace(input.subject))
                        {
                            formValues.Add("subject", input.subject);
                        }
                        if (!string.IsNullOrWhiteSpace(input.access_token))
                        {
                            formValues.Add("access_token", input.access_token);
                        }
                        if (!string.IsNullOrWhiteSpace(input.arbitrary_amrs))
                        {
                            formValues.Add("arbitrary_amrs", input.arbitrary_amrs);
                        }
                        if (!string.IsNullOrWhiteSpace(input.arbitrary_audiences))
                        {
                            formValues.Add("arbitrary_audiences", input.arbitrary_audiences);
                        }
                        if (input.access_token_lifetime > 0)
                        {
                            formValues.Add("access_token_lifetime", input.access_token_lifetime.ToString());
                        }

                        IFormCollection formCollection = new FormCollection(formValues);

                        var processsedResult = await _endpointHandlerExtra.ProcessRawAsync(formCollection);

                        if (processsedResult.TokenErrorResult != null)
                        {
                            context.Errors.Add(new ExecutionError($"{processsedResult.TokenErrorResult.Response.Error}:{processsedResult.TokenErrorResult.Response.ErrorDescription}"));
                            return null;
                        }
                        var result = new ArbitraryResourceOwnerResult
                        {
                            access_token = processsedResult.TokenResult.Response.AccessToken,
                            expires_in = processsedResult.TokenResult.Response.AccessTokenLifetime,
                            refresh_token = processsedResult.TokenResult.Response.RefreshToken,
                            token_type = "bearer"
                        };
                        return result;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError("Unable to process request", e));
                    }
                    return null;
                },
                deprecationReason: null);
        }
    }
}