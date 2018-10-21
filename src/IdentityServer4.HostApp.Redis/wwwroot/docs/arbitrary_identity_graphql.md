# [Extension Grant](https://tools.ietf.org/html/rfc6749#section-4.5): arbitrary_identity
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
query q($input: arbitrary_identity!) {
  arbitrary_identity(input: $input) {
    access_token
    expires_in
    refresh_token
    token_type
    id_token
  }
}
```
### Variables
```
{
  "input": {
    "client_id": "arbitrary-resource-owner-client",
    "client_secret": "secret",
    "scope": "offline_access a b c d",
    "arbitrary_claims":"{ 'role': ['application', 'limited'],'query':['dashboard', 'licensing'],'seatId':['1234abcd']}",
    "subject":"886bea3f-e025-4ab9-a811-e9b86f563668",
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
    "arbitrary_identity": {
      "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNjA3MzUsImV4cCI6MTU0MDE2NDMzNSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImNhdCIsImRvZyJdLCJjbGllbnRfaWQiOiJhcmJpdHJhcnktcmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMTYwNzM1LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiIxMjM0YWJjZCIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJhIiwiYiIsImIiLCJjIiwiYyIsImQiLCJkIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl19.VUSpaI4KvEmzPqVw6IijPrTLiXbdgyt4VxrVbBvyL1ZlGPujdWOiluBzh6dtFdCT7mP8OavPupKwwdPvt5sjdOUNv-Fk6TaVjNR4Lyu5F0kUlMeyhNRUHCPosTuTK0gYzBM0ePQuG2rU6JHn3uqCWjaBiC347moT6vz8v52e7p8guEoHwgWRGA-6UBIKuOyhltkmigaIzcf6FRZOKRAt55RPoIweRnMQGIzS5z-KU-pkMqzAhp1yfFIx10SKB-g9ON3fGEaRxWZB_XFAtPi-VO2tOYndXagOZ57G1qOQS7lD2Ykpf9NBmsMzZ_RMTopSCd3WIC3hcNEiAk4RZZAfxA",
      "expires_in": 3600,
      "refresh_token": "CfDJ8KCYS6ITMUNMj7yhU7AYnmy5s3zeH2mHyKmV6xGTUcCM1AqTM4nX6-sCRu-tgHvJWdC6_rFKjIpgDs7wef0rvTn7QGav_DeB5yjlAnaNDx1pCb_phNknhLCxF1Mve7G5GCAB_R3uUYnFz5zwdEHVEI73p4bpqygYiE-Z0x7sLQ5IBejzUylqTAQ5al8zO_HSitIpT58CFldHtkGO3slbJAY",
      "token_type": "bearer",
      "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDAxNjA3MzksImV4cCI6MTU0MDE2MTAzOSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDAxNjA3MzksImF0X2hhc2giOiJjNGlSRGxuRE9jTDAtS0JOSnZPMzRnIiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQwMTYwNzM1LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiIxMjM0YWJjZCIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsImFtciI6WyJhcmJpdHJhcnlfaWRlbnRpdHkiLCJhZ2VudDp1c2VybmFtZTphZ2VudDBAc3VwcG9ydHRlY2guY29tIiwiYWdlbnQ6Y2hhbGxlbmdlOmZ1bGxTU04iLCJhZ2VudDpjaGFsbGVuZ2U6aG9tZVppcCJdfQ.vdXB8DO1OukvlolQu0h4JH3CcXWnmdpFdLMwCBbRDiQiwVfrdgK8nEPiAMeIBAVfgQUhuOnWJxlomWkaGsNuZBZiIaaAk4aKTi4t5mnox8cARg7_duBftUG7VLAxA4LDEl12e5WPdSA5zOJU4H8Dtpsyt1gk2tj3wawUKO3JUET6-LJm_WPgW03BS2Ci7pThzajyW4stN1g65Qss5OpwRlr__7FXd0gEb5gYAGZ-5r4-EbWUmsNEgKgansYVr8qnxMwSNNpnTDCGWq5O-xKx7jBOaf0QfXXbFFMlP7PSiUG9ey1iHq5iu5m4dvGVfPpEJsVsvlLjlLhCdtN6T30rNg"
    }
  }
}
```
