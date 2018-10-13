# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_no_subject  

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
scope | <b>OPTIONAL</b>.  The scope of the access request as described by Section 3.3.  <b>offline_access is NOT allowed</b>.
arbitrary_claims | <b>REQUIRED</b>.  This is a json string object of key/array pairs.  i.e. <em>arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}</em></dd>
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
	scope:nitro metal
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
 ```
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1Mzk0NDgwNzMsImV4cCI6MTU0MjA0MDA3MywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjhjNTllYzQxLTU0ZjMtNDYwYi1hMDRlLTUyMGZjNWI5OTczZCIsInBpaWQiOiIyMzY4ZDIxMy1kMDZjLTRjMmEtYTA5OS0xMWMzNGFkYzM1NzkiLCJudWRpYnJhbmNoX3dhdGVybWFyayI6IkRhZmZ5IER1Y2siLCJzY29wZSI6WyJtZXRhbCIsIm5pdHJvIl19.hT9zPGvvet150MCJ-XWhT8AgyFoFDfPCRjIPQOHj_2BQ4dPS9wg4fCmS-bxAwIN5mpVV8oWMVvUeAsJi6Okm9L-CNzctOWvDS0nRiGT_jnOCSjFuzpzNmgI1mxzzZ1DsZZgqiNA6bFyqCklI3AZsntiMVhwYAerb573rvrkunpSvDnrXeSAazIUCaAiTIddc6fNMpr7jxyd56plCA9LBzKY_6kv6Xn6NFqu4oxM2ixR_29izWbhYNFkhy0FEceM9W9QTPUkH1Eg3ogvdLE3o6-Y6h4-JruT_OK_IXWhQXhRmU08oYLPsfo_zLbT8820H2eZ1tcb48ROMKM0Z6hsgDw",
    "expires_in": 2592000,
    "token_type": "Bearer"
}
```
[Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1Mzk0NDgwNzMsImV4cCI6MTU0MjA0MDA3MywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjhjNTllYzQxLTU0ZjMtNDYwYi1hMDRlLTUyMGZjNWI5OTczZCIsInBpaWQiOiIyMzY4ZDIxMy1kMDZjLTRjMmEtYTA5OS0xMWMzNGFkYzM1NzkiLCJudWRpYnJhbmNoX3dhdGVybWFyayI6IkRhZmZ5IER1Y2siLCJzY29wZSI6WyJtZXRhbCIsIm5pdHJvIl19.hT9zPGvvet150MCJ-XWhT8AgyFoFDfPCRjIPQOHj_2BQ4dPS9wg4fCmS-bxAwIN5mpVV8oWMVvUeAsJi6Okm9L-CNzctOWvDS0nRiGT_jnOCSjFuzpzNmgI1mxzzZ1DsZZgqiNA6bFyqCklI3AZsntiMVhwYAerb573rvrkunpSvDnrXeSAazIUCaAiTIddc6fNMpr7jxyd56plCA9LBzKY_6kv6Xn6NFqu4oxM2ixR_29izWbhYNFkhy0FEceM9W9QTPUkH1Eg3ogvdLE3o6-Y6h4-JruT_OK_IXWhQXhRmU08oYLPsfo_zLbT8820H2eZ1tcb48ROMKM0Z6hsgDw)

```
{
  "nbf": 1539448073,
  "exp": 1542040073,
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
  "nudibranch_watermark": "Daffy Duck",
  "scope": [
    "metal",
    "nitro"
  ]
}
```
 
