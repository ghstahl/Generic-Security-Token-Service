# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_resource_owner  
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

paramater | description
--------- | -
grant_type | <b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_resource_owner</b>".
subject | <b>REQUIRED if access_token is missing</b>.  The passed through subject. Either subject or access_token must be passed.
access_token | <b>REQUIRED if subject is missing</b>.  An access_token granted by this service.  access_token takes precedence over subject if both are passed.  The subject in the access_token is the only thing used, but that access_token has to be valid.
client_id | <b>REQUIRED</b>.  The client identifier issued to the client during the registration process described by Section 2.2.
client_secret | <b>REQUIRED</b>.  The client secret.  The client MAY omit the parameter if the client secret is an empty string.
scope | <b>OPTIONAL</b>.  The scope of the access request as described by Section 3.3.
arbitrary_claims | <b>REQUIRED</b>.  This is a json string object of key/value pairs.  i.e. <em>arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}</em></dd>
access_token_lifetime | <b>OPTIONAL</b>.  The access token's lifetime in seconds.  Must be > 0 and less than configured AccessTokenLifetime.

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
	scope:offline_access nitro metal
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
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
	scope:offline_access nitro metal
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
	access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjcyNjY2MTAsImV4cCI6MTUyNzI3MDIxMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTI3MjY2NjA1LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5Iiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.RqReedvUUjt2u_myfmkLYWag2FUKvxMtYswlM5Lq1I8iQyr7kNl_PtbXDrw4EBHHQ4DQLPpAws2nexRTeXGnuXbVXSzSa_ZmqpoTCls94bzCt8HinIXQ3zULYI-74jXhtsQ5pa8IR9zrwKRXdDdTSdwDSiuctgKm8_9U9knPif6FwWqoXdPNtZDfxggUF8VZIUprbURum27KrvJkItWDwQedfzvBxANBlGNjRckLl7LiJm18aspifMug0IbYW1rgeFq75uyRCRPxdcKxQTd-Z0ctbEBUMmzUh9hZ18YGg98oPZY4tSM9da-I5ea5YaL9qHArw1oE2IQzyE9PGLT0fg
```
Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjcyNjY3MDAsImV4cCI6MTUyNzI3MDMwMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTI3MjY2NzAwLCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5Iiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.ZHfUPZQ8t_2cWCoHb02Imd1LLf1-gx65PFwqV1HI3Im24JX5GZ35wC6gm7ThGU0E-fyzZUV27hr64bTSKHsS9gOzo96zm1eFPZ5wTaiS5IIi1Ov5oj66ZYIgdMoIvHbuxH4zzEB69RGMWnUSaYoUIeKZFT3YszzoMOip8lJ1-4bjft-LnuMDv_1xG6YVsv8bd2h8tqZe2yL3zkU2Cgb3MMto2cgPe0X18QZ201sCO_nPJq7_drGE9I0PKxTlZwqkuo5ZMQ6UHJvxAvBWAEuHP_Oiz-jbixGhXHCUKIhMtm7b_Y251rrvxfaefhjqLbaxg2U4BeIQFWFrCyA83D1jHA",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "7065534a01a8e5646e56422698f88bc2c163d6842850171b313428e2ae214c36"
}
 ```
 [jwt.io](https://jwt.io/)  
 ```
{
  "nbf": 1527266700,
  "exp": 1527270300,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "metal",
    "nitro"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1527266700,
  "idp": "local",
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
    "metal",
    "nitro",
    "offline_access"
  ],
  "amr": [
    "arbitrary_resource_owner"
  ]
}
 ```
