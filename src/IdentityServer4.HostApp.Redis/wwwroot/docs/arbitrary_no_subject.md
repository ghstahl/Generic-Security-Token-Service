# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_no_subject  
This is really the only grant_type you will need...  

## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

paramater | description
--------- | -
grant_type | <b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_no_subject</b>".
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
	grant_type:arbitrary_no_subject
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:offline_access nitro metal
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
 ```
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjcyNjY0MzQsImV4cCI6MTUyOTg1ODQzNCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjhjNTllYzQxLTU0ZjMtNDYwYi1hMDRlLTUyMGZjNWI5OTczZCIsInBpaWQiOiIyMzY4ZDIxMy1kMDZjLTRjMmEtYTA5OS0xMWMzNGFkYzM1NzkiLCJzY29wZSI6WyJtZXRhbCIsIm5pdHJvIl19.0Sk89-Ahnm3EY51qo2cy5Qm8z1Wp7H2iqZ5KWee4bYgt7omlYy9Pa5crnhdDfSUdMEMEUz8k2T2AxYKefaIg4z9c_AtHt1gS85b5J9y_N58yQWWHuyNC4rpyzVj5mGj_Jzpjb-6tqCmT2qEi2TZXIcCXOvkYSqwjbACOmxy85YZVw0HYl6R0Ql4HrW-MVHmuz5vTfZvfQlqjwHEr4hFd9YYNJNe1qrAaSs85mazlGr5_sOxfyk_SO0pQm6ZQUvmga6K7-pQ_pBL6hU8z42wPOU4Nxp7HYhk-cl0GqHla1u5k9Xe35_eacHJoizXpf1rdeihlLmEDNibXvyQFWad0qg",
    "expires_in": 2592000,
    "token_type": "Bearer",
    "refresh_token": "85377bc405c45ccb51e2d035a3fb76203dfb93ad9eac6a2dac22452df1bc5db4"
}
```
[jwt.io](https://jwt.io/)  
```
{
  "nbf": 1527266434,
  "exp": 1529858434,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "metal",
    "nitro"
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
  "scope": [
    "metal",
    "nitro"
  ]
}
```
