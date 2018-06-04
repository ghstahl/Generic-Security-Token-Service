using System;
using System.Collections.Generic;
using System.Text;
using GraphQL;
using GraphQLCore.ExtensionGrants.Models;
using P7.GraphQLCore;

namespace GraphQLCore.ExtensionGrants.GraphQL.Query
{
    public class IdentityQuery : IQueryFieldRecordRegistration
    {


        public IdentityQuery()
        {

        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            queryCore.FieldAsync<IdentityModelType>(name: "identity",
                description: null,
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var result = new IdentityModel {Claims = new List<ClaimHandle>()};
                        foreach (var claim in userContext.HttpContextAccessor.HttpContext.User.Claims)
                        {
                            result.Claims.Add(new ClaimHandle()
                            {
                                Name = claim.Type,
                                Value = claim.Value
                            });
                        }

                        return result;
                    }
                    catch (Exception e)
                    {

                    }

                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
        }
    }
}
