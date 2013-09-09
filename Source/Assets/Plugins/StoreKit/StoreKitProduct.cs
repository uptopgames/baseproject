using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IPHONE
public class StoreKitProduct
{
    public string productIdentifier { get; private set; }
    public string title { get; private set; }
    public string description { get; private set; }
    public string price { get; private set; }
	public string currencySymbol { get; private set; }
	public string currencyCode { get; private set; }
	public string formattedPrice { get; private set; }


	public static List<StoreKitProduct> productsFromJson( string json )
	{
		var productList = new List<StoreKitProduct>();

		var products = json.listFromJson();
		foreach( Dictionary<string,object> ht in products )
			productList.Add( productFromDictionary( ht ) );

		return productList;
	}


    public static StoreKitProduct productFromDictionary( Dictionary<string,object> ht )
    {
        StoreKitProduct product = new StoreKitProduct();

		if( ht.ContainsKey( "productIdentifier" ) )
        	product.productIdentifier = ht["productIdentifier"].ToString();

		if( ht.ContainsKey( "localizedTitle" ) )
        	product.title = ht["localizedTitle"].ToString();

		if( ht.ContainsKey( "localizedDescription" ) )
        	product.description = ht["localizedDescription"].ToString();

		if( ht.ContainsKey( "price" ) )
        	product.price = ht["price"].ToString();

		if( ht.ContainsKey( "currencySymbol" ) )
			product.currencySymbol = ht["currencySymbol"].ToString();

		if( ht.ContainsKey( "currencyCode" ) )
			product.currencyCode = ht["currencyCode"].ToString();

		if( ht.ContainsKey( "formattedPrice" ) )
			product.formattedPrice = ht["formattedPrice"].ToString();

        return product;
    }


	public override string ToString()
	{
		return String.Format( "<StoreKitProduct>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nCurrency Symbol: {4}\nFormatted Price: {5}\nCurrency Code: {6}",
			productIdentifier, title, description, price, currencySymbol, formattedPrice, currencyCode );
	}

}
#endif
