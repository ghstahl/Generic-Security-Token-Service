# Extension Grant: arbitrary_resource_owner  
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_resource_owner</b>".</dd>

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
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	i.e. <em>scope:offline_access a b c d e</em></dd>
	
  <dt>arbitrary_claims</dt>
  <dd><b>REQUIRED</b>.  This is a json string object of key/value pairs.  
	i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>

  <dt>arbitrary_amrs</dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></dd>
	
  <dt>arbitrary_audiences</dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_audiences:["cat","dog"]</em></dd>
	
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
	arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
```
or ...  
```
POST http://localhost:21354/connect/token

Headers:
    Content-Type:application/x-www-form-urlencoded

Body:

grant_type:arbitrary_resource_owner
client_id:arbitrary-resource-owner-client
client_secret:secret
scope:offline_access metal nitro
arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
access_token_lifetime:3600
access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1Mzk0NDcyMjMsImV4cCI6MTUzOTQ1MDgyMywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTM5NDQ3MjIwLCJpZHAiOiJsb2NhbCIsInRvcCI6IlRvcERvZyIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwicHJvZmlsZV9zZXJ2aWNlX2tleSI6ImFyYml0cmFyeV9yZXNvdXJjZV9vd25lcl9wcm9maWxlX3NlcnZpY2UiLCJzY29wZSI6WyJtZXRhbCIsIm5pdHJvIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciJdfQ.D1WtL9lOjjqYmTGLf-YO83rKXVagTvcWPA3_GMP9Bg2op_WYZc9aq7FRtKQA_zJpGw0qoWnUe-ISPKGdHVSFSX5n8OQRH4rkzprrOvRhjcx6617eBOLHx09aUNKgI2GNmykeoQTwrSNOp06yLJUOp1qgHKNPh3HBfSDUzcdqBDgul42aizfDyLcAscqgSmIcvRg1gwfJau0ApVbmbgSRKgILNRvwb_93l2rjTnDSSN7WbtBBz_F0zbEy5mKgH1p7_pvAyZQVpTsnKaYfwBMck0tgOKwpzeiLs2oRGkt3ZzPsGlnt6YSAXtJQE_xP4U8DTD9ebnlzqeJzXWsgv43Alg
 
```
Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1Mzk0NDczNzIsImV4cCI6MTUzOTQ1MDk3MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTM5NDQ3MzcyLCJpZHAiOiJsb2NhbCIsInRvcCI6IlRvcERvZyIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwicHJvZmlsZV9zZXJ2aWNlX2tleSI6ImFyYml0cmFyeV9yZXNvdXJjZV9vd25lcl9wcm9maWxlX3NlcnZpY2UiLCJvcmlnaW5fYXV0aF90aW1lIjoiMTUzOTQ0NzIyMCIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.Ewn3OhlhnvHnViNHKxi-lYefUHZge2SHNZNzn_qaceQPk_ZRZjEe0iNeQwYlh3ynb1Oy3FpDTKwUWiONiQlFM8Fdh2va13yTN-Yk3Wa-FHwGbUinAL17LLFLLmrXSPe8yng7SEJaol9FLkHFXZdCcaeSf_W_wp-cp4IFVB-eUjPNSXrWawO2c1BID6XeapZ_S6i_i9pAve9ZQiUq5ToR7CsYwRUFHILpC5mJ_MtcKB9ATM7QXNzjb-Wn3ir6NZrdBqmskcuZmTUp_HnkeiT40BM3pGrrPawO7TbOeJ2YQKyRycbDQKPw0Yg3flZl-susqWOpDLz01IRjPs7NwnImmw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "f1dd44c7b882a12d29cab648fad49f06e876b3bc60269b7841f8f9fa04afe226"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1Mzk0NDczNzIsImV4cCI6MTUzOTQ1MDk3MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTM5NDQ3MzcyLCJpZHAiOiJsb2NhbCIsInRvcCI6IlRvcERvZyIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwicHJvZmlsZV9zZXJ2aWNlX2tleSI6ImFyYml0cmFyeV9yZXNvdXJjZV9vd25lcl9wcm9maWxlX3NlcnZpY2UiLCJvcmlnaW5fYXV0aF90aW1lIjoiMTUzOTQ0NzIyMCIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.Ewn3OhlhnvHnViNHKxi-lYefUHZge2SHNZNzn_qaceQPk_ZRZjEe0iNeQwYlh3ynb1Oy3FpDTKwUWiONiQlFM8Fdh2va13yTN-Yk3Wa-FHwGbUinAL17LLFLLmrXSPe8yng7SEJaol9FLkHFXZdCcaeSf_W_wp-cp4IFVB-eUjPNSXrWawO2c1BID6XeapZ_S6i_i9pAve9ZQiUq5ToR7CsYwRUFHILpC5mJ_MtcKB9ATM7QXNzjb-Wn3ir6NZrdBqmskcuZmTUp_HnkeiT40BM3pGrrPawO7TbOeJ2YQKyRycbDQKPw0Yg3flZl-susqWOpDLz01IRjPs7NwnImmw)  
 ```
{
  "nbf": 1539447372,
  "exp": 1539450972,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "metal",
    "nitro"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1539447372,
  "idp": "local",
  "top": "TopDog",
  "role": [
    "application",
    "limited"
  ],
  "query": [
    "dashboard",
    "licensing"
  ],
  "seatId": "8c59ec41-54f3-460b-a04e-520fc5b9973d",
  "piid": "2368d213-d06c-4c2a-a099-11c34adc3579",
  "nudibranch_watermark": "Daffy Duck",
  "profile_service_key": "arbitrary_resource_owner_profile_service",
  "origin_auth_time": "1539447220",
  "scope": [
    "metal",
    "nitro",
    "offline_access"
  ],
  "amr": [
    "arbitrary_resource_owner"
  ]
}
 ```
