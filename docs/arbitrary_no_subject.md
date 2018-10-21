
# Extension Grant: arbitrary_no_subject  
This is really the only grant_type you will need...  

## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_no_subject</b>".</dd>
  
  <dt>client_id</dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt>client_secret</dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt>scope</dt>
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	<b>i.e. <em>scope:a b c d e</em></b></dd>
	 
  <dt>arbitrary_claims</dt>
  <dd><b>REQUIRED</b>.  This is a json string object of key/value pairs.  
	<b>i.e. <em>arbitrary_claims:{"sub":"Ratt","some-guid":"1234abcd","In":"Flames"}</em></b></dd>

  <dt>arbitrary_amrs</dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></b></dd>
	
  <dt>arbitrary_audiences</dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_audiences:["cat","dog"]</em></b></dd>
	
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
	grant_type:arbitrary_no_subject
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:a b c d e
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
 ```
 Produces....  
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDg3NzgsImV4cCI6MTU0Mjc0MDc3OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsiYSIsImIiLCJjIiwiZCIsImUiXSwiYW1yIjpbImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.d8q_0dKw8UuweIvv45wH2yxvihgcgYQodgGpJo29S0VYMkVSID-cXIbdLZTcUN5fwvG4iIlfRgDzScqJw0qqE91U_Cpr5AN-821suJfSe5QpkwPdL62ZAfqvGEsG-X16Wmx9pBA3hcsxBos0wW4cr9Y2Qo71eE2RqQ2KhjtCEp5F9woEAQ7YZAs5W88W_L6eGq_IPw_vg1utEResRgLpryf8UU22GFHBU5c8K1ZFXSfiaKVtJs02-McrY8fmPC1TMIK8IDTzUPRrWSB6Y8x5NobpzjWUlTu4qMEMBVG3G66yfhowSI5Wp_Jk5rigDrfnlagYNee2RQn5Mau32UCF5Q",
    "expires_in": 2592000,
    "token_type": "Bearer"
}
```
[Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDg3NzgsImV4cCI6MTU0Mjc0MDc3OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsiYSIsImIiLCJjIiwiZCIsImUiXSwiYW1yIjpbImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.d8q_0dKw8UuweIvv45wH2yxvihgcgYQodgGpJo29S0VYMkVSID-cXIbdLZTcUN5fwvG4iIlfRgDzScqJw0qqE91U_Cpr5AN-821suJfSe5QpkwPdL62ZAfqvGEsG-X16Wmx9pBA3hcsxBos0wW4cr9Y2Qo71eE2RqQ2KhjtCEp5F9woEAQ7YZAs5W88W_L6eGq_IPw_vg1utEResRgLpryf8UU22GFHBU5c8K1ZFXSfiaKVtJs02-McrY8fmPC1TMIK8IDTzUPRrWSB6Y8x5NobpzjWUlTu4qMEMBVG3G66yfhowSI5Wp_Jk5rigDrfnlagYNee2RQn5Mau32UCF5Q)

```
{
  "nbf": 1540148778,
  "exp": 1542740778,
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
    "e"
  ],
  "amr": [
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ]
}
```
 
