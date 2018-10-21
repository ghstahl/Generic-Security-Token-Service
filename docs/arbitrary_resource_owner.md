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
  <dd><b>REQUIRED if access_token is missing</b>.  The passed through subject. Either subject or access_token must be passed.</dd>
  
  <dt><h2>access_token</h2></dt>
  <dd><b>REQUIRED if subject is missing</b>.  An access_token granted by this service.  access_token takes precedence over subject if both are passed.  The subject in the access_token is the only thing used, but that access_token has to be valid.</dd>

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
  <dd><b>REQUIRED</b>.  This is a json string object of key/array pairs.  
	<b>i.e. <em>arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}</em></b></dd>

  <dt><h2>arbitrary_amrs</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></b></dd>
	
  <dt><h2>arbitrary_audiences</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_audiences:["cat","dog"]</em></b></dd>
	
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
	scope:offline_access a b c d e
	arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
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
	scope:offline_access a b c d e
	arbitrary_claims:{"top":["TopDog"],"role": ["application","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
	access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDcwOTksImV4cCI6MTU0MDE1MDY5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NzA5OSwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.AfLWKsHTA_CzVl4Bd2lqR8bTBlNOE-DXvjP6bGopqhXPd1TjBQYFaWuSqUd2AlQJUHRKGNh7bLb9sh-LmTXF9B0jP5ufXgIERR2fsaPLOkD_gTfmqZtxczi8qcVOqDi2KZZYfN-J5h0StZs_ojWZMWZntkXhMpjmP0gD33zKLPhSHmxk8OBh_gQsmzd8rc_nNAQa7k4-4q9_12jZxMrz069fT5_gxbFe7ZQjZgWARx5MFyOQrKY0DyTSCQjmNyOeZ2nXi6Diohkm2pmDya-nwWHcLVyEtD-CRjZPe-HkoIKuTRb76XNrZHsAGvNIfj_yscoDtYF2aLHLGjcm2Fu1jg
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
```
Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDcwOTksImV4cCI6MTU0MDE1MDY5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NzA5OSwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.AfLWKsHTA_CzVl4Bd2lqR8bTBlNOE-DXvjP6bGopqhXPd1TjBQYFaWuSqUd2AlQJUHRKGNh7bLb9sh-LmTXF9B0jP5ufXgIERR2fsaPLOkD_gTfmqZtxczi8qcVOqDi2KZZYfN-J5h0StZs_ojWZMWZntkXhMpjmP0gD33zKLPhSHmxk8OBh_gQsmzd8rc_nNAQa7k4-4q9_12jZxMrz069fT5_gxbFe7ZQjZgWARx5MFyOQrKY0DyTSCQjmNyOeZ2nXi6Diohkm2pmDya-nwWHcLVyEtD-CRjZPe-HkoIKuTRb76XNrZHsAGvNIfj_yscoDtYF2aLHLGjcm2Fu1jg",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmydVnvM_cDNtSF-aVGwJFKS8LIQ5-tgn6Uvarhy96Z0v83CW2qeplVRv60n9lu8GxMIWboy3n9z9TPccT72Ant6PsCq7ldycflLsVtZKY9TUOYY7ZcvXp_RploAl-RD9_UwyceIP4QREbMRo3ucDLR_dcqhAxEA-mxrlLj5p-55OuZQ2pgQBX8jo4SSzG1sPxU"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDcwOTksImV4cCI6MTU0MDE1MDY5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NzA5OSwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.AfLWKsHTA_CzVl4Bd2lqR8bTBlNOE-DXvjP6bGopqhXPd1TjBQYFaWuSqUd2AlQJUHRKGNh7bLb9sh-LmTXF9B0jP5ufXgIERR2fsaPLOkD_gTfmqZtxczi8qcVOqDi2KZZYfN-J5h0StZs_ojWZMWZntkXhMpjmP0gD33zKLPhSHmxk8OBh_gQsmzd8rc_nNAQa7k4-4q9_12jZxMrz069fT5_gxbFe7ZQjZgWARx5MFyOQrKY0DyTSCQjmNyOeZ2nXi6Diohkm2pmDya-nwWHcLVyEtD-CRjZPe-HkoIKuTRb76XNrZHsAGvNIfj_yscoDtYF2aLHLGjcm2Fu1jg)  
 ```
{
  "nbf": 1540147099,
  "exp": 1540150699,
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
  "auth_time": 1540147099,
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
    "a",
    "b",
    "c",
    "d",
    "e",
    "offline_access"
  ],
  "amr": [
    "arbitrary_resource_owner",
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ]
}
 ```
