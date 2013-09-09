using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;




#if UNITY_IPHONE
public enum FlurryAdSize
{
	Top = 1,
	Bottom = 2,
	Fullscreen = 3
}


public class FlurryBinding
{
	[DllImport("__Internal")]
	private static extern void _flurryStartSession( string apiKey );

	// Starts up your Flurry analytics session.  Call on application startup.
	public static void startSession( string apiKey )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryStartSession( apiKey );
	}
	
	
	#region Flurry Analytics

	[DllImport("__Internal")]
	private static extern void _flurryLogEvent( string eventName, bool isTimed );

	// Logs an event to Flurry.  If isTimed is true, this will be a timed event and it should be paired with a call to endTimedEvent
	public static void logEvent( string eventName, bool isTimed )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryLogEvent( eventName, isTimed );
	}


	[DllImport("__Internal")]
	private static extern void _flurryLogEventWithParameters( string eventName, string parameters, bool isTimed );

	// Logs an event with optional key/value pairs
	public static void logEventWithParameters( string eventName, Dictionary<string,string> parameters, bool isTimed )
	{
		if( parameters == null )
			parameters = new Dictionary<string, string>();
		
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryLogEventWithParameters( eventName, parameters.toJson(), isTimed );
	}


	[DllImport("__Internal")]
	private static extern void _flurryEndTimedEvent( string eventName, string parameters );

	// Ends a timed event that was previously started
	public static void endTimedEvent( string eventName )
	{
		endTimedEvent( eventName, new Dictionary<string,string>() );
	}

	public static void endTimedEvent( string eventName, Dictionary<string,string> parameters )
	{
		if( parameters == null )
			parameters = new Dictionary<string, string>();

		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryEndTimedEvent( eventName, parameters.toJson() );
	}


	[DllImport("__Internal")]
	private static extern void _flurrySetAge( int age );

	// Sets the users aga
	public static void setAge( int age )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurrySetAge( age );
	}


	[DllImport("__Internal")]
	private static extern void _flurrySetGender( string gender );

	// Sets the users gender
	public static void setGender( string gender )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurrySetGender( gender );
	}


	[DllImport("__Internal")]
	private static extern void _flurrySetUserId( string userId );

	// Sets the users unique id
	public static void setUserId( string userId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurrySetUserId( userId );
	}


	[DllImport("__Internal")]
	private static extern void _flurrySetSessionReportsOnCloseEnabled( bool sendSessionReportsOnClose );

	// Sets whether Flurry should upload session reports when your app closes
	public static void setSessionReportsOnCloseEnabled( bool sendSessionReportsOnClose )
	
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurrySetSessionReportsOnCloseEnabled( sendSessionReportsOnClose );
	}


	[DllImport("__Internal")]
	private static extern void _flurrySetSessionReportsOnPauseEnabled( bool setSessionReportsOnPauseEnabled );

	// Sets whether Flurry should upload session reports when your app is paused
	public static void setSessionReportsOnPauseEnabled( bool setSessionReportsOnPauseEnabled )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurrySetSessionReportsOnPauseEnabled( setSessionReportsOnPauseEnabled );
	}
	
	#endregion
	
	
	#region Flurry Ads

	[DllImport("__Internal")]
	private static extern void _flurryAdsInitialize( bool enableTestAds );

	// Initializes the Flurry ad system optioning with test ads
	public static void enableAds( bool enableTestAds )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryAdsInitialize( enableTestAds );
	}


	[DllImport("__Internal")]
	private static extern void _flurryAdsSetUserCookies( string cookies );

	// Sets user cookies for the ad session
	public static void adsSetUserCookies( Dictionary<string,string> cookies )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryAdsSetUserCookies( cookies.toJson() );
	}


	[DllImport("__Internal")]
	private static extern void _flurryAdsClearUserCookies();

	// Clears the user cookies
	public static void adsClearUserCookies()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryAdsClearUserCookies();
	}


	[DllImport("__Internal")]
	private static extern void _flurryAdsSetKeywords( string keywords );

	// Sets keywords for the ad session
	public static void adsSetKeywords( string keywords )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryAdsSetKeywords( keywords );
	}


	[DllImport("__Internal")]
	private static extern void _flurryAdsClearKeywords();

	// Clears keywords
	public static void adsClearKeywords()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryAdsClearKeywords();
	}
	
	
	[DllImport("__Internal")]
	private static extern bool _flurryFetchAdForSpace( string space, int adSize );

	// Fetches an ad
	public static void fetchAdForSpace( string space, FlurryAdSize adSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryFetchAdForSpace( space, (int)adSize );
	}

	
	[DllImport("__Internal")]
	private static extern bool _flurryIsAdAvailableForSpace( string space, int adSize );

	// Checks to see if an ad is available
	public static bool isAdAvailableForSpace( string space, FlurryAdSize adSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _flurryIsAdAvailableForSpace( space, (int)adSize );
		return false;
	}

	
	[DllImport("__Internal")]
	private static extern bool _flurryFetchAndDisplayAdForSpace( string space, int adSize );

	// Fetches then shows an ad
	public static void fetchAndDisplayAdForSpace( string space, FlurryAdSize adSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryFetchAndDisplayAdForSpace( space, (int)adSize );
	}
	
	
	[DllImport("__Internal")]
	private static extern bool _flurryDisplayAdForSpace( string space, int adSize );

	// Shows an ad
	public static void displayAdForSpace( string space, FlurryAdSize adSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryDisplayAdForSpace( space, (int)adSize );
	}


	[DllImport("__Internal")]
	private static extern void _flurryRemoveAdFromSpace( string space );

	// Removes an ad
	public static void removeAdFromSpace( string space )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_flurryRemoveAdFromSpace( space );
	}

	#endregion
	
	
	#region Flurry Wallet
	
	[DllImport("__Internal")]
	private static extern void _flurryAddObserverForCurrency( string currency );
	
	// Adds an observer that will notify via the onCurrencyValueUpdatedEvent/onCurrencyValueFailedToUpdateEvent when it changes
	public static void addObserverForCurrency( string currency )
	{
		if( Application.platform != RuntimePlatform.IPhonePlayer )
			return;

		_flurryAddObserverForCurrency( currency );
	}

	
	[DllImport("__Internal")]
	private static extern void _flurryDecrementCurrency( string currency, float amount );

	// Decrements the currency
	public static void decrementCurrency( string currency, float amount )
	{
		if( Application.platform != RuntimePlatform.IPhonePlayer )
			return;

		_flurryDecrementCurrency( currency, amount );
	}
	
	
	[DllImport("__Internal")]
	private static extern float _flurryGetLastReceivedCurrencyAmount( string currency );

	// Gets the current curreny amount as a float
	public static float getLastReceivedCurrencyAmount( string currency )
	{
		if( Application.platform != RuntimePlatform.IPhonePlayer )
			return 0f;

		return _flurryGetLastReceivedCurrencyAmount( currency );
	}
	
	#endregion
	
}
#endif