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
	access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDMwMjEsImV4cCI6MTU0MDE0NjYyMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0MzAyMSwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.vfxPtfec-VYxlJOb5Gc0t7NrEVC_6XFB5oBPosPRKS_QrTmsOXB7OB4-gUEAY-Hm1Z8H5kWa20MjZay5CdCuqRKdvNx6KVrEV1i2zp22_vHK9YYWJrKsQNrjj2DPsY6kSLTUgIysM8IeB27kgBTu6a6nRUor7kshxsr4kmluYXG9GyAIgjntOGWwdq5Hzi_KtrgOSJ1M6qsx6iGQpk_3e5jU3bOW9oUy_ENAW310NEWqfVNHupTfNvr_fcPpRCIyNvto5s_q71TgrRXwrGaWYnVgTR-QZYH7k1srRwjJ_dsls1wWG2abWDzW2TtjP95QOXHMPwgMHiwRlx2hzgj5FA
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
```
Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDMwOTYsImV4cCI6MTU0MDE0NjY5NiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0MzA5NiwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.jYzwMsxfMPG_7BJFE-jzOZpfUh-rdDK9hVgfoLkSJ9-QdKjG_IE6Q91iN0_t35xwBj9rVfF6855v7m7ckjmz890cOlzSBUiun1p9MC3qsGpiZjz6XY9_kwpvOPU-j59V1GFb8WjkDB1cLd1mtON4yLnrBhbTqLCUPAo8RR5PAQ3iVZv2l5xgtJ6OczDEgWZrBmJPAmkVVGcF4yWVxevAAaKTwPv01fuACp5HfvkeBkGe7LOJ8jPUyq1Ao-XaQIeu2npOVveU4OkpGmtG-uLhDjl_Y_uyWwERF_VjME8UsTORoRjOrbVIuKZMrrL-WK5sz6Jn6tfR2ar4nX9i-Dw6SA",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmyp2yrlV_FgO0KPb49Wrf0ki3yEhRIScTYy2QqfInYkV6w7faMDC3p74LYSxyVF0eGXNxhr8VHisupb-5f5XPNLkGUyNM6flKRMpEvKqmn_FUFtmzf8MmJpBowdbXeK9s7fEEnTK26eF7LpSPpf9fUkHxDbg8FEThiF_TSFWqBEtugntOKcedmlcj6OOxRTqi4"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDMwOTYsImV4cCI6MTU0MDE0NjY5NiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0MzA5NiwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.jYzwMsxfMPG_7BJFE-jzOZpfUh-rdDK9hVgfoLkSJ9-QdKjG_IE6Q91iN0_t35xwBj9rVfF6855v7m7ckjmz890cOlzSBUiun1p9MC3qsGpiZjz6XY9_kwpvOPU-j59V1GFb8WjkDB1cLd1mtON4yLnrBhbTqLCUPAo8RR5PAQ3iVZv2l5xgtJ6OczDEgWZrBmJPAmkVVGcF4yWVxevAAaKTwPv01fuACp5HfvkeBkGe7LOJ8jPUyq1Ao-XaQIeu2npOVveU4OkpGmtG-uLhDjl_Y_uyWwERF_VjME8UsTORoRjOrbVIuKZMrrL-WK5sz6Jn6tfR2ar4nX9i-Dw6SA)  
 ```
{
  "nbf": 1540143096,
  "exp": 1540146696,
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
  "auth_time": 1540143096,
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
