---
uid: transact
---

# Transactions & Requests
This page will be divided into three major segments:
- LNM Transactions
- B2B/C Transactions
- Utilities

Before we begin, it is good to appreciate that almost of all Daraja Transactions are asynchronous meaning that you will usually be waiting for callbacks on `queURL` etc.
In the same sprit, apart from OAuth, all other transactions will return an `awaitable` (`System.Threading.Tasks.Task`)

The resolved response will interface `RestSharp.IRestResponse<T>` where `T` is determined by the specific transaction.
Checkout the different [response types](xref:Safaricom.Mpesa.Responses) we have.

## Lipa Na Mpesa

To use these APIs make sure that `LNMShortCode & LNMPassWord` are set.

## LNM Payment
This is used trigger an LNM Transaction.

```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var res = await mpesa.LipaNaMpesaOnline(msisdn, 100, callbackURL, "Some Ref");
var transactionId = res.Data.CheckoutRequestID;
```

## Utilities

### Account Balance
This API request Daraja for a callback with the account balance.
```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var accountyParty = new IdentityParty(configs.ShortCode, IdentityParty.IdentifierType.SHORTCODE);
var res = await mpesa.AccountBalance(partyA, "some URL", "some URL");
var transactionId = res.Data.ConversationID;
```
_See Also:_
- [AccountBalance API documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_AccountBalance_)
- The expected [Daraja AccountBalance Callback Response](https://developer.safaricom.co.ke/docs?json#account-balance-api) to the `resultURL`.
- [ AccountBalance API Response](xref:Safaricom.Mpesa.Responses.AccountBalanceResponse)

### Authentication
Daraja uses OAuth2.0 hence expects a header `"Authorization", "Basic " + encoded"`.
Remember Daraja tokens expire after 60min. Don't worry though, this is all taken care in this library.
```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var token = mpesa.Auth().Data.AccessToken
```
or asynchronous
```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var res = await mpesa.AuthAsync();
var token = res.Data.AccessToken
```
_See Also:_
- [Auth API Documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_Auth_)
- [Auth Async API Documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_AuthAsync_)
- The expected [Daraja Callback Response](https://developer.safaricom.co.ke/docs?json#authentication)
- [Auth API Response](xref:Safaricom.Mpesa.Responses.AuthResponse)

### C2B Register
In the [Terminology](xref:terminology) page, we have discussed several terms and noted that C2B transactions are triggered by the user.
This call is used to register the validation and confirmation URLs and can only be run once in [Production Environment](xref:Safaricom.Mpesa.Api.Env#Safaricom_Mpesa_Api_Env_Production), so use it smartly. You can always consult Daraja and request changes, though this may take time.

```c#
var res = await mpesa.C2BRegister(confirmationURL, validationURL)
var transactionId = res.Data.ConversationID;
```
_See Also:_
- [C2BRegister API Documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_C2BRegister_)
- The expected [Daraja Responses](https://developer.safaricom.co.ke/docs?json#c2b-api)
- [C2BRegister API Response](xref:Safaricom.Mpesa.Responses.C2BRegisterResponse)

### Reversal
Sometimes you just want your money back(We wont judge ;)) and hence the [Reversal](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_ReversalRequest_) Api.

```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var res = await mpesa.ReversalRequest("LKXXXX1234", 100, queueUrl, resultUrl );
var transactionId = res.Data.ConversationID;
```
_See Also:_
- [Reversal API Documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_Reversal)
- The expected [Daraja Responses](https://developer.safaricom.co.ke/docs?json#reversal)
- [Reversal API Response](xref:Safaricom.Mpesa.Responses.ReversalResponse)

### Transction Status
The Transaction Status API is a very powerful API that can be important when you did not get the callback.

```c#
Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
var res = await mpesa.TransactionStatus("MKXXXX1234", queueUrl, resultUrl );
var transactionId = res.Data.ConversationID;
```
_See Also:_
- [TransactionStatus API Documentation](xref:Safaricom.Mpesa.Api#Safaricom_Mpesa_Api_TransactionStatus_)
- The expected [Daraja Responses](https://developer.safaricom.co.ke/docs?json#transaction-status)
- [TransactionStatus API Response](xref:Safaricom.Mpesa.Responses.TransactionStatusResponse)
