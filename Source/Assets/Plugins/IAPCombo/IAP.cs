using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID
public static class IAP
{
	private const string CONSUMABLE_PAYLOAD = "consume";
	private const string NON_CONSUMABLE_PAYLOAD = "nonconsume";

	private static Action<List<IAPProduct>> _productListReceivedAction;
	private static Action<bool> _purchaseCompletionAction;

	#pragma warning disable
	private static Action<string> _purchaseRestorationAction;
	#pragma warning restore

	static IAP()
	{
#if UNITY_IPHONE
		// product list
		StoreKitManager.productListReceivedEvent += ( products ) =>
		{
			var convertedProducts = new List<IAPProduct>();
			foreach( var p in products )
				convertedProducts.Add( new IAPProduct( p ) );

			if( _productListReceivedAction != null )
				_productListReceivedAction( convertedProducts );
		};
		StoreKitManager.productListRequestFailedEvent += ( error ) =>
		{
			Debug.Log( "fetching prouduct data failed: " + error );
			if( _productListReceivedAction != null )
				_productListReceivedAction( null );
		};

		// purchases
		StoreKitManager.purchaseSuccessfulEvent += ( transaction ) =>
		{
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( true );

			if( _purchaseRestorationAction != null )
				_purchaseRestorationAction( transaction.productIdentifier );
		};
		StoreKitManager.purchaseFailedEvent += ( error ) =>
		{
			Debug.Log( "purchase failed: " + error );
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( false );
		};
		StoreKitManager.purchaseCancelledEvent += ( error ) =>
		{
			Debug.Log( "purchase cancelled: " + error );
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( false );
		};

#elif UNITY_ANDROID
		// inventory
		GoogleIABManager.queryInventorySucceededEvent += ( purchases, skus ) =>
		{
			var convertedProducts = new List<IAPProduct>();
			foreach( var p in skus )
				convertedProducts.Add( new IAPProduct( p ) );

			if( _productListReceivedAction != null )
				_productListReceivedAction( convertedProducts );
		};
		GoogleIABManager.queryInventoryFailedEvent += ( error ) =>
		{
			Debug.Log( "fetching prouduct data failed: " + error );
			if( _productListReceivedAction != null )
				_productListReceivedAction( null );
		};

		// purchases
		GoogleIABManager.purchaseSucceededEvent += ( purchase ) =>
		{
			if( purchase.developerPayload == NON_CONSUMABLE_PAYLOAD )
			{
				if( _purchaseCompletionAction != null )
					_purchaseCompletionAction( true );
			}
			else
			{
				// we need to consume this one
				GoogleIAB.consumeProduct( purchase.productId );
			}
		};
		GoogleIABManager.purchaseFailedEvent += ( error ) =>
		{
			Debug.Log( "purchase failed: " + error );
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( false );
		};

		// consumption
		GoogleIABManager.consumePurchaseSucceededEvent += ( purchase ) =>
		{
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( true );
		};
		GoogleIABManager.consumePurchaseFailedEvent += ( error ) =>
		{
			if( _purchaseCompletionAction != null )
				_purchaseCompletionAction( false );
		};
#endif
	}


	// Initializes the billing system. Call this at app launch to prepare the IAP system.
	public static void init( string androidPublicKey )
	{
#if UNITY_ANDROID
		GoogleIAB.init( androidPublicKey );
#endif
	}


	// Accepts two arrays of product identifiers (one for iOS one for Android). All of the products you have for sale should be requested in one call.
	public static void requestProductData( string[] iosProductIdentifiers, string[] androidSkus, Action<List<IAPProduct>> completionHandler )
	{
		_productListReceivedAction = completionHandler;

#if UNITY_ANDROID
		GoogleIAB.queryInventory( androidSkus );
#elif UNITY_IPHONE
		StoreKitBinding.requestProductData( iosProductIdentifiers );
#endif
	}


	// Purchases the given product and quantity. completionHandler provides if the purchase succeeded
	public static void purchaseConsumableProduct( string productId, Action<bool> completionHandler )
	{
		_purchaseCompletionAction = completionHandler;

#if UNITY_ANDROID
		GoogleIAB.purchaseProduct( productId, CONSUMABLE_PAYLOAD );
#elif UNITY_IPHONE
		StoreKitBinding.purchaseProduct( productId, 1 );
#endif
	}


	// Purchases the given product and quantity. completionHandler provides if the purchase succeeded
	public static void purchaseNonconsumableProduct( string productId, Action<bool> completionHandler )
	{
		_purchaseCompletionAction = completionHandler;

#if UNITY_ANDROID
		GoogleIAB.purchaseProduct( productId, NON_CONSUMABLE_PAYLOAD );
#elif UNITY_IPHONE
		StoreKitBinding.purchaseProduct( productId, 1 );
#endif
	}


	// Restores all previous transactions. This is used when a user gets a new device and they need to restore their old purchases.
	// DO NOT call this on every launch. It will prompt the user for their password. Each transaction that is restored will have
	// the completion handler called for it
	public static void restoreCompletedTransactions( Action<string> completionHandler )
	{
		_purchaseCompletionAction = null;
		_purchaseRestorationAction = completionHandler;

#if UNITY_IPHONE
		StoreKitBinding.restoreCompletedTransactions();
#endif
	}

}
#endif