using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#if UNITY_IPHONE || UNITY_ANDROID
public class Vungle
{
	#region Events
	
	// Fired when a Vungle ad starts
	public static event Action onAdStartedEvent;
	
	// Fired when a Vungle ad finishes
	public static event Action onAdEndedEvent;
	
	// Fired when a Vungle video is dismissed and provides the time watched and total duration in that order.
	public static event Action<double,double> onAdViewedEvent;
	
	
	
	static void adStarted()
	{
		onAdStartedEvent.fire();
	}
	
	
	static void adFinished()
	{
		onAdEndedEvent.fire();
	}
	
	
	static void videoViewed( double timeWatched, double totalDuration )
	{
		onAdViewedEvent.fire( timeWatched, totalDuration );
	}
	
	
	static void vungleMoviePlayedEvent( Dictionary<string,object> data )
	{
		var timeWatched = double.Parse( data["videoViewed"].ToString() );
		var totalDuration = double.Parse( data["videoLength"].ToString() );
		
		onAdViewedEvent.fire( timeWatched, totalDuration );
	}
	
	#endregion
	
	
	static Vungle()
	{
#if UNITY_IPHONE
		VungleManager.vungleViewWillAppearEvent += adStarted;;
		VungleManager.vungleViewDidDisappearEvent += adFinished;
		VungleManager.vungleMoviePlayedEvent += vungleMoviePlayedEvent;
#elif UNITY_ANDROID
		VungleAndroidManager.onVungleAdStartEvent += adStarted;
		VungleAndroidManager.onVungleAdEndEvent += adFinished;
		VungleAndroidManager.onVungleViewEvent += videoViewed;
#endif
	}
	

	// Initializes the Vungle SDK optionally with an age and gender
	public static void init( string androidAppId, string iosAppId )
	{
		init( androidAppId, iosAppId, -1, VungleGender.None );
	}
	
	
	public static void init( string androidAppId, string iosAppId, int age, VungleGender gender )
	{
#if UNITY_IPHONE
		var userData = new Dictionary<string,object>();
		
		if( age > 0 )
		{
			userData["gender"] = (int)gender + 1;
			userData["age"] = age;
		}
		
		VungleBinding.startWithAppIdAndUserData( iosAppId, userData );
#elif UNITY_ANDROID
		if( age > 0 )
			VungleAndroid.init( androidAppId, age, gender );
		else
			VungleAndroid.init( androidAppId );
#endif
	}

	
	// Sets if sound should be enabled or not
	public static void setSoundEnabled( bool isEnabled )
	{
#if UNITY_IPHONE
		VungleBinding.setSoundEnabled( isEnabled );
#elif UNITY_ANDROID
		VungleAndroid.setSoundEnabled( isEnabled );
#endif
	}
	
	
	// Checks to see if a video is available
	public static bool isAdvertAvailable()
	{
#if UNITY_IPHONE
		return VungleBinding.isAdAvailable();
#elif UNITY_ANDROID
		return VungleAndroid.isVideoAvailable();
#else
		return false;
#endif
	}
	
	
	// Displays an advert
	public static void displayAdvert( bool showCloseButtonOnIOS )
	{
#if UNITY_IPHONE
		VungleBinding.playModalAd( showCloseButtonOnIOS );
#elif UNITY_ANDROID
		VungleAndroid.displayAdvert();
#endif
	}
	
	
	// Displays an incentivized advert with optional name
	public static void displayIncentivizedAdvert( bool showCloseButton, string user )
	{
#if UNITY_IPHONE
		VungleBinding.playIncentivizedAd( user, showCloseButton );
#elif UNITY_ANDROID
		VungleAndroid.displayIncentivizedAdvert( showCloseButton, user );
#endif
	}

}
#endif