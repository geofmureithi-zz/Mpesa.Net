---
uid: getting-started
---
# Quick Start
## Some few notes
This library is built to make your work easier.
We are aware that a lot of times in Daraja you have to keep sending arguments like shortCode, identifier type.
In this library these are defaulted when possible hence you will definitely only pass what is needed.
Feel free to submit a PR to improve this.

## Creating an instance
```c#
var mpesaAPI = new Api(Api.Env.SANDBOX, "consumerKey", "consumerSecret", Extra config = null);
```
From the example above we can note that we basically need:
1. [Env](xref:Safaricom.Mpesa.Api.Env) is the environment which transactions will executed ie `Sandbox | Production`
2. ConsumerKey and ConsumerSecret provided from Daraja. See [Authentication](https://developer.safaricom.co.ke/docs?json#authentication)
3. [Extra Config](xref:Safaricom.Mpesa.Api.ExtraConfig) includes configs used for LNM & security credential transactions. It is *RECOMMENDED* though optional (most transactions will need some of those configs).  
Below is an example:

```c#
 ExtraConfig configs = new ExtraConfig
 {
   ShortCode = 6000000,
   Initiator = "testapikkk",
   LNMShortCode = 123456,
   LNMPassWord = "pa55WOrd",
   SecurityCredential = "ddsds",
   CertPath = "~/c-sharp-mpesa-lib/Mpesa/cert.cer"
};
```
## Getting your credentials

Creds for Sandbox should be available on Daraja. For Production, you will need to fill in the go to production form.
See image below:
![](/images/creds.png)

For going live, click [here](https://developer.safaricom.co.ke/production_profile/form_production_profile)

## Multiple Paybills & Environments

Thats Simple!
```c#
var prodAPI = new Api(Api.Env.Production, "consumerKey1", "consumerSecret1", Extra config = null);
var sandboxAPI = new Api(Api.Env.Sandbox, "consumerKey2", "consumerSecret2", Extra config = null);
```
Each instance is exclusive of the other.

Now head to [Doing Transctions](xref:transact)
