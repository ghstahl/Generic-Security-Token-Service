# ID_TOKEN
## An opinionated view  

[The OpenId spec](https://openid.net/specs/openid-connect-core-1_0.html)  

My view of and id_token maps onto my view of other identifying tokens like your passport or drivers license.

### An identity to me consists of;
#### 1. Who are you  
```
sub
REQUIRED. Subject Identifier. A locally unique and never reassigned identifier within the Issuer for the 
End-User, which is intended to be consumed by the Client, 
e.g., 24400320 or AItOawmwtWwcT0k51BayewNvutrJUqsvl6qs7A4. It MUST NOT exceed 255 ASCII characters in length. 
The sub value is a case sensitive string.
```
#### 2. How did you get this identity  
```
amr
OPTIONAL. Authentication Methods References. JSON array of strings that are identifiers for authentication 
methods used in the authentication. For instance, values might indicate that both password and OTP authentication 
methods were used. The definition of particular values to be used in the amr Claim is beyond the scope of this 
specification. Parties using this claim will need to agree upon the meanings of the values used, which may be 
context-specific. The amr value is an array of case sensitive strings.
```
These are now custom values that an idp must put in.  This indicates what the entity provided in order to get an id_token issued.  
1. password
```
"amr": [
    "pwd"
  ]
```
2. password + 2FA
```
"amr": [
    "pwd","2FA"
  ]
```
3. an agent promted set of challenge questions.
```
"amr": [
    "agent:bob@supporttech.com","challenge:fullSSN","challenge:home_zip","challenge:birth_town"
  ]
```
4. an abstract pointer to an identity.  i.e. your cars VIN will chain up to a user either by looking at DMV records or Insurance company records
```
"amr": [
    "iot:my_sweet_iot","app_id:3423","mid_id:34253"
  ]
```
#### 3. When did you get this identity
```
auth_time
Time when the End-User authentication occurred. Its value is a JSON number representing the number of seconds 
from 1970-01-01T0:0:0Z as measured in UTC until the date/time. When a max_age request is made or when auth_time 
is requested as an Essential Claim, then this Claim is REQUIRED; otherwise, its inclusion is OPTIONAL. 
(The auth_time Claim semantically corresponds to the OpenID 2.0 PAPE [OpenID.PAPE] auth_time response parameter.)

iat
REQUIRED. Time at which the JWT was issued. Its value is a JSON number representing the number of seconds from 
1970-01-01T0:0:0Z as measured in UTC until the date/time.
```
#### 4. When does this identity expire
```
exp
REQUIRED. Expiration time on or after which the ID Token MUST NOT be accepted for processing. The processing of 
this parameter requires that the current date/time MUST be before the expiration date/time listed in the value. 
Implementers MAY provide for some small leeway, usually no more than a few minutes, to account for clock skew. 
Its value is a JSON number representing the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until 
the date/time. See RFC 3339 [RFC3339] for details regarding date/times in general and UTC in particular.
```
#### 5. Can I trust the issuer?  
```
iss
REQUIRED. Issuer Identifier for the Issuer of the response. The iss value is a case sensitive URL using the 
https scheme that contains scheme, host, and optionally, port number and path components and no query or fragment 
components.
```
[Validating an id_token](https://openid.net/specs/openid-connect-core-1_0.html#IDTokenValidation)

 

### What I find completely useless;
Pretty much anything that implies intent.  Intent is after the fact.  A good example is using your identity to buy an airline ticket.
You intend to buy the ticket.  The issuer of the ticket requires one or more identities, which only have to pass its rules or authenticity.  It would be ludicrous to think that your passport would have an audience field that indicated it was only to be used for an airlines ticketing service.


#### Audience. 
```
aud
REQUIRED. Audience(s) that this ID Token is intended for. It MUST contain the OAuth 2.0 client_id of the Relying Party 
as an audience value. It MAY also contain identifiers for other audiences. In the general case, the aud value is an 
array of case sensitive strings. In the common special case when there is one audience, the aud value MAY be a single 
case sensitive string.
```
There is no such thing as an intended audience on a passport.  The audience is whomever is willing to accept the passport and can the passports authenticity be trusted.  

### What I find helpful;

#### Nonce.  
```
nonce
String value used to associate a Client session with an ID Token, and to mitigate replay attacks. The value is passed 
through unmodified from the Authentication Request to the ID Token. If present in the ID Token, Clients MUST verify 
that the nonce Claim Value is equal to the value of the nonce parameter sent in the Authentication Request. If present 
in the Authentication Request, Authorization Servers MUST include a nonce Claim in the ID Token with the Claim Value 
being the nonce value sent in the Authentication Request. Authorization Servers SHOULD perform no other processing on 
nonce values used. The nonce value is a case sensitive string.
```
I use this more of a transport security where I as a web app initiated a login to an IDP.  I want assurances that the redirect I get back is one that I initiated.

Alternatively to the NONCE is to accept an id_token from anywhere.  This is the problem of the service that may have rules that will only accept an id_token if it was issued 5 minutes ago.  
If you have a mobile app that has to log in, you may want to encapsulate the login and the subsequent exchange of the id_token for access_tokens in one secure place.  I favor that one secure place to belong to the service authors and not a centralized single authority.



