
# Extension Grant: arbitrary_no_subject  
This is really the only grant_type you will need...  

## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt><h2>grant_type</h2></dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_no_subject</b>".</dd>
  
  <dt><h2>client_id</h2></dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt><h2>client_secret</h2></dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt><h2>scope</h2></dt>
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	<b>i.e. <em>scope:a b c d e</em></b></dd>
	 
  <dt><h2>arbitrary_claims</h2></dt>
  <dd><b>REQUIRED</b>.  This is a json string object of key/value pairs.  
	<b>i.e. <em>arbitrary_claims:{"sub":"Ratt","some-guid":"1234abcd","In":"Flames"}</em></b></dd>

  <dt><h2>arbitrary_amrs</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></b></dd>
	
  <dt><h2>arbitrary_audiences</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	<b>i.e. <em>arbitrary_audiences:["cat","dog"]</em></b></dd>

 <dt><h2>custom_payload</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a valid json.  
	i.e. <em>custom_payload:{"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}</em></dd>
	
  <dt><h2>access_token_lifetime</h2></dt>
  <dd><b>OPTIONAL</b>.  The access token's lifetime in seconds.  Must be > 0 and less than configured AccessTokenLifetime.</dd>
</dl>  

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
	scope:a b c d e
	arbitrary_claims:{"role": ["application", "limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
	custom_payload:{"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}
 ```
 Produces....  
 ```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDc5OTQsImV4cCI6MTU0NDczOTk5NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsiYSIsImIiLCJjIiwiZCIsImUiXSwiYW1yIjpbImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.2r-cxJtSyDIcQGPInmnXUd51RPbPi-AS8TQ0n0tSI0pixOuEG08fzzJGKKNX5hYFw47sdULRSL4DAsGporoo3_cUJH9Kao61qU-NaZw7qgS9CJwcm1Xw8zCwCTEy-cySNq0gt6V_aValvTGpfgHnmdzWM47GyK375O2Is1bwu3gIDdl4yf9fwNxNb4hBuCK2S84SmvQDrPFJuGz4b1cE9K0hogCLnkAHfe3-7DrekXuXLiVA1Y_vTbMxQSu0C8THG5s1P6GH5rBV5oG5LOaVftWSIV8UX5vDSlzyAPnrncdMu5sCvXcZi0sxtk2ouAgPQs-bS5Y3onRUdQLn_Se-pQ",
    "expires_in": 2592000,
    "token_type": "Bearer"
}
```
[Decode Token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDc5OTQsImV4cCI6MTU0NDczOTk5NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInJvbGUiOlsiYXBwbGljYXRpb24iLCJsaW1pdGVkIl0sInF1ZXJ5IjpbImRhc2hib2FyZCIsImxpY2Vuc2luZyJdLCJzZWF0SWQiOiI4YzU5ZWM0MS01NGYzLTQ2MGItYTA0ZS01MjBmYzViOTk3M2QiLCJwaWlkIjoiMjM2OGQyMTMtZDA2Yy00YzJhLWEwOTktMTFjMzRhZGMzNTc5IiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsiYSIsImIiLCJjIiwiZCIsImUiXSwiYW1yIjpbImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.2r-cxJtSyDIcQGPInmnXUd51RPbPi-AS8TQ0n0tSI0pixOuEG08fzzJGKKNX5hYFw47sdULRSL4DAsGporoo3_cUJH9Kao61qU-NaZw7qgS9CJwcm1Xw8zCwCTEy-cySNq0gt6V_aValvTGpfgHnmdzWM47GyK375O2Is1bwu3gIDdl4yf9fwNxNb4hBuCK2S84SmvQDrPFJuGz4b1cE9K0hogCLnkAHfe3-7DrekXuXLiVA1Y_vTbMxQSu0C8THG5s1P6GH5rBV5oG5LOaVftWSIV8UX5vDSlzyAPnrncdMu5sCvXcZi0sxtk2ouAgPQs-bS5Y3onRUdQLn_Se-pQ)

```
{
  "nbf": 1542147994,
  "exp": 1544739994,
  "iss": "https://localhost:44332",
  "aud": [
    "https://localhost:44332/resources",
    "a",
    "b",
    "c",
    "d",
    "e",
    "cat",
    "dog"
  ],
  "client_id": "arbitrary-resource-owner-client",
  "role": [
    "application",
    "limited"
  ],
  "query": [
    "dashboard",
    "licensing"
  ],
  "seatId": "8c59ec41-54f3-460b-a04e-520fc5b9973d",
  "piid": "2368d213-d06c-4c2a-a099-11c34adc3579",
  "nudibranch_watermark": "Daffy Duck",
  "scope": [
    "a",
    "b",
    "c",
    "d",
    "e"
  ],
  "amr": [
    "agent:username:agent0@supporttech.com",
    "agent:challenge:fullSSN",
    "agent:challenge:homeZip"
  ],
  "custom_payload": {
    "some_string": "data",
    "some_number": 1234,
    "some_object": {
      "some_string": "data",
      "some_number": 1234
    },
    "some_array": [
      {
        "a": "b"
      },
      {
        "b": "c"
      }
    ]
  }
}
```
 
