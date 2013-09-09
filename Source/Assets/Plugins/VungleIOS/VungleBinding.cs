using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;



#if UNITY_IPHONE
public class VungleBinding
{
	[DllImport("__Internal")]
	private static extern void _vungleStartWithAppId( string appId );

	// Starts up the SDK with the given appId
	public static void startWithAppId( string appId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleStartWithAppId( appId );
	}


	[DllImport("__Internal")]
	private static extern void _vungleSetSoundEnabled( bool enabled );

	// Enables/disables sound
	public static void setSoundEnabled( bool enabled )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleSetSoundEnabled( enabled );
	}


	[DllImport("__Internal")]
	private static extern void _vungleStartWithAppIdAndUserData( string appId, string userData );

	// Starts up the SDK with the given appId and userData. See VGUserData for allowed values.
	public static void startWithAppIdAndUserData( string appId, Dictionary<string,object> userData )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleStartWithAppIdAndUserData( appId, userData.toJson() );
	}


	[DllImport("__Internal")]
	private static extern void _vungleEnableLogging( bool shouldEnable );

	// Enables verbose logging
	public static void enableLogging( bool shouldEnable )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleEnableLogging( shouldEnable );
	}


	[DllImport("__Internal")]
	private static extern void _vungleSetCacheSize( int cacheSize );

	// Sets the maximum size in megabytes of the video cache
	public static void setCacheSize( int cacheSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleSetCacheSize( cacheSize );
	}


	[DllImport("__Internal")]
	private static extern bool _vungleIsAdAvailable();

	// Checks to see if a video ad is available
	public static bool isAdAvailable()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _vungleIsAdAvailable();
		return false;
	}


	[DllImport("__Internal")]
	private static extern void _vungleStop();

	// Shuts down the Vungle SDK
	public static void stop()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleStop();
	}


	[DllImport("__Internal")]
	private static extern void _vunglePlayModalAd( bool showCloseButton );

	// Plays a modal video ad optionally showing a close button
	public static void playModalAd( bool showCloseButton )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vunglePlayModalAd( showCloseButton );
	}


	[DllImport("__Internal")]
	private static extern void _vunglePlayInsentivisedAd( string user, bool showCloseButton );
	
	// Plays an insentivised video ad optionally showing a close button
	public static void playIncentivizedAd( string user, bool showCloseButton )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vunglePlayInsentivisedAd( user, showCloseButton );
	}


	[DllImport("__Internal")]
	private static extern void _vungleAllowAutoRotate( bool shouldAllow );

	// Sets Vungle to allow auto rotation or not
	public static void allowAutoRotate( bool shouldAllow )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_vungleAllowAutoRotate( shouldAllow );
	}

}
#endif
