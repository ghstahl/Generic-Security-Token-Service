# Extension Grant: arbitrary_identity [WORK IN PROGRESS]
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt><h2>grant_type</h2></dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_identity</b>".</dd>

  <dt><h2>subject</h2></dt>
  <dd><b>REQUIRED if access_token is missing</b>.  The passed through subject. Either subject or access_token must be passed.</dd>
  
  <dt><h2>access_token</h2></dt>
  <dd><b>REQUIRED if subject is missing</b>.  An access_token granted by this service.  access_token takes precedence over subject if both are passed.  The subject in the access_token is the only thing used, but that access_token has to be valid.</dd>

  <dt><h2>client_id</h2></dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt>client_secret</h2></dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt><h2>scope</h2></dt>
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	<b>i.e. <em>scope:offline_access a b c d e</em></b></dd>
	 
  <dt><h2>arbitrary_claims</h2></dt>
  <dd><b>REQUIRED</b>.  This is a json string object of key/value pairs.  
	i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>

  <dt><h2>arbitrary_amrs</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></dd>
	
  <dt><h2>arbitrary_audiences</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_audiences:["cat","dog"]</em></dd>

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
	grant_type:arbitrary_identity
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:offline_access a b c d e
	arbitrary_claims:{"preferred_username": ["ted@ted.com"],"name": ["ted@ted.com"]}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
```
 
Produces...  

```
{
    "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNTIyNzgsImV4cCI6MTU0MDE1MjU3OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDAxNTIyNzgsImF0X2hhc2giOiJfRms0Qzl6Zl9GRHMzWkFWeWszUFp3Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMTUyMjc2LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.edykL0h3iR5fAKR3m3apbAZoZfZP6UM8w4t-QEJUbOBpl-mLuVm7bbcFfykCPM55TDvvPdO1lyLrOHXXMxaXSZSwWVwGofPJi7kU34wC37fDsvbOcExegBMoNX7EDeiZimnqCLv17bv_TMJ-DE7afS3NKsETtZjsifBZSIKIDH5EuKMV3O41XIgVtgtYT2_Lez3akc6GVZkPpgfrTfDFXPYv_UAFqepRV_AWXlH2Kf6VNQCS1Qo6WMzz-uuGP1h25QWGIpmosqYgQYdASnu4aKtOPzqKx4neQ9mEGWWNZeNOLKJq3cg0gfRQyZB4Z8AQceINaLblcHUy2SNSic9hCQ",
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNTIyNzYsImV4cCI6MTU0MDE1NTg3NiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE1MjI3NiwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.Qyaxd4-LkB1J0RgHQNewt1DJGWk0WfdeS_EQjFeH6CxqoU94xKfsqmnPQMMaKnc8k46agj4vChYtyN-Q0QbGUoOupZ9IIrk0GRoReuYzJP8ogerqk2zvROxrzQA6sjppFUw3oLs1pB-wb8OCq4QpTM8Xgs5GnNiqxaBgYnSR1BPJwTvGZ1fwS8BWmmlIimp9dehG7sWunfoSbRrI6PtS-BJpz9ocitjEFaaLK71cNqPZoZSp06yCFbKKVTaa8QpS2rv0oDDgZ1MZocszJYn-9jEJ3NiXIICBbxkSCsMSRRMwP8Hpek2fnoEpgpmAkQC5B9voSwGeXONk3ypyKGiXoQ",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmyzFFVOFeKD5S3UO9RUrZOfAMH3kyrRVC2iNau4-MU1v43567VkPDApiLmfUXeTwgi8fVzz_kir0WnTHTjp-irVuEYYqzkpwxbSV8n8z7NsP0YrrFomOGD_JzkDaiKtn6WH6EOvA7GJj080UGx1T-MThQ7shcLLcVnjRuErYUymV0Rrau5_f2LsezQZ1Cs2wBc"
}
 ```
 [Decode id_token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNTIyNzgsImV4cCI6MTU0MDE1MjU3OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDAxNTIyNzgsImF0X2hhc2giOiJfRms0Qzl6Zl9GRHMzWkFWeWszUFp3Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMTUyMjc2LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.edykL0h3iR5fAKR3m3apbAZoZfZP6UM8w4t-QEJUbOBpl-mLuVm7bbcFfykCPM55TDvvPdO1lyLrOHXXMxaXSZSwWVwGofPJi7kU34wC37fDsvbOcExegBMoNX7EDeiZimnqCLv17bv_TMJ-DE7afS3NKsETtZjsifBZSIKIDH5EuKMV3O41XIgVtgtYT2_Lez3akc6GVZkPpgfrTfDFXPYv_UAFqepRV_AWXlH2Kf6VNQCS1Qo6WMzz-uuGP1h25QWGIpmosqYgQYdASnu4aKtOPzqKx4neQ9mEGWWNZeNOLKJq3cg0gfRQyZB4Z8AQceINaLblcHUy2SNSic9hCQ)
 ```
{
  "nbf": 1540152278,
  "exp": 1540152578,
  "iss": "https://localhost:44332",
  "aud": [
    "arbitrary-resource-owner-client",
    "cat",
    "dog"
  ],
  "iat": 1540152278,
  "at_hash": "_Fk4C9zf_FDs3ZAVyk3PZw",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1540152276,
  "idp": "local",
  "preferred_username": "ted@ted.com",
  "name": "ted@ted.com",
  "nudibranch_watermark": "Daffy Duck",
  "amr": [
    "arbitrary_identity",
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ]
}
 ```
 [Decode access_token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNTIyNzYsImV4cCI6MTU0MDE1NTg3NiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE1MjI3NiwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.Qyaxd4-LkB1J0RgHQNewt1DJGWk0WfdeS_EQjFeH6CxqoU94xKfsqmnPQMMaKnc8k46agj4vChYtyN-Q0QbGUoOupZ9IIrk0GRoReuYzJP8ogerqk2zvROxrzQA6sjppFUw3oLs1pB-wb8OCq4QpTM8Xgs5GnNiqxaBgYnSR1BPJwTvGZ1fwS8BWmmlIimp9dehG7sWunfoSbRrI6PtS-BJpz9ocitjEFaaLK71cNqPZoZSp06yCFbKKVTaa8QpS2rv0oDDgZ1MZocszJYn-9jEJ3NiXIICBbxkSCsMSRRMwP8Hpek2fnoEpgpmAkQC5B9voSwGeXONk3ypyKGiXoQ)
 ```
{
  "nbf": 1540152276,
  "exp": 1540155876,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "a",
    "b",
    "c",
    "d",
    "e",
    "cat",
    "dog"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1540152276,
  "idp": "local",
  "preferred_username": "ted@ted.com",
  "name": "ted@ted.com",
  "nudibranch_watermark": "Daffy Duck",
  "scope": [
    "a",
    "b",
    "c",
    "d",
    "e",
    "offline_access"
  ],
  "amr": [
    "arbitrary_identity",
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ]
}
 ```
