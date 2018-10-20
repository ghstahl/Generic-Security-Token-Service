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
	grant_type:arbitrary_identity
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:offline_access metal nitro
	arbitrary_claims:{"preferred_username": ["ted@ted.com"],"name": ["ted@ted.com"]}subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
```
 
Produces...  

```
{
    "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAwNzc4OTksImV4cCI6MTU0MDA3ODE5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDAwNzc4OTgsImF0X2hhc2giOiJEcF9vR1ZVQ2U4Tm5ORWI3Mzh6ZllnIiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMDc3ODk3LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.HEvtwf5vTofvhPINBXAvQiL1OJewgopcebqN9w3xMaAlioyByCQsOBYrV28Ic9KLnMGwfRvSZphwQQ9InaBTgfABiOf62GcIBl3DlH_LgUvPPwz9wXdAQmr4bx0F2YcdH18k1YUekB60SHRrLm1nfH0wZtYZ5iEjRVxotpWfEFQqwJMemCbUj2ZanOMgEJ2ylw9m2giEMpdWpL8eMo__hzW_6Uo46ifv5Ohus8gVEQOODVCqqbfUhRfQb1VT3EQOarrDORQ_nFE5OWP_6Y-icvgCOpmvtCDgP2iXdJ_tUF4mVdYcrVHDcVW9Nubj-4beJDKZD_bRMCM9DeAYqfT7gg",
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAwNzc4OTgsImV4cCI6MTU0MDA4MTQ5OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMDc3ODk3LCJpZHAiOiJsb2NhbCIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X2lkZW50aXR5Il19.evb0kXerObb-vBQzcy5FQR8DlswlgeY2wNa2XPGcLW28tA2DiaRYXpxCSflqCk92VHEGNbhjEjgObl_2xrpqrx9o7lWHrntRxbXZk7VP74eKNqGyiWfwU7w-_5hLAlw0skc33ExfjB6NbV5A5YAPQx20rrHp0K4DwbFY0ER1E4lH7LXXifyihjX0oPkDvp09hsgRYo4KD0v8MjT_GfZoyMKwJGp1du5R5QuScx3cTaqZQibcY8rd5pgWLuxvRLxqHrauvpyBDawvkOOEJb8QSCr_qlUfBvnJewQiy0CW82ioT2hEFLeI6sFKRFHVcgxLcFg-ns4E6KP9U8u27M6kBw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmySkoSmcm0p-Ai0UvEbsnpVHsNCpnjQlTFJT1FqLuIynZPPF9xJCryrbRUjG5VGbKMxY2ed7qyLuIhnQt-yyiMplzY1Tvmo_B28_-6FJWl8IJ1kT2Lbkuk3xwm8iam4eNalFm9OeJ8lmEd2ImgDTzL6YG0LHKzL2QFylLf5nvNKivRse62aNI_DPsHyqLvxW9U"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAwNzc4OTksImV4cCI6MTU0MDA3ODE5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDAwNzc4OTgsImF0X2hhc2giOiJEcF9vR1ZVQ2U4Tm5ORWI3Mzh6ZllnIiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMDc3ODk3LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.HEvtwf5vTofvhPINBXAvQiL1OJewgopcebqN9w3xMaAlioyByCQsOBYrV28Ic9KLnMGwfRvSZphwQQ9InaBTgfABiOf62GcIBl3DlH_LgUvPPwz9wXdAQmr4bx0F2YcdH18k1YUekB60SHRrLm1nfH0wZtYZ5iEjRVxotpWfEFQqwJMemCbUj2ZanOMgEJ2ylw9m2giEMpdWpL8eMo__hzW_6Uo46ifv5Ohus8gVEQOODVCqqbfUhRfQb1VT3EQOarrDORQ_nFE5OWP_6Y-icvgCOpmvtCDgP2iXdJ_tUF4mVdYcrVHDcVW9Nubj-4beJDKZD_bRMCM9DeAYqfT7gg)
 ```
{
  "nbf": 1540077899,
  "exp": 1540078199,
  "iss": "https://localhost:44332",
  "aud": [
    "arbitrary-resource-owner-client",
    "cat",
    "dog"
  ],
  "iat": 1540077898,
  "at_hash": "Dp_oGVUCe8NnNEb738zfYg",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1540077897,
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
