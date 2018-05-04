# IdentityServer4 Extension Grants
Some useful generic extension grants

## References 
[IdentityServer4](http://docs.identityserver.io)  
[The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)  

## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)

## Extension Grants  
#### Extension Grant: [arbitrary_resource_owner](docs/arbitrary_resource_owner.md)  
#### Extension Grant: [arbitrary_oidc_resource_owner](docs/arbitrary_oidc_resource_owner.md)  

I wanted to also have a NoSubject extension grant, but I was not able to add arbitrary claims to it.  Who cares though, because you can use the arbitrary_resource_owner extension grant and put in a subject of your choice that you don't have to look at.  After all its just a context into that subjects data.  That context is what you make of it.

