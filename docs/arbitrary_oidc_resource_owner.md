# Extension Grant: arbitrary_oidc_resource_owner  

This is not a generic grant as it attempts to exchange an id_token for tokens that give your subject access to your resources.  In this example I am simply passing along the original id_token's subject.  Odds are that this id_token's subject is not your actual user id, so a mapping is done in the ExtensionGrantValidator.  Here I replace the ClaimTypes.NamedIdentifier of the incoming id_token, to one that you would find in your user database.  The example prepends MyCompany.{original ClaimTypes.NamedIdentifier}

[Example ExtensionGrantValidator](../src/ArbitraryOpenIdConnectTokenExtensionGrants/ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator.cs)  
```
var idToken = raw[Constants.IdToken];
var principal = await providerValidator.ValidateToken(idToken);

var subjectId = principal.GetClaimValue(ClaimTypes.NameIdentifier);
var newPrincipal = principal.AddUpdateClaim(ClaimTypes.NameIdentifier, $"myCompany.{subjectId}");
principal = newPrincipal;
```

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

  <dt>access_token_lifetime</dt>
  <dd><b>OPTIONAL</b>.  The access token's lifetime in seconds.  Must be > 0 and less than configured AccessTokenLifetime.</dd>

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
    access_token_lifetime:3600
    authority:wellknown://google
    id_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3NDhlOWY3NjcxNTlmNjY3YTAyMjMzMThkZTBiMjMyOWU1NDQzNjIifQ.eyJhenAiOiIxMDk2MzAxNjE2NTQ2LWVkYmw2MTI4ODF0N3JrcGxqcDNxYTNqdW1pbnNrdWxvLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiMTA5NjMwMTYxNjU0Ni1lZGJsNjEyODgxdDdya3BsanAzcWEzanVtaW5za3Vsby5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjEwNDc1ODkyNDQyODAzNjY2Mzk1MSIsImF0X2hhc2giOiJCdGJaVlhRcTJ5MjFsQjR3VFo0MGxBIiwibm9uY2UiOiI2MzY2MTU2NzY5OTk0NDk4ODUuTWprNE5qTmhaVE10WW1ZMFlTMDBOMlJqTFRrNFpHUXRZamd3TkdNM05XTXhaV05pWXpkbVlXUmhaRFl0WmpVeE1pMDBOR1prTFRrMk9Ua3RPR0kyWmprNE56azRabVJsIiwiZXhwIjoxNTI1OTc0NTAwLCJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJpYXQiOjE1MjU5NzA5MDAsIm5hbWUiOiJIZXJiIFN0YWhsIiwicGljdHVyZSI6Imh0dHBzOi8vbGg0Lmdvb2dsZXVzZXJjb250ZW50LmNvbS8tdXZPc3RBRzhUcWsvQUFBQUFBQUFBQUkvQUFBQUFBQUFGVTAvaU9OSWpKbjNkZHMvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IkhlcmIiLCJmYW1pbHlfbmFtZSI6IlN0YWhsIiwibG9jYWxlIjoiZW4ifQ.guI4P1o_BZwH2L_YpuOkNW9sFG469nOJkXGnM3TjcPT88z2x_YQ4NHCnJ9za6H_JiTLnWDHXyz-CmQ9cxHIawsv103pi_oim_DLcD_XTGov-FVOoBqFj5fvTi3O2vFhz8nw4q2UlnoHZKsLnMPriBKZ9ovaIy53M3OYoyXbUqfejXNRremUautGYUyOiN7wbqTENX17ROvINU7IJxWzthXJ4k9eDwcctn55z6N6P6Tv3LFlzl_5HuMdOohBbPdbyHiY6-1xbxdwf0RpFGm_8-vxlHPCvNtUO3lBOOVruOsvRya84htqK1ak3Hhxy7_CNakNkyL52wWFrTgUX3fYtXw
 ```
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjU5NzA5NjcsImV4cCI6MTUyNTk3NDU2NywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Im15Q29tcGFueS4xMDQ3NTg5MjQ0MjgwMzY2NjM5NTEiLCJhdXRoX3RpbWUiOjE1MjU5NzA5NjcsImlkcCI6ImxvY2FsIiwic29tZS1ndWlkIjoiMTIzNGFiY2QiLCJJbiI6IkZsYW1lcyIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X29pZGNfcmVzb3VyY2Vfb3duZXIiLCJwd2QiXX0.YINXCz2ey4P7DMmz-Xj4J2eGlrE9MUUZwThLVy25u_TltoKOdIlHsVhwvmmWNGOirqeFf2ZPG5xAzUxfh9NsJx9GkjMQMWI6PKR6pJxGWNZH8S7uHT19GVw7BVHhKW1Kpvb5OxPg6Rn148JsTGa_l3h4-97P3T8xHdQABjZ5X3Iadif7Elu6YdFth4nc7NCIibMdapaZMfoYGxrLrnCHrVQ4UFCJQl94rg9_nv9Pevf5XX0a-N-TFSB4D8bHOQcvHdJ0FIZK7R61PuUQw-XZP_OgjkmCWQyoXFF9tecOKLaHNHbNQb-Hu-X-zhq4mIQfbWbH_VG_RsvsFwveTlPprA",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "a89d5053355efda2b7657e4b58a4f6d82892c1a1594b5ddd385571305b3761c1"
}
 ```
