# IdentityServer4-Extension-Grants
Some useful generic extension grants

## References 
[IdentityServer4](http://docs.identityserver.io)  
[The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)  

## Extension Grant: arbitrary_resource_owner  
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt>grant_type</dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_resource_owner</b>".</dd>

  <dt>subject</dt>
  <dd><b>REQUIRED</b>.  The passed through subject</dd>
  
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
</dl>

## Example  
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
	subject:rat
 ```
 ```
 {
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImQxOTU1YjExZjAxZGQ5ZGI5ZmFhNTE3OGU0YWE2MjI2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ5NDE0MTMsImV4cCI6MTUyNDk0NTAxMywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyMTM1NCIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjIxMzU0L3Jlc291cmNlcyIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6InJhdCIsImF1dGhfdGltZSI6MTUyNDk0MTQxMywiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.SdKfgknC-0sE0wTSqnjA3AWcHaLVatkQXMdW8XpnpgnoXpTq4yAPLWBdJO4kiHqMgVxuhKDOjiz7jd0kZhP2T5cEdXC29n_so-17RdKTiImKAuX2A_-Ci3t8nv6vG4VM7RJBuRt3zQnSAAAuwYt4ih6VI7proSqSwrbrv57QhIkHPObg-5SS3B7iFgzckhoyxKfOs1pDCVlpiY-VVYxmhMs0VYLQaKqA8BuunvDhgWTLTB-wk8Lf7ICnwpJUn_1LJNss1PnaN6dypILWW2s4j9gDtv26HmI65B0jxpqgmtDHFixDvDsTBQkFSsF4TA-Ak8UOS_DtiFyXJzOyd31P2w",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "92cc858acee62dc2314c817061cb3bea018d7303c04517335737b89ebc8eb89f"
}
 ```
