# Extension Grant: arbitrary_identity
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_identity</b>".</dd>

  <dt>subject</dt>
  <dd><b>REQUIRED if access_token is missing</b>.  The passed through subject. Either subject or access_token must be passed.</dd>
  
  <dt>access_token</dt>
  <dd><b>REQUIRED if subject is missing</b>.  An access_token granted by this service.  access_token takes precedence over subject if both are passed.  The subject in the access_token is the only thing used, but that access_token has to be valid.</dd>

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
	
  <dt>access_token_lifetime</dt>
  <dd><b>OPTIONAL</b>.  The access token's lifetime in seconds.  Must be > 0 and less than configured AccessTokenLifetime.</dd>
</dl>  

## Example  
I use [Postman](https://www.getpostman.com/)  

```
POST http://localhost:21354/connect/token

Headers:
    Content-Type:application/x-www-form-urlencoded

Body:
	grant_type:arbitrary_resource_owner
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:offline_access metal nitro
	arbitrary_claims:{"preferred_username": ["ted@ted.com"],"name": ["ted@ted.com"],	"agent:username": ["agent0@supporttech.com"],	"agent:challengeQuestion": ["First Car", "First House", "First Pet"]}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
```
 
Produces...  

```
{
    "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1Mzk5OTU5NjIsImV4cCI6MTUzOTk5NjI2MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50IiwiaWF0IjoxNTM5OTk1OTYyLCJhdF9oYXNoIjoiSG9GRUE2NzBidVFkZnB0aXV6d2NqUSIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUzOTk5NTk2MiwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsImFnZW50OnVzZXJuYW1lIjoiYWdlbnQwQHN1cHBvcnR0ZWNoLmNvbSIsImFnZW50OmNoYWxsZW5nZVF1ZXN0aW9uIjpbIkZpcnN0IENhciIsIkZpcnN0IEhvdXNlIiwiRmlyc3QgUGV0Il0sIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsImFtciI6WyJhcmJpdHJhcnlfaWRlbnRpdHkiXX0.ZTirRoXdfmSjv8GKtsXQWoOcqOEA_c1pHK4-80gD5tf245RmmEp0NIH1jGbkL3t4T5oameOatFo7ONCjUHDWc4ELkYlhAmn960Sk4Kla6c8rX4pUziS9ET1Afxmzdfq2CGqtMno-gsr6DGjvX3y9Bwmvo5X_90kCjC0u_kDUNsYZKPms99aW4haIBmvSinLNbINMfTE-Y4yGHNoGM8qDeZhfqEygUW8byZagO0IiP57qcCng4oH-o3QtfRUdU5aZ-7AqkVXA17-yCvTDE477Q6dwv0RVAMht-wbtTxEjpTZ3o1pfDpmulSlyYm8ErkECYq6ni3astXyaz4PWK1RhAg",
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1Mzk5OTU5NjIsImV4cCI6MTUzOTk5OTU2MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTM5OTk1OTYyLCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwiYWdlbnQ6dXNlcm5hbWUiOiJhZ2VudDBAc3VwcG9ydHRlY2guY29tIiwiYWdlbnQ6Y2hhbGxlbmdlUXVlc3Rpb24iOlsiRmlyc3QgQ2FyIiwiRmlyc3QgSG91c2UiLCJGaXJzdCBQZXQiXSwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfaWRlbnRpdHkiXX0.5B5dsGJh9MGqvaHrQAbjCVK8lQxszLkKkba8OK3oYAmv_Pp5PtX6bDfa1rXINp52W8qsOLZsTUvOUgYoPWJUl0DS64_oetGWH5BTlugyOvH3VdGHFUGp9H21ezl_lefEu-8kLx41tNUwyigQmyU4UxXvc9PGKMU32jA1Uvwfg9aNVBVDE5NiqgCG0AQv1TJrZxYVw_vEUbl97xdLVrfbWn-eBS9uNjh5peIvBWiUfXcZm-TbFAlsaz9_5Omt6Sbg2QgKHKZ2faBRQflhHvBFTA5_HBolITGPSyxdQtNXtUbzv6Z1pyvW-aT_wq8tco_KwlQj0bjBNsxoImShzJaTdg",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmyt0UAvxCJgu9DtnaJtXB27bPiVCpIRwk1Vd85pPLHFZ_sFKadSPjS1XdlcArbGRnScrz7NjhWurOKO_NPOHurKAvxLYKJLNGQMfp0iWVVlovFeHbhDnjjWMAE37bbcOFET9trS7bTVSfOhVp1siB_XPQYKsUP5h4grLWRp38iSALnue2DE8596sG767nDgD_I"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1Mzk5OTU5NjIsImV4cCI6MTUzOTk5NjI2MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50IiwiaWF0IjoxNTM5OTk1OTYyLCJhdF9oYXNoIjoiSG9GRUE2NzBidVFkZnB0aXV6d2NqUSIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUzOTk5NTk2MiwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsImFnZW50OnVzZXJuYW1lIjoiYWdlbnQwQHN1cHBvcnR0ZWNoLmNvbSIsImFnZW50OmNoYWxsZW5nZVF1ZXN0aW9uIjpbIkZpcnN0IENhciIsIkZpcnN0IEhvdXNlIiwiRmlyc3QgUGV0Il0sIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsImFtciI6WyJhcmJpdHJhcnlfaWRlbnRpdHkiXX0.ZTirRoXdfmSjv8GKtsXQWoOcqOEA_c1pHK4-80gD5tf245RmmEp0NIH1jGbkL3t4T5oameOatFo7ONCjUHDWc4ELkYlhAmn960Sk4Kla6c8rX4pUziS9ET1Afxmzdfq2CGqtMno-gsr6DGjvX3y9Bwmvo5X_90kCjC0u_kDUNsYZKPms99aW4haIBmvSinLNbINMfTE-Y4yGHNoGM8qDeZhfqEygUW8byZagO0IiP57qcCng4oH-o3QtfRUdU5aZ-7AqkVXA17-yCvTDE477Q6dwv0RVAMht-wbtTxEjpTZ3o1pfDpmulSlyYm8ErkECYq6ni3astXyaz4PWK1RhAg)
 ```
{
  "nbf": 1539995962,
  "exp": 1539996262,
  "iss": "https://localhost:44332",
  "aud": "arbitrary-resource-owner-client",
  "iat": 1539995962,
  "at_hash": "HoFEA670buQdfptiuzwcjQ",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1539995962,
  "idp": "local",
  "preferred_username": "ted@ted.com",
  "name": "ted@ted.com",
  "agent:username": "agent0@supporttech.com",
  "agent:challengeQuestion": [
    "First Car",
    "First House",
    "First Pet"
  ],
  "nudibranch_watermark": "Daffy Duck",
  "amr": [
    "arbitrary_identity"
  ]
}
 ```
