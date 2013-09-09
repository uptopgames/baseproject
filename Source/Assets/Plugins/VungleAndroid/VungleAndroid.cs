using UnityEngine;
using System.Collections;



public enum VungleGender
{
	None = -1,
	Male = 0,
	Female
}


#if UNITY_ANDROID
public class VungleAndroid
{
	private static AndroidJavaObject _plugin;

	static VungleAndroid()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		// find the plugin instance
		using( var pluginClass = new AndroidJavaClass( "com.prime31.VunglePlugin" ) )
			_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
	}


	// Initializes the Vungle SDK optionally with an age and gender
	public static void init( string appId )
	{
		init( appId, -1, VungleGender.None );
	}

	public static void init( string appId, int age, VungleGender gender )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "init", appId, age, (int)gender );
	}


	// Call this when your application is sent to the background
	public static void onPause()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "onPause" );
	}


	// Call this when your application resumes
	public static void onResume()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "onResume" );
	}


	// Checks to see if a video is available
	public static bool isVideoAvailable()
	{
		if( Application.platform != RuntimePlatform.Android )
			return false;

		return _plugin.Call<bool>( "isVideoAvailable" );
	}


	// Sets if sound should be enabled or not
	public static void setSoundEnabled( bool isEnabled )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "setSoundEnabled", isEnabled );
	}


	// Sets if adverts should be alowed to auto rotate
	public static void setAutoRotation( bool shouldAutorotate )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "setAutoRotation", shouldAutorotate );
	}


	// Checks to see if sound is enabled
	public static bool isSoundEnabled()
	{
		if( Application.platform != RuntimePlatform.Android )
			return true;

		return _plugin.Call<bool>( "isSoundEnabled" );
	}


	// Displays an advert
	public static void displayAdvert()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "displayAdvert" );
	}


	// Displays an incentivized advert with optional name
	public static void displayIncentivizedAdvert( bool showCloseButton )
	{
		displayIncentivizedAdvert( showCloseButton, string.Empty );
	}

	public static void displayIncentivizedAdvert( bool showCloseButton, string name )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "displayIncentivizedAdvert", showCloseButton, name );
	}

}
#endif

