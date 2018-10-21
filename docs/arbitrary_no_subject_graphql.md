# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_no_subject 
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
query q($input: arbitrary_no_subject!) {
   arbitrary_no_subject(input: $input){
    access_token
    expires_in
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
    "scope": " metal nitro aggregator_service.read_only",
    "arbitrary_claims":"{ 'role': ['application', 'limited'],'query':['dashboard', 'licensing'],'seatId':['1234abcd']}",
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
    "arbitrary_no_subject": {
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNTAzMjQsImV4cCI6MTU0Mjc0MjMyNCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibml0cm8iLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiIxMjM0YWJjZCIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImFnZ3JlZ2F0b3Jfc2VydmljZS5yZWFkX29ubHkiLCJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJtZXRhbCIsIm5pdHJvIiwibml0cm8iXSwiYW1yIjpbImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.hJZvG11twmE3aGWkDI3dEBk5RMF9rJK8BqFqqutTGn7c5NaSSPbO_hvfewJVhdPiVxrg2EIw1PELxFsctExWK_KneaDKH30VhoaFZiUZcGpL-TIFHpEpu7Dii8xFUTcF70LIEa_ltzERYmGWSJQb1MAXNSOp3-wv13HLxLkAoEFbzlF2yX7SyiKXoQeBQ9LSN-U-DVq6_EdJetiD-9f01CZzCfTj3UJ-gyVLsSnYBtrNFi2r5AfQUuSUOLdPnIWjUQwkhYXdo-HubgF0d1f4asydLUwKh25sJsoS4CRTUazEIDSigC6MUVUlJDY_BpTPGOUy0ZYgZa_ADCgEok8rxQ",
      "expires_in": 2592000,
      "token_type": "bearer"
    }
  }
}
```
