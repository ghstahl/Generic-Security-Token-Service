# Revocation


# Endpoint: /connect/revocation

## Revocation Request  

   The client constructs the request by including the following
   parameters using the "application/x-www-form-urlencoded" format in
   the HTTP request entity-body:

<dl>

  <dt><h2>client_id</h2></dt>
  <dd><b>REQUIRED</b>.  The client identifier issued to the client during
         the registration process described by Section 2.2.</dd>
  
  <dt><h2>client_secret</h2></dt>
  <dd><b>REQUIRED</b>.  The client secret.  The client MAY omit the
         parameter if the client secret is an empty string.</dd>
  
  <dt><h2>token_type_hint</h2></dt>
  <dd><b>OPTIONAL</b>. A hint about the type of the token
           submitted for revocation.  Clients MAY pass this parameter in
           order to help the authorization server to optimize the token
           lookup.  If the server is unable to locate the token using
           the given hint, it MUST extend its search across all of its
           supported token types.  An authorization server MAY ignore
           this parameter, particularly if it is able to detect the
           token type automatically.  This specification defines <b><em>three</em></b>
           such values:  
          <dl>
             <dt>access_token</dt>
              <dd>revokes the all refresh_tokens that have references to the subject for the calling client.</dd>
            <dt>refresh_token</dt>
              <dd>revokes the all refresh_tokens that have references to the subject for the calling client.</dd>
               <dt>subject</dt>
            <dd>revokes the all refresh_tokens that have references to the subject for the calling client.</dd>
          </dl>
	
</dd>
	 
  <dt><h2>token</h2></dt>
  <dd><b>REQUIRED</b>.  This is one of the 3 token types.  
	 <dl>
      <dt>access_token</dt>
      <dt>refresh_token</dt>
      <dt>subject</dt>
  </dl>
  <dt><h2>revoke_all_subjects</h2></dt>
  <dd><b>OPTIONAL</b>.  When set to true, this will revoke across all clients withing a shared namespace.  </dd>
	

</dl>  

## Example  
I use [Postman](https://www.getpostman.com/)  

```
POST http://localhost:21354/connect/revocation

Headers:
    	Content-Type:application/x-www-form-urlencoded

Body:
  client_id:arbitrary-resource-owner-client
  client_secret:secret
  token_type_hint:refresh_token
  token:CfDJ8KHr4bbINrxKkapSXCyORFXzz0UeSHGsDGePP6W2dw9JcDxPruU2pS5QfSc4ewtI5sq5poz2scfUH9dhnfpYuoPIdfFKmCQEpPARa8-ejMwUzXRZKhm6zO0GVT4ru0FTkku-iHb5mip1APQlmfDYcSULWzuoC8r9PhYtqAE39HT-n93nWGkL9LllR7cG7Z9HWDWEgZKUikQqJTk5nvicL0Q
  revoke_all_subjects:true
```
```
POST http://localhost:21354/connect/revocation

Headers:
    	Content-Type:application/x-www-form-urlencoded

Body:
  client_id:arbitrary-resource-owner-client
  client_secret:secret
  token_type_hint:subject
  token:PorkyPig
  revoke_all_subjects:true
 ```
 ```
POST http://localhost:21354/connect/revocation

Headers:
    	Content-Type:application/x-www-form-urlencoded

Body:
  client_id:arbitrary-resource-owner-client
  client_secret:secret
  token_type_hint:access_token
  token:eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3MjhhYTE5M2VhZTMyMzg0M2ZjNTZlOTNlN2MwZDFiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDMyNzM3NjksImV4cCI6MTU0MzI3NzM2OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvcmVzb3VyY2VzIiwiRmxhbWVzIiwiSW4iLCJtZXRhbCIsIm5pdHJvIiwiY2F0IiwiZG9nIl0sImNsaWVudF9pZCI6ImFyYml0cmFyeS1yZXNvdXJjZS1vd25lci1jbGllbnQiLCJzdWIiOiJQb3JreVBpZyIsImF1dGhfdGltZSI6MTU0MzI3Mzc2OSwiaWRwIjoibG9jYWwiLCJ0b3AiOiJUb3BEb2ciLCJyb2xlIjpbImFwcGxpY2F0aW9uIiwibGltaXRlZCJdLCJxdWVyeSI6WyJkYXNoYm9hcmQiLCJsaWNlbnNpbmciXSwic2VhdElkIjoiOGM1OWVjNDEtNTRmMy00NjBiLWEwNGUtNTIwZmM1Yjk5NzNkIiwicGlpZCI6IjIzNjhkMjEzLWQwNmMtNGMyYS1hMDk5LTExYzM0YWRjMzU3OSIsImNsaWVudF9uYW1lc3BhY2UiOiJEYWZmeSBEdWNrIiwic2NvcGUiOlsiRmxhbWVzIiwiSW4iLCJtZXRhbCIsIm5pdHJvIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbImFyYml0cmFyeV9yZXNvdXJjZV9vd25lciIsImFnZW50OnVzZXJuYW1lOmFnZW50MEBzdXBwb3J0dGVjaC5jb20iLCJhZ2VudDpjaGFsbGVuZ2U6ZnVsbFNTTiIsImFnZW50OmNoYWxsZW5nZTpob21lWmlwIl0sImN1c3RvbV9wYXlsb2FkIjp7InNvbWVfc3RyaW5nIjoiZGF0YSIsInNvbWVfbnVtYmVyIjoxMjM0LCJzb21lX29iamVjdCI6eyJzb21lX3N0cmluZyI6ImRhdGEiLCJzb21lX251bWJlciI6MTIzNH0sInNvbWVfYXJyYXkiOlt7ImEiOiJiIn0seyJiIjoiYyJ9XX19.SZr-iZDLIaQ41qwYJm_yWb4IIzYNUrxuMaNhMzERRjx0Ico611gNCs25d5qji97BI7gGcbxVwbFxufpcb5a9BoxY2nlcVqV7N3OzpLuwpryjbO8sLYkhng1yUR1zxSd1lxQDyKSUQ1qkfGmTyp-B3rgPY878J0Ep0k-vsMtQQrno5MNzNIdTMzwdDgT0d8q8dctqu0xKtzwD-BrzKFLgn5ss9mQG3OYdUEqsckarNJnWgRmgNtLCRRgMOvzRMjWJO0DyNNTnI0w6_dSIeo0GobhI5OMpTDO8OlJIfmbRytF44SrINh1apK8C6INUb9I7PzqORfvVvZQcSDRd9e-pZA
  revoke_all_subjects:true
 ```