# Extension Grant: arbitrary_identity [WORK IN PROGRESS]
## Access Token Request  

   The client makes a request to the token endpoint by adding the
   following parameters using the "application/x-www-form-urlencoded"
   format per Appendix B with a character encoding of UTF-8 in the HTTP
   request entity-body:

<dl>
  <dt><h2>grant_type</h2></dt>
  <dd><b>REQUIRED</b>.  Value MUST be set to "<b>arbitrary_identity</b>".</dd>

  <dt><h2>subject</h2></dt>
  <dd><b>REQUIRED</b>.  The passed through subject.</dd>
  
  <dt><h2>client_id</h2></dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt><h2>client_secret</h2></dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt><h2>scope</h2></dt>
  <dd><b>REQUIRED</b>.  The scope of the access request as described by
         Section 3.3.       
	<b>i.e. <em>scope:offline_access a b c d e</em></b></dd>
	 
  <dt><h2>arbitrary_claims</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json string object of key/value pairs.  
	i.e. <em>arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}</em></dd>

  <dt><h2>arbitrary_amrs</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]</em></dd>
	
  <dt><h2>arbitrary_audiences</h2></dt>
  <dd><b>OPTIONAL</b>.  This is a json array of strings.  
	i.e. <em>arbitrary_audiences:["cat","dog"]</em></dd>

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
	grant_type:arbitrary_identity
	client_id:arbitrary-resource-owner-client
	client_secret:secret
	scope:offline_access a b c d e
	arbitrary_claims:{↵	"preferred_username": ["ted@ted.com"],↵	"name": ["ted@ted.com"]↵}
	subject:886bea3f-e025-4ab9-a811-e9b86f563668
	access_token_lifetime:3600
	arbitrary_amrs:["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]
	arbitrary_audiences:["cat","dog"]
	custom_payload:{"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}
```
 
Produces...  

```
{
    "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDgwMDgsImV4cCI6MTU0MjE0ODMwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDIxNDgwMDgsImF0X2hhc2giOiJ3M1RvUGpjVDBZRjBINmFmT1luMUdBIiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQyMTQ4MDA4LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.mCsJ_rHNAQizedUqzXZwfn-MKXCXBimeVGR97WrtFT0PMOn6rhJIRom-mJCUmGEceis9oJjkLYbbL1L-E8z3iLjX03P3ETNbR7VWUSkGRMlYrGnhPaVmBlOssDqWOley2OzElO0HMO7CdkoqKnPZwb7EvRVDRgWxep7aDt74mxnn2kC5-W609PLyiqYGBnE3X8bV5b4sDaeZiTUIirUsvdnnoxxHZ5pkCK1a7fnpA1ofzKoZI2TUJ03PE1u_q_Rjapfl9citWFHw6xsVNDe2pU57UWNp-xnMF6hvBvMRzo1aVXyE1v2Rf4c6T67OoeOrhyjSLrLu_Aix-61QOMQijA",
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDgwMDgsImV4cCI6MTU0MjE1MTYwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MjE0ODAwOCwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.Q2Zzjb2YcAgEhHWJv4MDZbeJ2czN_o7jpfncZaYxodCvknj921ZvFlzFhJDoegsf4WLU6fNIGCI3aAlnLFG_8vWLFdTyuRdiblsH_lysjo_WU5Q_J2wAl7qH7CInrsYvxth4CiOpMC1wTKjurLHjWWRwoYbhGxOt4twgsGXe0c6uCXE7-_9Teg_CA4uhV1z63seNS5MKt5dEB5DNd9Efs2mzZqXXvxA2BfFTR3Z8j1MAfhxqEtMVDf2iloVEp90AUVse5EyTksJ_FVUNZkupqsniv7ubi2zA33XRJP6V-XNKDFzRFVgHDMHbp1lgOVDbGxsuI1QjqFWGz77cmqvC-w",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8KHr4bbINrxKkapSXCyORFWkxJ0_jsv64hGbSHq1Ours7cyciI6c12RKWSUR3ezUXfz8P97Dhmt3Q8ezApZAlBx9Q_DBRrQUSauBb7WzGrgcPn9xVVh7nMiZHGbQ7CbiO3Cle8EVd7e4sTE6IvpuRHGCRu1tykvterL4n28t--u20t5p4SYcWjQVupOyFo83CDO1qWEEDqQ5DAQOcU5Y3qI"
}
 ```
 [Decode id_token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDgwMDgsImV4cCI6MTU0MjE0ODMwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsImNhdCIsImRvZyJdLCJpYXQiOjE1NDIxNDgwMDgsImF0X2hhc2giOiJ3M1RvUGpjVDBZRjBINmFmT1luMUdBIiwic3ViIjoiODg2YmVhM2YtZTAyNS00YWI5LWE4MTEtZTliODZmNTYzNjY4IiwiYXV0aF90aW1lIjoxNTQyMTQ4MDA4LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6InRlZEB0ZWQuY29tIiwibmFtZSI6InRlZEB0ZWQuY29tIiwibnVkaWJyYW5jaF93YXRlcm1hcmsiOiJEYWZmeSBEdWNrIiwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.mCsJ_rHNAQizedUqzXZwfn-MKXCXBimeVGR97WrtFT0PMOn6rhJIRom-mJCUmGEceis9oJjkLYbbL1L-E8z3iLjX03P3ETNbR7VWUSkGRMlYrGnhPaVmBlOssDqWOley2OzElO0HMO7CdkoqKnPZwb7EvRVDRgWxep7aDt74mxnn2kC5-W609PLyiqYGBnE3X8bV5b4sDaeZiTUIirUsvdnnoxxHZ5pkCK1a7fnpA1ofzKoZI2TUJ03PE1u_q_Rjapfl9citWFHw6xsVNDe2pU57UWNp-xnMF6hvBvMRzo1aVXyE1v2Rf4c6T67OoeOrhyjSLrLu_Aix-61QOMQijA)
 ```
{
  "nbf": 1542148008,
  "exp": 1542148308,
  "iss": "https://localhost:44332",
  "aud": [
    "arbitrary-resource-owner-client",
    "cat",
    "dog"
  ],
  "iat": 1542148008,
  "at_hash": "w3ToPjcT0YF0H6afOYn1GA",
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1542148008,
  "idp": "local",
  "preferred_username": "ted@ted.com",
  "name": "ted@ted.com",
  "nudibranch_watermark": "Daffy Duck",
  "amr": [
    "arbitrary_identity",
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
 [Decode access_token via jwt.io](https://jwt.io/#debugger-io?token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDIxNDgwMDgsImV4cCI6MTU0MjE1MTYwOCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiYSIsImIiLCJjIiwiZCIsImUiLCJjYXQiLCJkb2ciXSwiY2xpZW50X2lkIjoiYXJiaXRyYXJ5LXJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6Ijg4NmJlYTNmLWUwMjUtNGFiOS1hODExLWU5Yjg2ZjU2MzY2OCIsImF1dGhfdGltZSI6MTU0MjE0ODAwOCwiaWRwIjoibG9jYWwiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm5hbWUiOiJ0ZWRAdGVkLmNvbSIsIm51ZGlicmFuY2hfd2F0ZXJtYXJrIjoiRGFmZnkgRHVjayIsInNjb3BlIjpbImEiLCJiIiwiYyIsImQiLCJlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9pZGVudGl0eSIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.Q2Zzjb2YcAgEhHWJv4MDZbeJ2czN_o7jpfncZaYxodCvknj921ZvFlzFhJDoegsf4WLU6fNIGCI3aAlnLFG_8vWLFdTyuRdiblsH_lysjo_WU5Q_J2wAl7qH7CInrsYvxth4CiOpMC1wTKjurLHjWWRwoYbhGxOt4twgsGXe0c6uCXE7-_9Teg_CA4uhV1z63seNS5MKt5dEB5DNd9Efs2mzZqXXvxA2BfFTR3Z8j1MAfhxqEtMVDf2iloVEp90AUVse5EyTksJ_FVUNZkupqsniv7ubi2zA33XRJP6V-XNKDFzRFVgHDMHbp1lgOVDbGxsuI1QjqFWGz77cmqvC-w)
 ```
{
  "nbf": 1542148008,
  "exp": 1542151608,
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
  "sub": "886bea3f-e025-4ab9-a811-e9b86f563668",
  "auth_time": 1542148008,
  "idp": "local",
  "preferred_username": "ted@ted.com",
  "name": "ted@ted.com",
  "nudibranch_watermark": "Daffy Duck",
  "scope": [
    "a",
    "b",
    "c",
    "d",
    "e",
    "offline_access"
  ],
  "amr": [
    "arbitrary_identity",
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
