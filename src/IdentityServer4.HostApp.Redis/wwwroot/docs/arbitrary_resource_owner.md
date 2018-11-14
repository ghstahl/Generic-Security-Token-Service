# Extension Grant: arbitrary_resource_owner  
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt><h2>grant_type</h2></dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_resource_owner</b>".</dd>

  <dt><h2>subject</h2></dt>
  <dd><b>REQUIRED</b>.  The passed through subject.</dd>
  
  <dt><h2>client_id</h2></dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt><h2>client_secret</h2></dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
 
  <dt><h2>scope</h2></dt>
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	<b>i.e. <em>scope:offline_access a b c d e</em></b></dd>
	
  <dt><h2>arbitrary_claims</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json string object of key/array pairs.  
	<b>i.e. <em>arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}</em></b></dd>

  <dt><h2>arbitrary_amrs</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></b></dd>
	
  <dt><h2>arbitrary_audiences</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_audiences:["cat","dog"]</em></b></dd>

 <dt><h2>custom_payload</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a valid json.  
	i.e. <em>custom_payload:{"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}</em></dd>
	
  <dt><h2>access_token_lifetime</h2></dt>
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
	scope:offline_access metal nitro In Flames
	arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
	custom_payload:{"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}
```

Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDc5NTEsImV4cCI6MTU0MjE1MTU1MSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiRmxhbWVzIiwiSW4iLCJtZXRhbCIsIm5pdHJvIiwiY2F0IiwiZG9nIl0sImNsaWVudF9pZCI6ImFyYml0cmFyeS1yZXNvdXJjZS1vd25lci1jbGllbnQiLCJzdWIiOiI4ODZiZWEzZi1lMDI1LTRhYjktYTgxMS1lOWI4NmY1NjM2NjgiLCJhdXRoX3RpbWUiOjE1NDIxNDc5NTEsImlkcCI6ImxvY2FsIiwidG9wIjoiVG9wRG9nIiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjhjNTllYzQxLTU0ZjMtNDYwYi1hMDRlLTUyMGZjNWI5OTczZCIsInBpaWQiOiIyMzY4ZDIxMy1kMDZjLTRjMmEtYTA5OS0xMWMzNGFkYzM1NzkiLCJudWRpYnJhbmNoX3dhdGVybWFyayI6IkRhZmZ5IER1Y2siLCJzY29wZSI6WyJGbGFtZXMiLCJJbiIsIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIiwiYWdlbnQ6dXNlcm5hbWU6YWdlbnQwQHN1cHBvcnR0ZWNoLmNvbSIsImFnZW50OmNoYWxsZW5nZTpmdWxsU1NOIiwiYWdlbnQ6Y2hhbGxlbmdlOmhvbWVaaXAiXSwiY3VzdG9tX3BheWxvYWQiOnsic29tZV9zdHJpbmciOiJkYXRhIiwic29tZV9udW1iZXIiOjEyMzQsInNvbWVfb2JqZWN0Ijp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0fSwic29tZV9hcnJheSI6W3siYSI6ImIifSx7ImIiOiJjIn1dfX0.WQHvclRl-eYmD689RZwXgHt3Esll5AW102JDQovrsdR1CJLh48POLsrsFxlKdlvB5P2gPJktvL8NAivjsCU6YJqytr9wzmkGTZ_I175ji4NsjOWx9NHA8eK4Mdl0QSHtQQGviM8HxSsbLLMYFfn1Kb-ePss7k0z5Uz9yO_ZKNK9MGWaTZ4HEyrDZHKXJz1aLQTxqHxJkaWZz7dwa89G7A02m9NvcnqRNglEUQTS469mY1RWhImhL7H8lOPZ2zZySnb7j0cqv9hJXS2affZREQB-GNs76xP3y1pFimuZpWLhxzLZyJsIn-25iZ1IATOUbz4w8GJLFJDle36w-K2-W5A",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KHr4bbINrxKkapSXCyORFWDJCDaUmZZ_Z1HC_Ad22ZH1DfaXx87F6FyDn6doMFyNMxi0ACIWVDLrO53AYdUbwhzm_q_XT56fLq1bG9UlrWIsL4LNW_gHcHDPHAoK_UoBH7z3BcuT2b9cbCwGJklX0DlzCvelglL9JlT6QMiJ5MQ-j8uTj2ZLnVkg_9-OrBzqCmV89FDvsHaHUUPWai3Frg"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDc5NTEsImV4cCI6MTU0MjE1MTU1MSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiRmxhbWVzIiwiSW4iLCJtZXRhbCIsIm5pdHJvIiwiY2F0IiwiZG9nIl0sImNsaWVudF9pZCI6ImFyYml0cmFyeS1yZXNvdXJjZS1vd25lci1jbGllbnQiLCJzdWIiOiI4ODZiZWEzZi1lMDI1LTRhYjktYTgxMS1lOWI4NmY1NjM2NjgiLCJhdXRoX3RpbWUiOjE1NDIxNDc5NTEsImlkcCI6ImxvY2FsIiwidG9wIjoiVG9wRG9nIiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjhjNTllYzQxLTU0ZjMtNDYwYi1hMDRlLTUyMGZjNWI5OTczZCIsInBpaWQiOiIyMzY4ZDIxMy1kMDZjLTRjMmEtYTA5OS0xMWMzNGFkYzM1NzkiLCJudWRpYnJhbmNoX3dhdGVybWFyayI6IkRhZmZ5IER1Y2siLCJzY29wZSI6WyJGbGFtZXMiLCJJbiIsIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIiwiYWdlbnQ6dXNlcm5hbWU6YWdlbnQwQHN1cHBvcnR0ZWNoLmNvbSIsImFnZW50OmNoYWxsZW5nZTpmdWxsU1NOIiwiYWdlbnQ6Y2hhbGxlbmdlOmhvbWVaaXAiXSwiY3VzdG9tX3BheWxvYWQiOnsic29tZV9zdHJpbmciOiJkYXRhIiwic29tZV9udW1iZXIiOjEyMzQsInNvbWVfb2JqZWN0Ijp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0fSwic29tZV9hcnJheSI6W3siYSI6ImIifSx7ImIiOiJjIn1dfX0.WQHvclRl-eYmD689RZwXgHt3Esll5AW102JDQovrsdR1CJLh48POLsrsFxlKdlvB5P2gPJktvL8NAivjsCU6YJqytr9wzmkGTZ_I175ji4NsjOWx9NHA8eK4Mdl0QSHtQQGviM8HxSsbLLMYFfn1Kb-ePss7k0z5Uz9yO_ZKNK9MGWaTZ4HEyrDZHKXJz1aLQTxqHxJkaWZz7dwa89G7A02m9NvcnqRNglEUQTS469mY1RWhImhL7H8lOPZ2zZySnb7j0cqv9hJXS2affZREQB-GNs76xP3y1pFimuZpWLhxzLZyJsIn-25iZ1IATOUbz4w8GJLFJDle36w-K2-W5A)  
 ```
{
  "nbf": 1542147951,
  "exp": 1542151551,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "Flames",
    "In",
    "metal",
    "nitro",
    "cat",
    "dog"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1542147951,
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
  "scope": [
    "Flames",
    "In",
    "metal",
    "nitro",
    "offline_access"
  ],
  "amr": [
    "arbitrary_resource_owner",
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ],
  "custom_payload": {
    "some_string": "data",
    "some_number": 1234,
    "some_object": {
      "some_string": "data",
      "some_number": 1234
    },
    "some_array": [
      {
        "a": "b"
      },
      {
        "b": "c"
      }
    ]
  }
}
 ```
