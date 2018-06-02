
# Passing a Simple JWT Around 
:::card
<div class="bg-light p-4">
 <p class="mb-0">JSON Web Token (JWT) is a compact, URL-safe means of representing claims to be transferred between two parties. The claims in a JWT are encoded as a JavaScript Object Notation (JSON) object that is used as the payload of a JSON Web Signature (JWS) structure or as the plaintext of a JSON Web Encryption (JWE) structure, enabling the claims to be digitally signed or MACed and/or encrypted.
</p>
</div>
:::

So you like a JWT because its a digitally signed way of sharing claims with external parties.  Exactly how is that external party going to validate that JWT?

Using a service like this, you benifit from the openid-configuration discovery endpoint.  In particular the "jwks_uri".

i.e  The following jwk_uri;
:::card
<div class="bg-light p-4">
 <p class="mb-0">
 https://localhost:44332/.well-known/openid-configuration/jwks
</p>
</div>
:::
 
produces...
```
{
	"keys": [{
		"kty": "RSA",
		"use": "sig",
		"kid": "a728aa193eae323843fc56e93e7c0d1b",
		"e": "AQAB",
		"n": "5KJdT36_ilqs6GXHke-fKY63T0fzkSce3ZNCk1m-GprY7Jnd3DmPedbFGpFCHtLqAsjTi_tCLtgoSvZvnu9u11u9f6xLCYEyYOQ3m69cdZ2l6m2nigdNCJBmfqzMz98G7ecnR1F3RNLFsoaZqTanrR0gNtcUYm2C1YAtvW__czwPj7ECD1YkgQuqFm0_vYw1EnZNI1IcGwz7qaYC0BZiKJVCrkcNZ095_xLCDyi-FWOcjSwWfrCrIxpIAppsYlN3wLOas8RuUDuTDpaSUoich7QzZaPVlOlMuHUcWJkz53dqj1Q9MA6KsLHxo3l4TqYQjXhgkLDwU5xkjb4NRI-vtQ",
		"alg": "RS256"
	}]
}
```
Standardizing discovery of your public keys is the win here.

With standardization comes open-source libraries like the C# [IdentityModel library](https://github.com/IdentityModel/IdentityModel2).
This library has a DiscoveryCache when pointed to the OAuth2 authority will pull all the certs for you and then lets you validate in a standard way.

