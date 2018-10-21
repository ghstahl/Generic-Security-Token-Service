<!--
  Title: IdentityServer4 Extension Grants
  Description: An OAuth2 service that lets you create tokens for anything.
  Author: Herb Stahl
  -->
# Generic-Security-Token-Service (GSTS)
The GSTS was built using [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) as the foundational OAuth 2.0/OpenIdConnect framework.  Primarily the use of [extension grants](http://docs.identityserver.io/en/release/topics/extension_grants.html).

# What is a Security Token Service?
Well, it depends who you ask.  There is even a draft [OAuth Working Group](https://tools.ietf.org/html/draft-ietf-oauth-token-exchange-11) spec.  Getting into specifics on the OAuth Working Group draft, it looks too high level to be generic.  The fact that it requires you to pass in a subject token already goes down an opinionated path of usage.  I may not have or require a subject_token to need an STS to create me tokens.  

Furthermore it optionally asks that I pass in a resource that I want access to.  Here it leaves open the question: Resource to what and whose rules determine what tokens if any you will get?  In my opinion this is a play to have the STS know business rules that it has no business knowing.   

Business rules, in my experience, are distributed!  

Amazon's AWS has one as well.   The [AWS Security Token Service](https://docs.aws.amazon.com/STS/latest/APIReference/Welcome.html).  Right off the bat it flately states the following;  

> The AWS Security Token Service (STS) is a web service that enables you to request temporary, limited-privilege credentials for AWS Identity and Access Management (IAM) users or for users that you authenticate (federated users). 

Not very generic if it only gives out tokens for AWS services.  It does prove the point that STS's are subjective and custom.

If you do a quick google search, a bunch pop up and each are custom and opinionated to an end users goal.  

# What is GSTS?
GSTS is basically a minting service.  Much like the US Mint, which mints money without regard of what that money is spent on.
In short is a glorified JWT library, but this one requires something like a REDIS cache to manage refresh_tokens.  Like a good JWT library, it doesn't care why you are minting the token.  

The GSTS does provide the means to know exactly who minted it so downstream decisions can be made, by injecting a watermark claim (nudibranch_watermark).  Imagine 2 clients request the GSTS to mint a token with identical data.  Given the result, we can compare the 2 tokens and know that they are NOT the same.  There are claims that the GSTS puts in there to mark the token to a given client.  This is a configuration step where a give group may have many clients, which all share the same watermark.  The client_id is also in the access_token, however that isn't something you want to check for especially when a single group may have many.

The GSTS will mint tokens for registered clients and take on the burden of becomming the Authority for those tokens.  If the GSTS mints a refresh_token, it manages the lifetime of said refresh_token.  Hence the need for a REDIS cache.  

So instead of a library, I now have a service that requires a DevOps team to manage it.  

The resource to what and whose rules are NOT what GSTS cares about.  The assumption is that that knowledge is known before a call the GSTS is made to mint a token to those resources.  

**GSTS doesn't get involved in your business!** 

# Example Usage  
In this example the "SomeService" accepts an id_token as a binding input.  It can be anything as that decision is up to the "SomeService".  
![Binding User using id_token](/docs/binding-sequence.png)

In this example the "SomeService" accepts an id_token of a WebCamera as a binding input.  An id_token can be programatically generated.     
![Binding WebCamera using id_token](/docs/bind-webcamera-sequence.png)
docs/bind-webcamera-sequence.png


# The App  
![Image of Yaktocat](/docs/The_Fighting_Nudibranch_WebApp.png)

 ## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)  

The [redis-config](src/IdentityServer4.HostApp.Redis/appsettings.redis.json) is currently configured out, and using the InMemoryPersistantGrant Store.  Change "useRedis": true, and setup your redis cache connection via secrets.json.

The [keyvault-config](src/IdentityServer4.HostApp.Redis/appsettings.keyVault.json) is currently configured out.  It uses developer signing.

### secrets.json
```
{
  "appOptions": {
    "redis": {
      "redisConnectionString": "{{your app}}.redis.cache.windows.net:6380,password={{secret}},ssl=True,abortConnect=False"
    },
    "keyVault": {
      "ClientId": "{{ClientId}}",
      "ClientSecret": "{{ClientSecret}}"
    }
  },
  "clients:nag-client:secrets:0": "glitter",
  "clients:cct-client:secrets:0": "tinkerbell",


  "TokenEndpointHealthTask": {
    "clientId": "arbitrary-resource-owner-client",
    "clientSecret": "secret",
    "scheduleCron": "*/1 * * * *"
  }
}
```

## Extension Grants  
#### Extension Grant: [arbitrary_no_subject](docs/arbitrary_no_subject.md)  
#### Extension Grant: [arbitrary_resource_owner](docs/arbitrary_resource_owner.md) [GraphQL Variant](docs/arbitrary_resource_owner_graphql.md)    
#### Extension Grant: [arbitrary_identity](docs/arbitrary_identity.md)  


## Workarounds  
[refresh_token_revocation_workaround](docs/refresh_token_revocation_workaround.md)  

# [References](docs/references.md)  
