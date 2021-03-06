# CSharp-Payture-official

This is Offical Payture API for C#. We're try to make this as simple as possible for you! Explore tutorial and get started. Please note, you will need a Merchant account,  contact our support to get one. 
Here you can explore how to use our API functions!

## Install
Just download our package from NuGet *(please, take into account that this is the test package now)*:  **CSharpPaytureAPI** (last version is 1.0.4)


And include to your project:
```c#
using CSharpPayture;
```

## Payture API tutorial
Before fall into the deep, we're need to provide you general conception of working with our API function. See picture: 
![](generalCSharp.jpg)

## [Steps](#newMerchant)

 * [Creating merchant account](#newMerchant)
 * [Get access to required API](#accessToAPI)
 * [Expand transaction](#expandTransaction)
 * [Send request](#sendRequest)

## [Base Types](#baseTypes)
* [PayInfo](#PayInfo)
* [Card](#Card)
* [Data](#Data)
* [PaytureCommands](#PaytureCommands)
* [Customer](#Customer)
* [PaytureResponse](#PaytureResponse)
* [CardInfo](#CardInfo)
* [Transaction](#Transaction)

## [Test App](#testApp)

Now, let's walk through the steps from the picture above



## First Step - Creating Merchant Account <a id="newMerchant" ></a>
To get access for API usage just create the instance of Merchant object, pass in the constructor the name of the host, name of your account and your account password.  Suppose that you have Merchant account with  name: Key = "MyMerchantAccount" and password Password = "MyPassword".

Pass the 'https://sandbox.payture.com' for test as the name of Host (first parameter).
```c#
var merchant = new Merchant("https://sandbox.payture.com", "MyMerchantAccount", "MyPassword");
```
We're completed the first step! Go next!
***
Please note, that  Key = "'MyMerchantAccount" and Password = "MyMerchantAccount"  - fake, [our support](http://payture.com/kontakty/) help you to get one!
***

## Second Step - Get access to required API <a id="accessToAPI" ></a>
At this step you just call one of following methods on Merchant object (which provide proper API type for you) and pass in the PaytureCommands [see description here](#PaytureCommands): 
* Api (this is PaytureAPI)
```c#
merchant.Api( PaytureCommands.Pay );
```
* InPay (this is PaytureInPay)
```c#
merchant.InPay( PaytureCommands.Pay );
```
* EWallet (this is PaytureEWallet)
```c#
merchant.EWallet( PaytureCommands.Init );
```
* Apple (this is PaytureApplePay)
And pass in the [PaytureCommands](#PaytureCommands).
```c#
merchant.Apple( PaytureCommands.Pay );
```
* Android (this is PaytureAndroidPay)
And pass in the [PaytureCommands](#PaytureCommands).
```c#
merchant.Android( PaytureCommands.Pay );
```
Result of this methods is the instanse of Transaction object which you expand in the next step. 

 [See this table](#PaytureCommandsTable) for explore what PaytureCommands received  theese methods.

## Third Step - Expand transaction <a id="extpandTransaction" ></a>
This is the most difficult step, but you can do it!
In the previous step we get the Transaction object [see here that is it](#Transaction). You need expand it, below you find detailed description how do this for every type of api.

At this step we're call only one method: ExpandTransaction(...). But there are more overload exist!
### ExpandTransaction ( string orderId, Int64? amount )
This overload available in any of the API type

Call this for following PaytureCommands:
* Unblock
* Refund
* Charge
* GetState (PaytureAPI)
* PayStatus (PaytureEWallet, PaytureInPay)

| Parameter's name | Definition                                                        |
| ---------------- | ----------------------------------------------------------------- |
| orderId          | Payment identifier in your service system.                        |
| amount           | Amount of payment kopec. (in case of GetState or PayStatus pass null)                                          |


Example for Charge:

> Note Charge operation we're can make after the funds on Customer's card was blocked.
```c#
var orderId = "TESTORD000000000000000000"; //pass in the transaction's OrderId used in PaytureCommands.Block operation.
var amount = 7444444; //transaction's Amount used in PaytureCommands.Block

//Create and expand transaction for Api:
var payTransactionApi = merchant.Api( PaytureCommands.Charge ).ExpandTransaction( orderId, amount );

//Create and expand transaction for InPay:
var payTransactionInPay = merchant.InPay( PaytureCommands.Charge ).ExpandTransaction( orderId, amount );

//Create and expand transaction for EWallet:
var payTransactionEWallet = merchant.EWallet( PaytureCommands.Charge ).ExpandTransaction( orderId, amount );
```

### ExpandTransaction Methods for PaytureAPI
#### ExpandTransaction( PayInfo info, IDictionary<string, string> customFields, string customerKey, string paytureId  )
This overload you call for api methods:
* **Pay** (PaytureCommands.Pay).
* **Block**  (PaytureCommands.Block).

Description of provided params.

| Parameter's name | Definition                                                                             |
| ---------------- | -------------------------------------------------------------------------------------- |
| info             | Params for transaction processings [see here for explore PayInfo object](#PayInfo)     |
| customerKey      | Customer identifier in Payture AntiFraud system.                                       |
| customFields     | Additional fields for processing (especially for AntiFraud system).                    |
| paytureId        | Payments identifier in Payture AntiFraud system.                                       |


Example for Pay:
```c#
var payInfo = new PayInfo(
    "4111111111111112", // card number, required
     10, //expiration month, required
     20, //expiration year, required
     "Test Test", // cardholder name, required
     123, // secure code, required
     "TestOrder0000000000512154545", // payment's identifier in Merchant system
     41000 //amount, required
);
var customFields = new Dictionary<string, string>{
    { "IP", "93.120.05.36" },
    { "Description", "SomeUsefullHere" }
}; //optional, can be null 

var customerKey = "testKey"; // 
var paytureId = ""; //optional 

//Create and expand transaction 
var payTransaction = merchant.Api( PaytureCommands.Pay ).ExpandTransaction( payInfo, customFields, customerKey, paytureId );
```

### ExpandTransaction Methods for PaytureInPay
#### ExpandTransaction( Data data )
This overload you call for api **Init** method ( PaytureCommands.Init )
Full description of recieved [data see here](#Data).
You must specify following fields of Data object then call Init api method of PaytureInPay:
* SessionType (maybe Pay or Block)
* OrderId
* Amount
* IP

Other fields is optional. Example:
```c#
var orderId = "TESTORD000000000000000000";
var amount = 102000; // in kopec
var ip = "93.45.120.14";
var data = new Data( SessionType.Pay, orderId, amount, ip );

//Create and expand transaction 
var initTransaction = merchant.InPay( PaytureCommands.Init ).ExpandTransaction( data );
```
> Please note that the response from Init method will be contain SessionId - the unique payment's identifier - further you need to use it in PaytureCommands.Pay  api method for proseccing transaction on Payture side: call manually (suppose, we're have sessionId value from Init):
*  merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( sessionId ) - use for SessionType=Pay or SessionType=Block

> To do the same thing you can take value from response's RedirectURL property - which is string representation of Url constracted for you (a value in RedirectURL will be set only in PaytureCommands.Init response, in over cases it has null value)  - and just redirect customer to this address.


### ExpandTransaction Methods for PaytureEWallet
#### ExpandTransaction( Customer customer, string cardId, Data data ) 
This overload you call for api 
* **Init** (PaytureCommands.Init). 

Example for SessionType=Pay and SessionType=Block:
```c#
var cardId = "40252318-de07-4853-b43d-4b67f2cd2077";
var customer = new Customer( "testCustomerEW", "testPass" ); 
var sessionType = SessionType.Pay; //= SessionType.Block,  required
var orderId = "TESTORD000000000000000000"; // required
var amount = 102000; // in kopec, required
var ip = "93.45.120.14"; // required
var product = "SomeCoolProduct"; // optional, maybe empty or null
var total = amount; // optional, maybe empty or null
var template = "tempTag"; // optional, maybe empty or null
var lang = "RU"; // optional, maybe empty or null
var data = new Data ( sessionType, orderId, amount, ip, product, total, template, lang ) // required

//Create and expand transaction 
var initPayTransaction = merchant.EWallet( PaytureCommands.Init ).ExpandTransaction( customer, cardId, data ); //SessionType=Pay or SessionType=Block
```


Example for SessionType=Add:
```c#
var cardId = null; //we're pass null for Add SessionType
var customer = new Customer( "testCustomerEW", "testPass" ); 
var sessionType = SessionType.Add; //  required
var ip = "93.45.120.14"; // required
var template = "tempTag"; // optional, maybe empty or null
var lang = "RU"; // optional, maybe empty or null
var data = new Data 
{
    SessionType = sessionType.ToString(),
    IP = ip,
    TemplateTag = template,
    Language = lang
}; // required

//Create and expand transaction 
var initAddTransaction = merchant.EWallet( PaytureCommands.Init ).ExpandTransaction( customer, null, data ); // SessionType=Add
```

> Please note that the response from Init method will be contain SessionId - the unique payment's identifier - further you need to use it in PaytureCommands.Pay or PaytureCommands.Add api methods for proseccing transaction on Payture side: call manually (suppose, we're have sessionId value from Init):
*  merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( sessionId ); - use for SessionType=Pay or SessionType=Block
*  merchant.EWallet( PaytureCommands.Add ).ExpandTransaction( sessionId ); - use for SessionType=Add 

> To do the same thing you can take value from response's RedirectURL property - which is string representation of Url constracted for you (a value in RedirectURL will be set only in PaytureCommands.Init response, in over cases it has null value)  - and just redirect customer to this address.

#### ExpandTransaction( Customer customer, string cardId, int secureCode, Data data ) 
This overload you call for api 
* **Pay** (PaytureCommands.Pay) - on Merchant side for REGISTERED card

Example for SessionType=Pay and SessionType=Block:
```c#
var sessionType = SessionType.Pay; //= SessionType.Block,  required
var orderId = "TESTORD000000000000000000"; // required
var cardId = "40252318-de07-4853-b43d-4b67f2cd2077"; // required
var secureCode = 123; // required
var amount = 102000; // in kopec, required
var ip = "93.45.120.14"; // required
var confirmCode = "SomeCoolProduct"; // optional, maybe empty or null
var customFields = ""; // optional maybe null
var customer = new Customer( "testCustomerEW", "testPass" ); //required
var data = new Data ( sessionType, orderId, amount, ip ) // required
data.ConfirmCode = confirmCode;
data.CustomFields = customFields;

//Create and expand transaction 
var payTransaction = merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( customer, cardId, secureCode, data );

```
#### ExpandTransaction( Customer customer, Card card, Data data ) 
This overload you call for api 
* **Pay** (PaytureCommands.Pay) - on Merchant side for NOT REGISTERED card

Example for SessionType=Pay and SessionType=Block:
```c#
var sessionType = SessionType.Pay; //= SessionType.Block,  required
var orderId = "TESTORD000000000000000000"; // required
var amount = 102000; // in kopec, required
var ip = "93.45.120.14"; // required
var confirmCode = "SomeCoolProduct"; // optional, maybe empty or null
var customFields = ""; // optional maybe null
var data = new Data ( sessionType, orderId, amount, ip ) // required
data.ConfirmCode = confirmCode;
data.CustomFields = customFields;

var customer = new Customer( "testCustomerEW", "testPass" ); //required

var card = new Card( 
    "4111111111111112", //card number
    10, //expiration month
    20, //expiration year
    "Card Holder", //CardHolder Name
    111, //secure code
); //required

//Create and expand transaction 
var payTransaction = merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( customer, card, data );

```

#### ExpandTransaction( Customer customer, Card card )
This overload you call for api
* **Add** method ( PaytureCommand.Add ) on Merchant side.

Example:
```c#
var customer = new Customer( "testCustomerEW", "testPass" ); //required
var card = new Card( 
    "4111111111111112", //card number
    10, //expiration month
    20, //expiration year
    "Card Holder", //CardHolder Name
    111, //secure code
); //required

//Create and expand transaction 
var addTransaction = merchant.EWallet( PaytureCommands.Add ).ExpandTransaction( customer, card );
```

Please note, that you can add card *only for registered customer*.


#### ExpandTransaction( Customer customer )
This overload is called for following api methods:

* **Register** (PaytureCommands.Register),
* **Update** (PaytureCommands.Update), 
* **Delete** (PaytureCommands.Delete), 
* **Check** (PaytureCommands.Check), 
* **GetList** (PaytureCommands.GetList)
Description of recieved [Customer data see here](#Customer).

Example for PaytureCommands.Register:
```c#
var customer = new Customer( 
    "testCustomerEW", // login, required
    "testPass", //password, required
    "78456865353", //phone, optional
    "newCustTest@gmailTest@.ru" // email, optional
     ); 

//Create and expand transaction      
var registerTransaction = merchant.EWallet( PaytureCommands.Reqister ).ExpandTransaction( customer );
```



#### ExpandTransaction( Customer customer, string cardId, Int64? amount, string orderId = null )
This overload is called for api methods: 
* **SendCode** (PaytureCommands.SendCode). You need to specify all parameters include orderId.
Example:
```c#
var customer = new Customer( "testCustomerEW", "testPass" ); 
var cardId = "40252318-de07-4853-b43d-4b67f2cd2077";
var amount = 50000; 
var orderId = "TESTORD000000000000000000";

//Create and expand transaction 
var sendCodeTransaction = merchant.EWallet( PaytureCommands.SendCode ).ExpandTransaction( customer, cardId, amount, orderId );
```
* **Activate** (PaytureCommands.Activate). Specify customer, cardId and amount for this operation.
Example:
```c#
var customer = new Customer( "testCustomerEW", "testPass" ); 
var cardId = "40252318-de07-4853-b43d-4b67f2cd2077";
var amount = 100; //pass small amount for activate

//Create and expand transaction 
var activateTransaction = merchant.EWallet( PaytureCommands.Activate ).ExpandTransaction( customer, cardId, amount );
```
* **Remove** (PaytureCommands.Remove). You need to specify customer and cardId only for this operation. For amount pass null.
Example:
```c#
var customer = new Customer( "testCustomerEW", "testPass" ); 
var cardId = "40252318-de07-4853-b43d-4b67f2cd2077";

//Create and expand transaction 
var removeTransaction = merchant.EWallet( PaytureCommands.Remove ).ExpandTransaction( customer, cardId, null );
```


#### ExpandTransaction( string sessionId )
This overload is called for api methods: 
* **Pay** (PaytureCommands.Pay). On Payture side
* **Add** (PaytureCommands.Add). On Payture side

Example for PaytureCommands.Pay:
```c#
var sessionId = "e5c43d9f-2646-42bc-aeec-0b9005ceb972"; //received from PaytureCommands.Init 

//Create and expand transaction 
var payTransaction = merchant.EWallet( PaytureCommands.Pay ).ExpandTransaction( sessionId );
```

#### ExpandTransaction( string MD, string paRes )
This overload is called for api methods: 
* **PaySubmut3DS** (PaytureCommands.PaySubmit3DS).
Example for:
```c#
var md = "20150624160356619170 "; //received from ACS 
var pares = "ODJhYTk0NGUtMDk0ZlKJjjhbjlsrglJKJHNFKSRFLLkjnksdfjgdlgkd.... "; //received from ACS 

//Create and expand transaction 
var paySubmitTransaction = merchant.EWallet( PaytureCommands.PaySubmit3DS ).ExpandTransaction( md, pares );
```

### ExpandTransaction Methods for PaytureApplePay and PaytureAndroidPay
#### ExpandTransaction( string payToken, string orderId, int? amount )
This overload you call for:
* **Pay** (PaytureCommands.Pay) 
* **Block** (PaytureCommands.Block) 
Description of provided params.

| Parameter's name | Definition                                                                             |
| ---------------- | -------------------------------------------------------------------------------------- |
| payToken         | PayToken for current transaction.   |
| orderId          | Current transaction OrderId, if you miss this value (if pass null) - it will be generate on Payture side.    |
| amount           | Current transaction amount in kopec (pass null for ApplePay).                      |



## Last Step - Send request <a id="sendRequest" ></a>
After transaction is expanded you can send request to the Payture server via one of two methods:
* ProcessOperation(); - this is sync method. The executed thread will be block while waiting response from the server - return the PaytureResponse object.
* ProcessOperationAsync(); - this async method, return Task<PaytureResponse> object.


## Base Types <a id="baseTypes"></a>:

### PayInfo <a id="PayInfo"></a>
This object used for PaytureAPI and consist of following fields:

| Fields's name    | Field's type | Definition                                      |
| ---------------- | ------------ | ----------------------------------------------- |
| OrderId          | string       | Payment identifier in your service system.      |
| Amount           | long         | Amount of payment kopec.                        |
| PAN              | string       | Card's number.                                  |
| EMonth           | string       | The expiry month of card.                       |
| EYear            | string       | The expiry year of card.                        |
| CardHolder       | string       | Card's holder name.                             |
| SecureCode       | string       | CVC2/CVV2.                                      |

You can use following constructors for creation PayInfo object:
```c#
var infoFirst = new PayInfo( 
    "4111111111111112", //PAN
    10, //EMonth
    20, //EYear
    "Test Test", //CardHolder
    123, //SecureCode
    "TestOrder0000000000512154545", //OrderId
    580000 //Amount 
    );
var infoSecond = new PayInfo( 
    "4111111111111112", //PAN
    "10", //EMonth
    "20", //EYear
    "Test Test", //CardHolder
    "123", //SecureCode
    "TestOrder0000000000512154545", //OrderId
    580000 //Amount 
    );
```
As you can see theese two conscructors differ the type of recieved arguments for EMonth, EYear and SecureCode fields. Choose the most convenient. Internally theese fields pass a restriction check:
* EMonth - takes values from 1 to 12 inclusively.
* EYear - two last digit of expiry year, must be greate or equal to current year.
* SecureCode - only digit allowed.


### Card <a id="Card"></a>
This object used for PaytureEWallet and consist of following fields:

| Fields's name    | Field's type | Definition                                      |
| ---------------- | ------------ | ----------------------------------------------- |
| CardId           | string       | Card identifier in Payture system.              |
| CardNumber       | string       | Card's number.                                  |
| EMonth           | int          | The expiry month of card.                       |
| EYear            | int          | The expiry year of card.                        |
| CardHolder       | string       | Card's holder name.                             |
| SecureCode       | int          | CVC2/CVV2.                                      |

Examples of creation instance of Card:
```c#
var card = new Card( "4111111111111112", 10, 20, "Test Test", 123 ); //create card with CardId = null
var cardWithId = new Card( "4111111111111112", 10, 20, "Test Test", 123, "40252318-de07-4853-b43d-4b67f2cd2077" ); //create card with CardId = "40252318-de07-4853-b43d-4b67f2cd2077"
```
### Data <a id="Data"></a>
This is object used for PaytureEWallet and PaytureInPay, consist of following fields 

| Fields's name    | Field's type  | Definition                                                                                                          |
| ---------------- | ------------- | ------------------------------------------------------------------------------------------------------------------- |
| SessionType      | string        | Session Type - determines the type of operation. In this object - it's string representation of SessionType enum.   |
| IP               | string        | Customer's IP adress.                                                                                               |
| TemplateTag      | string        | Tamplate which used for payment page.                                                                               | 
| Language         | string        | Addition parameter for determining language of template.                                                            |
| OrderId          | string        | Payment identifier in your service system.                                                                          |
| Amount           | long          | Amount of payment kopec.                                                                                            |
| Url              | string        | The address to which Customer will be return after completion of payment.                                            |
| Product          | string        | Name of product.                                                                                                    | 
| Total            | int?          | Total Amount of purchase.                                                                                           |
| ConfirmCode      | string        | Confirmation code from SMS. Required in case of confirm request for current transaction.                            |
| CustomFields     | string        | Addition transaction's fields.                                                                                      |

Examples of creation instance of Data:
```c#
//public Data( SessionType sessionType, string orderId, long amount, string ip )
var dataFirst = new Data( 
    SessionType.Pay, //SessionType.Pay - for one-stage operation; SessionType.Block - for two-stage operation; SessionType.Add - for adding card (PaytureEWallet)
    "TestOrder0000000000512154545",
    20000, 
    "127.0.0.1"
);

//public Data( SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string url, string template, string lang ) 
var dataSecond = new Data( 
    SessionType.Pay, //SessionType.Pay - for one-stage operation; SessionType.Block - for two-stage operation; SessionType.Add - for adding card (PaytureEWallet)
    "TestOrder0000000000512154545",
    20000, 
    "127.0.0.1",
    "CoolProductName",
    20000,
    "https://url.ru", //return address for customer then payment will be processed
    "MyTemplate",
    "RU"
);


var customFields = new Dictionary<string, string> { //This is addition transaction's fields in PaytureEWallet
    { "Email", "test@test.com" },
    { "CustomerDescription", "SoImpotantInfo" },
    { "AdditionField", "AdditionInfo" }
};
var confirmCode = "123454787" //required in case in confirm request for current transaction OrderId, otherwise pass null
//public Data( SessionType sessionType, string orderId, long amount, string ip, string product, Int64? total, string confirmCode,  IDictionary<string, string> customFields, string template, string lang )
var dataThird = new Data( 
    SessionType.Pay, //SessionType.Pay - for one-stage operation; SessionType.Block - for two-stage operation; SessionType.Add - for adding card (PaytureEWallet)
    "TestOrder0000000000512154545",
    20000, 
    "127.0.0.1",
    "CoolProductName",
    20000,
    confirmCode,
    customFields,
    "MyTemplate",
    "RU"
);
```

### PaytureCommands <a id="PaytureCommands"></a>
This is enum of **all** available commands for Payture API.

PaytureCommands list and availability in every api type

| Command      | Api | InPay | EWallet | Apple | Android | Description                                                                                                            |
| ------------ | --- | ----- | ------- | ----- | ------- | ---------------------------------------------------------------------------------------------------------------------- |
| Pay          |  +  |   +   |    +    |   +   |    +    | Command for pay transaction. In InPay and EWallet can be used for Block operation                                      |
| Block        |  +  |       |         |   +   |    +    | Block of funds on customer card. You can write-off of funds by Charge command or unlocking of funds by Unblock command |
| Charge       |  +  |   +   |    +    |       |         | Write-off of funds from customer card                                                                                  |
| Refund       |  +  |   +   |    +    |       |         | Operation for refunds                                                                                                  |
| Unblock      |  +  |   +   |    +    |       |         | Unlocking of funds  on customer card                                                                                   |
| GetState     |  +  |       |         |       |         | Get the actual state of payments in Payture processing system                                                          |
| Init         |     |   +   |    +    |       |         | Payment initialization, customer will be redirected on Payture payment gateway page for enter card's information       |
| PayStatus    |     |   +   |    +    |       |         | Get the actual state of payments in Payture processing system                                                          |
| Add          |     |       |    +    |       |         | Register new card in Payture system                                                                                    |
| Register     |     |       |    +    |       |         | Register new customer account                                                                                          |
| Update       |     |       |    +    |       |         | Update customer account                                                                                                |
| Check        |     |       |    +    |       |         | Check for existing customer account in Payture system                                                                  |
| Delete       |     |       |    +    |       |         | Delete customer account from Payture system                                                                            |
| Activate     |     |       |    +    |       |         | Activate registered card in Payture system                                                                             |
| Remove       |     |       |    +    |       |         | Delete card from Payture system                                                                                        |
| GetList      |     |       |    +    |       |         | Return list of registered cards for the customer existed in Payture system                                             |
| SendCode     |     |       |    +    |       |         | Additional authentication for customer payment                                                                         |
| Pay3DS       |  +  |       |         |       |         | Command for one-stage charge from card with 3-D Secure                                                                 |
| Block3DS     |  +  |       |         |       |         | Block of funds on customer card with 3-D Secure                                                                        |
| PaySubmit3DS |     |       |    +    |       |         | Commands for completed charging funds from card with 3-D Secure                                                        |


### Customer <a id="Customer"></a>
This object used for PaytureEWallet and consist of following fields:

| Fields's name    | Field's type | Definition                                                       |
| ---------------- | ------------ | ---------------------------------------------------------------- |
| VWUserLgn        | string       | Customer's identifier in Payture system. (Email is recommended). |
| VWUserPsw        | string       | Customer's password in Payture system.                           |
| PhoneNumber      | string       | Customer's phone number.                                         |
| Email            | string       | Customer's email.                                                |

```c#
var customer = new Customer( "testLogin@mail.com", "customerPassword"); //create customer without phone and email
var customer2 = new Customer( "testLogin@mail.com", "customerPassword", "77125141212", "testLogin@mail.com" ); //customer with all fields
```


### PaytureResponse <a id="PaytureResponse"></a>
This object is response from the Payture server and consist of following fields:

| Fields's name    | Field's type                | Definition                                                                                       |
| ---------------- | --------------------------- | ------------------------------------------------------------------------------------------------ |
| APIName          | PaytureCommands             | Name of commands that was called.                                                                |
| Success          | bool                        | Determines the success of processing request.                                                    |
| ErrCode          | string                      | Will be contain code of error if one occur during process the transaction on the Payture server. | 
| RedirectURL      | string                      | Will be contain the new location for redirect. (for PaytureCommands.Init).                       |
| Attributes       | Dictionary<string, string>  | Addition attributes from the response.                                                           |
| InternalElements | dynamic                     | Additional information from the response.                                                        |
| ListCards        | List<CardInfo>              | List of cards, theese registered for current Customer (this field filled for PaytureCommands.GetList)  |
| ResponseBodyXML  | string                      | String representation received from Payture server in XML format                                 |


### CardInfo <a id="CardInfo"></a>
Special object for containing Customer card's information, that we're received from PaytureCommands.GetList command

| Fields's name    | Field's type  | Definition                                                             |
| ---------------- | ------------- | ---------------------------------------------------------------------- |
| CardNumber       | string        | The masked card's number.                                              |
| CardId           | string        | Card identifier in Payture system.                                     |
| CardHolder       | string        | Name of card's holder                                                  | 
| ActiveStatus     | string        | Indicate of card's active status in Payture system                     |
| Expired          | bool          | Indicate whether the card expired on the current date                  |
| NoCVV            | bool          | Indicate whether or not payment without CVV/CVC2                       |

### Transaction <a id="Transaction"></a>
You don't needed to create object of this type by yoursef - it will be created for you then you access to appopriate API via Merchant object. 
This object contans the necessary fields which used in request construction process. And this is abstract type.



## Test application <a id="testApp"></a>
You can download simple test application - realized as console app - and test work of our API just type the command in command line. Full description of command for app available into app by the command help. And then the app starts - it ask you for necessity of assistance.


Visit our [site](http://payture.com/) for more information.
You can find our contact [here](http://payture.com/kontakty/).
