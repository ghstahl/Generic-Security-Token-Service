using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using P7.GraphQLCore;

namespace GraphQLCore.ExtensionGrants.GraphQL.Query
{
    public class ArbitraryIdentityQuery : IQueryFieldRecordRegistration
    {
        private const string GrantType = "arbitrary_identity";
        private IEndpointHandlerExtra _endpointHandlerExtra;
        public ArbitraryIdentityQuery(IEndpointHandlerExtra endpointHandlerExtra)
        {
            _endpointHandlerExtra = endpointHandlerExtra;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            queryCore.FieldAsync<ArbitraryIdentityResultType>(name: GrantType,
                description: $"mints a custom {GrantType} token.",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ArbitraryIdentityInput>> { Name = "input" }),
                resolve: async context =>
                {
                    try
                    {

                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var input = context.GetArgument<ArbitraryIdentityInputHandle>("input");

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
                        var result = new ArbitraryIdentityResult
                        {
                            id_token = processsedResult.TokenResult.Response.IdentityToken,
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