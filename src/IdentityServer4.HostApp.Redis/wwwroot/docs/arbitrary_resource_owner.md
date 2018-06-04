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
arbitrary_claims | <b>REQUIRED</b>.  This is a json string object of key/array pairs.  i.e. <em>arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}</em></dd>
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
    scope:offline_access metal nitro aggregator_service.read_only
    arbitrary_claims:{ "role": ["application", "limited"],"query":["dashboard", "licensing"],"seatId":["1234abcd"]}
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
    arbitrary_claims:{ "role": ["application", "limited"],"query":["dashboard", "licensing"],"seatId":["1234abcd"]}
    access_token_lifetime:3600
    access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwNTM5MjUsImV4cCI6MTUyODA1NzUyNSwiaXNzIjoiaHR0cHM6Ly9wN2lkZW50aXR5c2VydmVyNC5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6WyJodHRwczovL3A3aWRlbnRpdHlzZXJ2ZXI0LmF6dXJld2Vic2l0ZXMubmV0L3Jlc291cmNlcyIsImFnZ3JlZ2F0b3Jfc2VydmljZSIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUyODA1MzkyNSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.WuGY49tw8ZB1KUvYQyGAKPQHMpxqmi4Jmu0qQspLDV-QxBCitf2zGhMGt-RajUFajBYZy_2_mEA1ho3RsAQCJKNOWzN-1B0VhhGs1ajTGvNXhRIbbcCa0QwMBqeCWtOhAqj6-moIh0SK_6liCawwxL8-p_A5hHeEIly0qUZGoEdYXVjnSKMwUmBfnAZAWZqhqKMqTOAV1bGXLgD10OmcCkmzwvxc7A4HAvsmmt3_UnA-HNaViDwoDWYjgfzVVbDxPuhQPAmFrq5C_X3sy3FS58pknKhJBR-FO3_O3X3olbAwUugOaolAg2RINJxyEhT0AWkVgPMuarTmG8djbMYqpQ
```
Produces...  

```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwNTM5MjUsImV4cCI6MTUyODA1NzUyNSwiaXNzIjoiaHR0cHM6Ly9wN2lkZW50aXR5c2VydmVyNC5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6WyJodHRwczovL3A3aWRlbnRpdHlzZXJ2ZXI0LmF6dXJld2Vic2l0ZXMubmV0L3Jlc291cmNlcyIsImFnZ3JlZ2F0b3Jfc2VydmljZSIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUyODA1MzkyNSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.WuGY49tw8ZB1KUvYQyGAKPQHMpxqmi4Jmu0qQspLDV-QxBCitf2zGhMGt-RajUFajBYZy_2_mEA1ho3RsAQCJKNOWzN-1B0VhhGs1ajTGvNXhRIbbcCa0QwMBqeCWtOhAqj6-moIh0SK_6liCawwxL8-p_A5hHeEIly0qUZGoEdYXVjnSKMwUmBfnAZAWZqhqKMqTOAV1bGXLgD10OmcCkmzwvxc7A4HAvsmmt3_UnA-HNaViDwoDWYjgfzVVbDxPuhQPAmFrq5C_X3sy3FS58pknKhJBR-FO3_O3X3olbAwUugOaolAg2RINJxyEhT0AWkVgPMuarTmG8djbMYqpQ",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "0556d5f587e388ddab708d0fd440b3484069d8d78530e9d17bdaa7cd6055783b"
}
 ```
 [Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwNTM5MjUsImV4cCI6MTUyODA1NzUyNSwiaXNzIjoiaHR0cHM6Ly9wN2lkZW50aXR5c2VydmVyNC5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6WyJodHRwczovL3A3aWRlbnRpdHlzZXJ2ZXI0LmF6dXJld2Vic2l0ZXMubmV0L3Jlc291cmNlcyIsImFnZ3JlZ2F0b3Jfc2VydmljZSIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUyODA1MzkyNSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.WuGY49tw8ZB1KUvYQyGAKPQHMpxqmi4Jmu0qQspLDV-QxBCitf2zGhMGt-RajUFajBYZy_2_mEA1ho3RsAQCJKNOWzN-1B0VhhGs1ajTGvNXhRIbbcCa0QwMBqeCWtOhAqj6-moIh0SK_6liCawwxL8-p_A5hHeEIly0qUZGoEdYXVjnSKMwUmBfnAZAWZqhqKMqTOAV1bGXLgD10OmcCkmzwvxc7A4HAvsmmt3_UnA-HNaViDwoDWYjgfzVVbDxPuhQPAmFrq5C_X3sy3FS58pknKhJBR-FO3_O3X3olbAwUugOaolAg2RINJxyEhT0AWkVgPMuarTmG8djbMYqpQ)  
 ```
{
  "nbf": 1528053925,
  "exp": 1528057525,
  "iss": "https://p7identityserver4.azurewebsites.net",
  "aud": [
    "https://p7identityserver4.azurewebsites.net/resources",
    "aggregator_service",
    "metal",
    "nitro"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1528053925,
  "idp": "local",
  "role": [
    "application",
    "limited"
  ],
  "query": [
    "dashboard",
    "licensing"
  ],
  "seatId": "1234abcd",
  "scope": [
    "aggregator_service.read_only",
    "metal",
    "nitro",
    "offline_access"
  ],
  "amr": [
    "arbitrary_resource_owner"
  ]
}
 ```
