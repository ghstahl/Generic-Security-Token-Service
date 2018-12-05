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
    public class ArbitraryNoSubjectQuery : IQueryFieldRecordRegistration
    {
        private const string GrantType = "arbitrary_no_subject";
        /*
            grant_type:arbitrary_no_subject
            client_id:arbitrary-resource-owner-client
            client_secret:secret
            scope:nitro metal
            arbitrary_claims:{ "role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
            access_token_lifetime:3600
         */
        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;
        public ArbitraryNoSubjectQuery(ITokenEndpointHandlerExtra tokenEndpointHandlerExtra)
        {
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            queryCore.FieldAsync<ArbitraryNoSubjectResultType>(name: GrantType,
                description: $"mints a custom {GrantType} token.",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ArbitraryNoSubjectInput>> { Name = "input" }),
                resolve: async context =>
                {
                    try
                    {

                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var input = context.GetArgument<ArbitraryNoSubjectInputHandle>("input");

                        var formValues = new Dictionary<string, StringValues>()
                        {
                            {"grant_type", GrantType},
                            {"client_id", input.client_id},
                            {"client_secret", input.client_secret},
                            {"scope", input.scope},
                            {"arbitrary_claims", input.arbitrary_claims}
                        };
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

                        var processsedResult = await _tokenEndpointHandlerExtra.ProcessRawAsync(formCollection);

                        if (processsedResult.TokenErrorResult != null)
                        {
                            context.Errors.Add(new ExecutionError($"{processsedResult.TokenErrorResult.Response.Error}:{processsedResult.TokenErrorResult.Response.ErrorDescription}"));
                            return null;
                        }
                        var result = new ArbitraryNoSubjectResult
                        {
                            access_token = processsedResult.TokenResult.Response.AccessToken,
                            expires_in = processsedResult.TokenResult.Response.AccessTokenLifetime,
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