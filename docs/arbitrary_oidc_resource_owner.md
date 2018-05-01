# Extension Grant: arbitrary_oidc_resource_owner  

This is not a generic grant as it attempts to exchange an id_token for tokens that give your subject access to your resources.  In this example I am simply passing along the original id_token's subject.  Odds are that this id_token's subject is not your actual user id, so a mapping is done in the ExtensionGrantValidator.  Here I replace the ClaimTypes.NamedIdentifier of the incoming id_token, to one that you would find in your user database.  The example prepends MyCompany.{original ClaimTypes.NamedIdentifier}

[Example ExtensionGrantValidator](../src/ArbitraryOpenIdConnectTokenExtensionGrants/ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator.cs)

## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_oidc_resource_owner</b>".</dd>
  
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

  <dt>authority</dt>
  <dd><b>REQUIRED</b>.  The wellknown authority that issued the id_token.  i.e. wellknown://google</dd>

  <dt>id_token</dt>
  <dd><b>REQUIRED</b>.  The id_token issued by a wellknown authority.</dd>
</dl>

## Example  
I use [Postman](https://www.getpostman.com/)  

 ```
POST http://localhost:21354/connect/token

Headers:
    Content-Type:application/x-www-form-urlencoded

Body:
    grant_type:arbitrary_oidc_resource_owner
    client_id:arbitrary-resource-owner-client
    client_secret:secret
    scope:offline_access nitro metal
    arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
    authority:wellknown://google
    id_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImFmZmM2MjkwN2E0NDYxODJhZGMxZmE0ZTgxZmRiYTYzMTBkY2U2M2YifQ.eyJhenAiOiIxMDk2MzAxNjE2NTQ2LWVkYmw2MTI4ODF0N3JrcGxqcDNxYTNqdW1pbnNrdWxvLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiMTA5NjMwMTYxNjU0Ni1lZGJsNjEyODgxdDdya3BsanAzcWEzanVtaW5za3Vsby5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjEwNDc1ODkyNDQyODAzNjY2Mzk1MSIsImF0X2hhc2giOiJoYmFsY3lEUzhlenJERGZpaS1XczFBIiwibm9uY2UiOiI2MzY2MDc3NTA4MjE5NjMxNzEuWW1Zd1pUVTFNakF0T0Roa09TMDBOREpoTFdGak16SXRaalF3T0Rka09EYzROR1poTW1JMU16Z3pZall0WTJRMU5pMDBZekJtTFRobU5HRXROV1JtWkdNek1qTXpabVJoIiwiZXhwIjoxNTI1MTgxODgyLCJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJpYXQiOjE1MjUxNzgyODIsIm5hbWUiOiJIZXJiIFN0YWhsIiwicGljdHVyZSI6Imh0dHBzOi8vbGg0Lmdvb2dsZXVzZXJjb250ZW50LmNvbS8tdXZPc3RBRzhUcWsvQUFBQUFBQUFBQUkvQUFBQUFBQUFGVTAvaU9OSWpKbjNkZHMvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IkhlcmIiLCJmYW1pbHlfbmFtZSI6IlN0YWhsIiwibG9jYWxlIjoiZW4ifQ.CWPgqht9iUUdsVtfWMt1_0tJODZGrL32QXkzkG-1xq54OnWTWsC24D5ll_IQ4Hhh00AFT-veFVZeL8GZvIS0EIPB1sijbMm3mJTDvPIfiJwyubu12DhGPI70gcQ-LMH0xQJAvCteX56XoUAAfw4CglUOT627qv_lmEa1ycU0DzxOPBA4Kg25xTv1GjLFUGLQL_i-i7oAilcvQCB2Q91modYdDc9pOtUK94LhogeASxkwm5rtJMX8KukcgkFY1rCVHlC7HsT6qdHX_jjcEnHhnfHX7pVocA2at7r_eTlJsl0t35JYtZZU2g5nJePNicQrQeTZ81uwOuoRDn0xRmk1EA
 ```
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjUxNzg1MzIsImV4cCI6MTUyNTE4MjEzMiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6IjEwNDc1ODkyNDQyODAzNjY2Mzk1MSIsImF1dGhfdGltZSI6MTUyNTE3ODUyOCwiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfb2lkY19yZXNvdXJjZV9vd25lciIsInB3ZCJdfQ.MiyGEb_VYFB3lV7A5d-aTczIcJcthdKci9xCtPCDaLyQQN54HzemnRHhq8tWOZLqqMBkUC9-wUzgQRzltr3NYFGoWr67eQjTooISot6oXTUQfiFvbGMstsdV66i9huzx9HcBJ9bjI4eSUpm-C9bQMR4VdNdLO6IwjliL50mFb9iSyRYO9kgOvkH954vVLL4rezTRxcQjWpgUx9rkUwQZQpPbtWAPcjG83ORlb1vQtYS8x9sipi4JnroUDPTRDyRdz7uWJX4gbpqqzBZxD-tgng9E8bDktqDAl2XScJIfNLUrRUQMMjhy7WO-oVvTjAeSHA7hwBcYo_Ow5MPPfJFkMw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "0a86b91f2f57258c2acc74acb40174e7cf0acb9d75fc864a6291fcf37a1995eb"
}
 ```
