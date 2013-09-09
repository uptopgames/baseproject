using UnityEngine;
using System;
using System.Collections;
using Prime31;


#if UNITY_IPHONE
public class AdManager : AbstractManager
{
	// Fired when the adView is either shown or hidden
	public static event Action<bool> adViewDidChange;
	
	// Fired when an interstial ad fails to load or show
	public static event Action<string> interstitalAdFailed;
	
	// Fired when an interstitial ad is loaded and ready to show
	public static event Action interstitialAdLoaded;
	
	
	public static bool adViewIsShowing = false;
	
	
    static AdManager()
    {
		AbstractManager.initialize( typeof( AdManager ) );
    }


	public void adViewDidShow( string returnValue )
	{
		adViewIsShowing = ( returnValue == "1" );
		adViewDidChange.fire( adViewIsShowing );
	}
	
	
	public void interstitialFailed( string error )
	{
		interstitalAdFailed.fire( error );
	}
	
	
	public void interstitialLoaded( string empty )
	{
		interstitialAdLoaded.fire();
	}

}
#endif