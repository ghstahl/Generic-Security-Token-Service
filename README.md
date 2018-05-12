# IdentityServer4 Extension Grants
## This is really a generic JWT service  

I wanted to use [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) as a generic token managment service.
I wanted this service to be without knowledge of any user database, let alone claims for users.  
I wanted this service to manage the scopes that a given client is allowed.  
I wanted this service to take care of the job of what its good at, token managment when tokens are in flight.  

By the time I call this service to mint a token for me, I have already figured out the user and claims on my side.


## References 
[IdentityServer4](http://docs.identityserver.io)  
[The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)  

## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)

## Extension Grants  
#### Extension Grant: [arbitrary_no_subject](docs/arbitrary_no_subject.md)  
#### Extension Grant: [arbitrary_resource_owner](docs/arbitrary_resource_owner.md)  
#### Extension Grant: [arbitrary_oidc_resource_owner](docs/arbitrary_oidc_resource_owner.md)  


## Workarounds  
[refresh_token_revocation_workaround](docs/refresh_token_revocation_workaround.md)  
