<!--
  Title: IdentityServer4 Extension Grants
  Description: An OAuth2 service that lets you create tokens for anything.
  Author: Herb Stahl
  -->
# Generic-Security-Token-Service (GSTS)
The GSTS was built using [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) as the foundational OAuth 2.0/OpenIdConnect framework.  Primarily the use of [extension grants](http://docs.identityserver.io/en/release/topics/extension_grants.html).

# What is a Security Token Service?
Well, it depends who you ask.  There is even a draft [OAuth Working Group](https://tools.ietf.org/html/draft-ietf-oauth-token-exchange-11) spec.  Getting into specifics on the OAuth Working Group draft, it looks too high level to be generic.  The fact that it requires you to pass in a subject token already goes down an opinionated path of usage.  I may not have or require a subject_token to need an STS to create me tokens.  Furthermore it optionally asks that I pass in a resource that I want access to.  Here it leaves open the question: Resource to what and whose rules determine what tokens if any you will get?  It seems to me that I may need something like this but it is going to be very custom to my services and platform.  Certainly not generic.

Amazon's AWS has one as well.   The [AWS Security Token Service](https://docs.aws.amazon.com/STS/latest/APIReference/Welcome.html).  Right off the bat it flately states the following;  

> The AWS Security Token Service (STS) is a web service that enables you to request temporary, limited-privilege credentials for AWS Identity and Access Management (IAM) users or for users that you authenticate (federated users). 

Not very generic if it only gives out tokens for AWS services.  It does prove the point that STS's are subjective and custom.

If you do a quick google search, a bunch pop up and each are custom and opinionated to an end users goal.  

# What is GSTS?
GSTS is basically a minting service.  Much like the US Mint, which mints money without regard of what that money is spent on.
In short is a glorified JWT library, but this one requires something like a REDIS cache to manage refresh_tokens.  Like a good JWT library, it doesn't care why you are minting the token.  The GSTS does provide the means to know exactly who minted it so downstream decisions can be made.  Imagine 2 clients request the GSTS to mint a token with identical data.  Given the result, we can compare the 2 tokens and know that they are NOT the same.  There are claims that the GSTS puts in there to mark the token to a given client.

The GSTS will mint tokens for registered clients and take on the burden of becomming the Authority for those tokens.  If the GSTS mints a refresh_token, it manages the lifetime of said refresh_token.  Hence the need for a REDIS cache.  

So instead of a library, I know have a service that requires a DevOps team to manage it.  

The resource to what and whose rules are NOT what GSTS cares about.  The assumption is that that knowledge is known before a call the GSTS is made to mint a token to those resources.  

**GSTS doesn't get involved in your business!**  

 
![Image of Yaktocat](/docs/The_Fighting_Nudibranch_WebApp.png)

I wanted to use [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) as a generic token managment service.
I wanted this service to be without knowledge of any user database, let alone claims for users.  
I wanted this service to manage the scopes that a given client is allowed.  
I wanted this service to take care of the job of what its good at, token managment when tokens are in flight.  

By the time I call this service to mint a token for me, I have already figured out the user and claims on my side.


## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)

## Extension Grants  
#### Extension Grant: [arbitrary_no_subject](docs/arbitrary_no_subject.md)  
#### Extension Grant: [arbitrary_resource_owner](docs/arbitrary_resource_owner.md)  
#### Extension Grant: [arbitrary_oidc_resource_owner](docs/arbitrary_oidc_resource_owner.md)  


## Workarounds  
[refresh_token_revocation_workaround](docs/refresh_token_revocation_workaround.md)  

# [References](docs/references.md)  
