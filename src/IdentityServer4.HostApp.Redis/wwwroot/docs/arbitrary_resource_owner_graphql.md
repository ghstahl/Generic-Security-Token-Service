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
    "access_token_lifetime":3600
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
    "access_token":"eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgwNTM5MjUsImV4cCI6MTUyODA1NzUyNSwiaXNzIjoiaHR0cHM6Ly9wN2lkZW50aXR5c2VydmVyNC5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6WyJodHRwczovL3A3aWRlbnRpdHlzZXJ2ZXI0LmF6dXJld2Vic2l0ZXMubmV0L3Jlc291cmNlcyIsImFnZ3JlZ2F0b3Jfc2VydmljZSIsIm1ldGFsIiwibml0cm8iXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTUyODA1MzkyNSwiaWRwIjoibG9jYWwiLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiMTIzNGFiY2QiLCJzY29wZSI6WyJhZ2dyZWdhdG9yX3NlcnZpY2UucmVhZF9vbmx5IiwibWV0YWwiLCJuaXRybyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJhcmJpdHJhcnlfcmVzb3VyY2Vfb3duZXIiXX0.WuGY49tw8ZB1KUvYQyGAKPQHMpxqmi4Jmu0qQspLDV-QxBCitf2zGhMGt-RajUFajBYZy_2_mEA1ho3RsAQCJKNOWzN-1B0VhhGs1ajTGvNXhRIbbcCa0QwMBqeCWtOhAqj6-moIh0SK_6liCawwxL8-p_A5hHeEIly0qUZGoEdYXVjnSKMwUmBfnAZAWZqhqKMqTOAV1bGXLgD10OmcCkmzwvxc7A4HAvsmmt3_UnA-HNaViDwoDWYjgfzVVbDxPuhQPAmFrq5C_X3sy3FS58pknKhJBR-FO3_O3X3olbAwUugOaolAg2RINJxyEhT0AWkVgPMuarTmG8djbMYqpQ",
    "access_token_lifetime":3600
  }
}
```
### Result
```
{
  "data": {
    "arbitrary_resource_owner": {
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Imh0dHBzOi8vcDdrZXl2YWx1dC52YXVsdC5henVyZS5uZXQva2V5cy9QN0lkZW50aXR5U2VydmVyNFNlbGZTaWduZWQvOGJkZDYxODA3NWQwNGEwZDgzZTk4NmI4YWE5NGQ3YjIiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1MjgyMDk3NTgsImV4cCI6MTUyODIxMzM1OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYWdncmVnYXRvcl9zZXJ2aWNlIiwibWV0YWwiLCJuaXRybyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTI4MjA5NzU4LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiIxMjM0YWJjZCIsInNjb3BlIjpbImFnZ3JlZ2F0b3Jfc2VydmljZS5yZWFkX29ubHkiLCJtZXRhbCIsIm5pdHJvIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciJdfQ.MyzGPi4yMn8RXwsjxbFmf7pWDz4UObjG5mJj0rFMa2eiGPJuYHy70gzqzMcXGXBrBhj1WqvY0aRWtTJLvuW09l0TCFCRVTZ9lF0xoEYk4L8du2-XHPskWYy5oqrDYUoJsToWc2hJvvMkpBJZaTP_yZ7gnFOw7YRZaUL6KSveRSQSRPPV3B_UolCn_1xLX_qSB0DtINsk2MQNbq4XFa3Jk491oxxmpa7mP3nKpTDgs9-XwpEBYj8MAwFLH22Ypb_KddDqQbfL3mgEDAoERlpddIFyFBsyCEvopdub1cyhhv6en1_AaBKzfPgk26aWkJf2iVu0BoLj7-Tx-bbIDVvavA",
      "expires_in": 3600,
      "refresh_token": "aa7a935a84c52f7a1fb0b2209a01f8648f8628c5d6504519e71fb1633392c7f3",
      "token_type": "bearer"
    }
  }
}
```
