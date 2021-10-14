# MonopriceBuyBot

You will need to implement what items it should try and buy, an example is commented out in an attempt to stop you from buying something automatically without thinking about it:

//var itemsToBuy = new (string ID, bool Preferred)[]<br />
//{<br />
//    ("42116", true),<br />
//    ("37887", false)<br />
//};<br />

The bot is currently setup to buy a single item, changes would need to be made if trying to buy multiple items, the first string is the monoprice product id, the second bool determines what to buy if multiple items are put in the cart (removes all but preferred)

Arguments for the program are email, password, creditCardCCV, firstName
The bot uses your default credit card for payment
