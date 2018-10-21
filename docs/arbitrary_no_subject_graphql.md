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
    "access_token_lifetime":3600
  }
}
```

### Result
```
{
  "data": {
    "arbitrary_no_subject": {
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgyMjg1OTksImV4cCI6MTUzMDgyMDU5OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwicm9sZSI6WyJhcHBsaWNhdGlvbiIsImxpbWl0ZWQiXSwicXVlcnkiOlsiZGFzaGJvYXJkIiwibGljZW5zaW5nIl0sInNlYXRJZCI6IjEyMzRhYmNkIiwic2NvcGUiOlsiYWdncmVnYXRvcl9zZXJ2aWNlLnJlYWRfb25seSIsIm1ldGFsIiwibml0cm8iXX0.GESVzfw_7hLuKYrZNkGj71nRnAXfGnKHdR9K4FbUHpZ3Xymg-SO6Ow1p2Wi_NYfvOAdXY7fn0M7nNrSBLlT4dmblUHgeVX3GQok2N0v7JZRo_pmgz_HlcIQYrbKpKPqXZN1SHu3VDC9VrJuAecRIRHwToWXvDwDKFZwptOYyAYEL8FpVhnAgIT6ysgLqTnc-qso_Y87WuRwRnFBrvjD-LvL_WgPPY62XmL730eCf8_ArTSi_Cp4dVRmjEBPhtDboKO7uBX0iFnZ1QM54op7zApkMN3PVJPMj4_-B_C4ddT2RTxcGyz_FqWIeUu-wNNfy_MQMZybQUnXpx-gIXWPSoQ",
      "expires_in": 2592000,
      "token_type": "bearer"
    }
  }
}
```
