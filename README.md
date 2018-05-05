# IdentityServer4 Extension Grants

I wanted to use [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) as a generic token managment service.
I wanted this service to be without knowledge of any user database, let alone claims for users.
I wanted this service to manage the scopes that a give client is allowed.
I wanted this service to take care of the job of what its good at, token managment when tokens are in flight.

By the time I call this service to build a token for me, I have already figured out the user and claims on my side.


## References 
[IdentityServer4](http://docs.identityserver.io)  
[The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)  

## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)

## Extension Grants  
#### Extension Grant: [arbitrary_resource_owner](docs/arbitrary_resource_owner.md)  
#### Extension Grant: [arbitrary_oidc_resource_owner](docs/arbitrary_oidc_resource_owner.md)  

I wanted to also have a NoSubject extension grant, but I was not able to add arbitrary claims to it.  Who cares though, because you can use the arbitrary_resource_owner extension grant and put in a subject of your choice that you don't have to look at.  After all its just a context into that subjects data.  That context is what you make of it.

