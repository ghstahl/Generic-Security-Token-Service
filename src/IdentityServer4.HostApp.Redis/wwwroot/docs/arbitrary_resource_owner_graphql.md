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
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNDc2MzEsImV4cCI6MTU0MDE1MTIzMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibml0cm8iLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MDE0NzYzMSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJudWRpYnJhbmNoX3dhdGVybWFyayI6IkRhZmZ5IER1Y2siLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiLCJhZ2VudDp1c2VybmFtZTphZ2VudDBAc3VwcG9ydHRlY2guY29tIiwiYWdlbnQ6Y2hhbGxlbmdlOmZ1bGxTU04iLCJhZ2VudDpjaGFsbGVuZ2U6aG9tZVppcCJdfQ.rB191GKHiSrjNGS_4ue3G4VBCpTtRvJOVda-AqucjdOuJrS0QSyxIL7whNZ4VNchGpVuesHO-KIQV_AYdNh7lhJNIrUuA8NoYj0i-zlDmGEa_0gTb8HneGgECLtssMzClC16tcUwFaGwqTUEJ1baz7KMyYx8rHdoApWvn-jtJw2gkkq4uzOrwzDncwoaOGsV-rh3d-yf8ebz6h-bv_Oailz2DiQH2Fcn9YkWeyjczShGPz3qeUjSqWr6OQkSLi1lLnjoAGYz8RZD154-PsPGkB8gZ5SevWbvse2zrchAYdmPWDicUbmEBtD6pUcDaKG6TCRI7BYghE-m9XfY1eKmBQ",
      "expires_in": 3600,
      "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmx7qh790APFbJRQiQ0kob9qZKNhjzia0rA8RKOJ0Ku4yF5S1PjHFDv9uw-ja93b8k6izekzAQryIsZhf5BIoFlxxHQ7MCTmSww64pNi-WClXGS2XguH8_CoaYuM9WLqSBbIYdXcBqQtQLVHVISHVyH4jybCFeQCoN39TsCE4YcBdiMDcLIHmX23PHYLwuWyGxg",
      "token_type": "bearer"
    }
  }
}
```
