# Managing the LifeTime of Tokens 

If you are simply minting tokens that don't require offline_access, aka you don't need a refresh_token, then the role of this service is to simply provide a discovery endpoint for your public keys.
There is no lifetime to manage here.

However, if you do require offline_access, which means you are probably passing refresh_tokens around, then you will need custodial services to manage the lifetime of the refresh_token.

The current implemenation of this service uses [redis](https://redis.io/) simply to persist refresh_tokens.  More specifically it is persiting everything it needs to remint when a refresh_token grant comes in.

## [OAuth 2.0 Token Revocation](https://tools.ietf.org/html/rfc7009)
Since this is a token minting service, the current configuration is scoped to revoke every refresh_token that is associated with a subject regardless of what client minted it.

The revocation configuration can be;

scope | description
--------- | -
refresh_token | Only this refresh_token is revoked.
client | All refresh_tokens that this client created that are associated with the same subject are revoked.
subject | [current config]All refresh_tokens associated with the same subject are revoked.

