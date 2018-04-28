# IdentityServer4 Extension Grants
Some useful generic extension grants

## References 
[IdentityServer4](http://docs.identityserver.io)  
[The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)  

## Configuration
[client-config](src/IdentityServer4.HostApp/Config.cs)

## Extension Grant: arbitrary_resource_owner  
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_resource_owner</b>".</dd>

  <dt>subject</dt>
  <dd><b>REQUIRED</b>.  The passed through subject</dd>
  
  <dt>client_id</dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt>client_secret</dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt>scope</dt>
  <dd><b>OPTIONAL</b>.  The scope of the access request as described by
         Section 3.3.</dd>
	 
  <dt>arbitrary_claims</dt>
  <dd><b>REQUIRED</b>.  This is a json string object of key/value pairs.  
	i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>
</dl>

## Example  
I am using [Postman](https://www.getpostman.com/)  

 ```
POST http://localhost:21354/connect/token

Headers:
    Content-Type:application/x-www-form-urlencoded

Body:
    grant_type:arbitrary_resource_owner
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope: offline_access nitro metal
	arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
	subject:Ratt
 ```
 ```
 {
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ5NDcwMDksImV4cCI6MTUyNDk1MDYwOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6IlJhdHQiLCJhdXRoX3RpbWUiOjE1MjQ5NDcwMDksImlkcCI6ImxvY2FsIiwic29tZS1ndWlkIjoiMTIzNGFiY2QiLCJJbiI6IkZsYW1lcyIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.mSw45BkHO1IXMmtkN1fdgERb-d56NhnEjhIDrJRkpZjV5lUY5Pyi-dIqNXbd-TiCA-FZcPcPlnhGftgIMpB8vPiryEoDIxZ9YCRQIrhUncGiv8Q093hffZi-PvSVKSZyP_GkAuuRHWYIZAfzTickH8erNc01buuvpl4H-ItbCP_Klly_L3Vve5ZXPR1NbNp8DJMtbq-H2hgEN-zYqVqRGQnomgtPtCPxiiAIkUyLby-qEOxtL6x-5fMVXTT54mtGOa46-LVki8xf5hToHO1Hm5Pqm29ZVyMNJuiz6rH362vYlG3ybaqbCGJb5TbXMgRfjup3YjycSPJ6K-Mo46x1aw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "71e7c354d005f50b1e2cd52089c0dff2efc45c6e25072948b04c4d947c7c5b5c"
}
 ```
