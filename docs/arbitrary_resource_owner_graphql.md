# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_resource_owner 
## GraphQL Support  

This is a graphql variant of the original OAuth2 version.  

## GraphQL Client
[Altair GraphQL Client](https://chrome.google.com/webstore/detail/altair-graphql-client/flnheeellpciglgpaodhkhmapeljopja?hl=en)  

### GraphQL Endpoint
```
https://localhost:44332/api/v1/GraphQL
```

### query
```
query q($input: arbitrary_resource_owner!) {
  arbitrary_resource_owner(input: $input){
    access_token
    expires_in
    refresh_token
    token_type
  }
}
```
### Variables
```
{
  "input": {
    "client_id": "arbitrary-resource-owner-client",
    "client_secret": "secret",
    "scope": "offline_access metal nitro aggregator_service.read_only",
    "arbitrary_claims":"{ 'role': ['application', 'limited'],'query':['dashboard', 'licensing'],'seatId':['1234abcd']}",
    "subject":"886bea3f-e025-4ab9-a811-e9b86f563668",
    "access_token_lifetime":3600,
      "arbitrary_amrs":"['agent:username:agent0@supporttech.com','agent:challenge:fullSSN','agent:challenge:homeZip']",
"arbitrary_audiences":"['cat','dog']"
  }
}
```
or
```
{
  "input": {
    "client_id": "arbitrary-resource-owner-client",
    "client_secret": "secret",
    "scope": "offline_access metal nitro aggregator_service.read_only",
    "arbitrary_claims":"{ 'role': ['application', 'limited'],'query':['dashboard', 'licensing'],'seatId':['1234abcd']}",
    "access_token":"eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDQ4MTksImV4cCI6MTU0MDE0ODQxOSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibml0cm8iLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NDgxOSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibWV0YWwiLCJuaXRybyIsIm5pdHJvIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.GZRkBntVngZdgpKnl1Ej-FV_UlZn65mVPuLfzcDcHVjaA8CRlPbSov0REyHHHtqc-5p1E_D0gOlTURLrhUCUp307VRGYtwa5bfNVYnJI8KCPYpPG0U5r4tM3NMOqK2WwMCVTg7BgKEhX85RDZIsG2caXj71XEdi7rSOkvT-Z5MhixYLmt8YbtVPRmIPBl7xneFc7tmFu34AkwNCLZYzdDzGVxcJBowDjjBWsy99fjExkNp6wUTW69wkbNuX0htNCrUUwGu6MJW_mRkL_6usLsG8m2mQWiw5Udd-LH2brfzcRZH-YAdCDVPuYfj5gOl0fyAFxa7HdzE-uYFyGYIhZ_A",
    "access_token_lifetime":3600,
      "arbitrary_amrs":"['agent:username:agent0@supporttech.com','agent:challenge:fullSSN','agent:challenge:homeZip']",
"arbitrary_audiences":"['cat','dog']"
  }
}
```
### Result
```
{
  "data": {
    "arbitrary_resource_owner": {
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDUyMzAsImV4cCI6MTU0MDE0ODgzMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibml0cm8iLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NTIzMCwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibWV0YWwiLCJuaXRybyIsIm5pdHJvIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.10VdLMTyHqFymK6OffkOOjOtjHy52RvcW5Nre6ik6uo173YD696Gq0PtIXv3Ohgo9bM9tKfIqpKJY-dpwjSEdPCidXbSgrVVlMMZAa6M20n0BtUGhMw-QrqQC_Fp0JL_zp5zrIF9GRxo9-ZXFaP_KYAaj05wJ8mFuCCTYc9HRfD6xX-t00DZjiLiDUj7CKW2smhvGszTLSSY_xf2deXC2Ifq7JMpZqlFmggdztuh5jHM3YTtyOm9OGOyeDvWcWgxWtyVIUBEWrChhfdTGtKyZvUDQdwVKjh-nCL7YwgA5QJ13m1W25ExKDD09keMvYVJhze-L4hPjKojg0Y9ntPmFw",
      "expires_in": 3600,
      "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmxLIu2w6e25Xa9z3_KxZ8MSU14PuKmtcXidIIDAF1M4fG6oU2Vxh0JCpLRPh4L68aG6CUbTy95NsYLe2AvXMkANzco7dAHC3GrtGaEaKpD9UYaWQjpCdhP1Ab1--gBHOFuht6tHVJW3e8H0hl3PfQwaclTu6F0wgsSdO41FI7l_HvAv4YB2TNHlEUbuP3666y8",
      "token_type": "bearer"
    }
  }
}
```
