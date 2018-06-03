# API Configuration 

I am using [IdentityServer4](http://docs.identityserver.io/en/release/) as the core engine for this project.  I would have loved to let you pass in this configuration in the request, but I haven't found an elegant way to let that happen.  I beleve the following is an OK compromise where you need to register an api resource configuration on the NudiBranch service along with the clients.

Typically one mints tokens that give access to resources.  The below example shows 2 ways to configure api resources.  The difference comes when you have scopes.  In the case of scopes, you tend to use scopes to indicate granular access.

An apiResource is also a scope, so you can always request it directly.  

When an apiResource configuration has scopes, as in the case of aggregator_service, along with aggregator_service being a scope additional scopes take on the name of the apiResource as a namespace.  
{#fenced-id .fenced-class}
~~~
i.e. {apiResouceName}.{scope}, aggregator_service.full_access.

aggregator_service
aggregator_service.full_access
aggregator_service.read_only
~~~ 
When the scope is created the following data exists in the token;

1. The audience always contains the name of the apiResource.  i.e. metal or aggregator_service
2. The scope claim(s) contain the requested scope.  i.e. metal or aggregator_service.full_access

```
{
  "apiResources": [
    {
      "name": "metal",
      "scopes": [
      ]
    },
    {
      "name": "aggregator_service",
      "scopes": [
        {
          "name": "full_access",
          "displayName": "Full access to aggregator_service"
        },
        {
          "name": "read_only",
          "displayName": "Read only access to aggregator_service"
        }
      ]
    }
  ]
}
```

The above configuration results in the following scopes being available to assign as allowed_scopes for clients;
```
metal
aggregator_service
aggregator_service.full_access
aggregator_service.read_only
```

## Example Tokens  
[Aggregator Service Token](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwMzc2NTQsImV4cCI6MTUzMDYyOTY1NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlIl0sImNsaWVudF9pZCI6ImFyYml0cmFyeS1yZXNvdXJjZS1vd25lci1jbGllbnQiLCJzdWIiOiJSYXR0IiwiYXV0aF90aW1lIjoxNTI4MDM3NjU0LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiIxMjM0YWJjZCIsInNjb3BlIjpbImFnZ3JlZ2F0b3Jfc2VydmljZS5yZWFkX29ubHkiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.G4eiLBu6-2xvLnmgA2ZZa1c7rloQk87Q42Eygu6qXR0NKFK2i-SNascILdZpNSlZXKXHFcCfWabkqZg9__dqzG_s42zNH5U41dm47N4oyTGRUMspZzxfM4MyAalm23caScdzkA3GY3K1AtYU7UQqQKbTb68Ei1FPh110ICO3w_j5YhWWcACaHOUfyegBPp24khsJbu17pCnkv-F1gmk8SffesDtMjD5BI1rLN5IenWe4aYCYWw9zqDmQZ9ye98a8UJlABnob9tcJ-SxlhLFajp6vFMUdgKf7SxpceLm0CIkeO11t-LGZHUepXWYbe3z9N4hCWZlU-BtAUyUQw5CAJA)

[Metal Token](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwMzg3MDksImV4cCI6MTUzMDYzMDcwOSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6IlJhdHQiLCJhdXRoX3RpbWUiOjE1MjgwMzg3MDksImlkcCI6ImxvY2FsIiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjEyMzRhYmNkIiwic2NvcGUiOlsibWV0YWwiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.ZgKUd2KNqEpdI3WrdkDC7rI7l05NTzW9aES5fZKXUfVyTmMjB-OXWZpucuWferFeasKyjKbj-2pDA41SrLilcLOAdLzfNX1pCp0AI1Zq1Qojn8E5KFmzoDhdWgoQNIWvnCnmYin1P0ryWVEmy3AiPBwYq2SvlUmGUh7Qsq0hPbrxL-8yQMlRx0yiRVd6i4BhgLVtX68kjJAYrkbv912YdlJMPxqe9hvM3F5Ov2OWKrf_nfC4yT5jgIro4cpOk4gp2FtBvrR2UqPBPAcrugEbHCCD--pIxukQHuLuE3mGRdQJIMhrUOLaUVyk1YE_YU_Ux1O5qlDucJ_Ujl9qWnll8Q)

