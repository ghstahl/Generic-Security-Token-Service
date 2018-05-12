# Multiple refresh_tokens with same subject and client_id not getting revoked problem
[IdentityServer4 Issue](https://github.com/IdentityServer/IdentityServer4/issues/2308)  

## Project
[The Workaround](../src/MultiRefreshTokenSameSubjectSameClientIdWorkAround)  

## Usage

```
public void ConfigureServices(IServiceCollection services)
{
   ...
   var builder = services.AddIdentityServer();
   ...
   
   // my replacement services added AFTER services.AddIdentityServer();
   builder.AddRefreshTokenRevokationGeneratorWorkAround();
   services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();         

   ...
}
```
