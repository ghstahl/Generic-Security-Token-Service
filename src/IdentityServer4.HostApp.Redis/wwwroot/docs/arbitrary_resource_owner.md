# Extension Grant: arbitrary_resource_owner  
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
arbitrary_claims | <b>REQUIRED</b>.  This is a json string object of key/value pairs.  i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>
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
    scope: offline_access nitro metal
    arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
    subject:Ratt
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
    arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
    access_token_lifetime:3600
    access_token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjY2OTcwMTMsImV4cCI6MTUyNjcwMDYxMywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNTYiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNTYvcmVzb3VyY2VzIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiUmF0dCIsImF1dGhfdGltZSI6MTUyNjY5NzAxMywiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.lLai2_h2bP7Pda_qplK9gXhaHCk2MThXu4ypMFDPm3RdlBMjErCCUNQFiueHsI314797EhX361pDjDw_smn5noTcVioRsbJYFZYMOjgWrKTmOjZiDwn7rlLPQOe5ubI_qv9rKkTjh076BrwMJF9u-c9CaByfnwrQmsHlFiG1q3HA01G38M77F0Z3g4Cyf2-whEVYQUspp5eND2hAkT10xcMFeNfJKp-gU4b6TF35hiXwYv_pxK3C4rH305iPVW8zu5oKZCkKu-Kt9dIOyRVt2mnAgSEmbVIbdff0QTu_3hgrI9IfrI24B3WSEP9rjM91YUJMAPbkMM6A1a0dKelOMw
```
Produces...  

```
 {
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ5NDcwMDksImV4cCI6MTUyNDk1MDYwOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6IlJhdHQiLCJhdXRoX3RpbWUiOjE1MjQ5NDcwMDksImlkcCI6ImxvY2FsIiwic29tZS1ndWlkIjoiMTIzNGFiY2QiLCJJbiI6IkZsYW1lcyIsInNjb3BlIjpbIm1ldGFsIiwibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X3Jlc291cmNlX293bmVyIl19.mSw45BkHO1IXMmtkN1fdgERb-d56NhnEjhIDrJRkpZjV5lUY5Pyi-dIqNXbd-TiCA-FZcPcPlnhGftgIMpB8vPiryEoDIxZ9YCRQIrhUncGiv8Q093hffZi-PvSVKSZyP_GkAuuRHWYIZAfzTickH8erNc01buuvpl4H-ItbCP_Klly_L3Vve5ZXPR1NbNp8DJMtbq-H2hgEN-zYqVqRGQnomgtPtCPxiiAIkUyLby-qEOxtL6x-5fMVXTT54mtGOa46-LVki8xf5hToHO1Hm5Pqm29ZVyMNJuiz6rH362vYlG3ybaqbCGJb5TbXMgRfjup3YjycSPJ6K-Mo46x1aw",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "71e7c354d005f50b1e2cd52089c0dff2efc45c6e25072948b04c4d947c7c5b5c"
}
 ```
 [jwt.io](https://jwt.io/)  
 ```
 {
  "nbf": 1524947009,
  "exp": 1524950609,
  "iss": "http://localhost:21354",
  "aud": [
    "http://localhost:21354/resources",
    "metal",
    "nitro"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "Ratt",
  "auth_time": 1524947009,
  "idp": "local",
  "some-guid": "1234abcd",
  "In": "Flames",
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
