
# Extension Grant: arbitrary_no_subject  
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
arbitrary_claims | <b>REQUIRED</b>.  This is a json string object of key/value pairs.  i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>
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
    arbitrary_claims:{"sub":"Ratt","some-guid":"1234abcd","In":"Flames"}
    access_token_lifetime:3600
 ```
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjYwMTE2NzUsImV4cCI6MTUyODYwMzY3NSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6IlJhdHQiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyJdfQ.ImY42UjAuDR2wrkRQgUsOizh81Rf-ncPer3-nmQWJdeW3xXySs7ZRWstveIyvQewmdrQnoaqJeDBBob1NWQiLe4fa6gQ791IexiPzUkRL7zjpAEZqNSCoKmB4vG3hraAmX7gbe8nK5GydEqdwVU5Ql5hwkUEUKMDr1VlruwxyRFregscsx8rd_9Mq-EyF8z2QCZTT41Bkq-g-cavAOjpeDWBqYFEzdW4qQ37TlgIN15yW9gURkpjw8Yfj6hIo11g2oSqGqKVaKN42Jo5-3ddzTkl4Abqitbs-rpRSgOKdzIOPWvIA3r0JKreuItIq73_8oIHMk8u7VSsFZaPguVnkQ",
    "expires_in": 2592000,
    "token_type": "Bearer",
    "refresh_token": "d70394dea53d481146b4054bdfcbf8aa2f912c662f6e434fdce103238ddb71b5"
}
```
[jwt.io](https://jwt.io/)  
```
{
  "nbf": 1526011675,
  "exp": 1528603675,
  "iss": "http://localhost:21354",
  "aud": [
    "http://localhost:21354/resources",
    "metal",
    "nitro"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "sub": "Ratt",
  "some-guid": "1234abcd",
  "In": "Flames",
  "scope": [
    "metal",
    "nitro"
  ]
}
```
