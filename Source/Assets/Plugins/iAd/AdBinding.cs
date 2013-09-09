using UnityEngine;
using System.Runtime.InteropServices;


public static class AdBinding
{
    [DllImport("__Internal")]
    private static extern void _iAdCreateAdBanner( bool bannerOnBottom );

	// Starts up iAd requests and ads the ad view
    public static void createAdBanner( bool bannerOnBottom )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_iAdCreateAdBanner( bannerOnBottom );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _iAdDestroyAdBanner();

	// Destroys the ad banner and removes it from view
    public static void destroyAdBanner()
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_iAdDestroyAdBanner();
    }	


    [DllImport("__Internal")]
    private static extern void _iAdFireHideShowEvents( bool shouldFire );

	// Switches the orientation of the ad view
    public static void fireHideShowEvents( bool shouldFire )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_iAdFireHideShowEvents( shouldFire );
    }


	[DllImport("__Internal")]
	private static extern bool _iAdInitializeInterstitial();

	// Starts loading a new interstitial ad.  Returns false when interstitials are not supported.
	public static bool initializeInterstitial()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _iAdInitializeInterstitial();

		return false;
	}


	[DllImport("__Internal")]
	private static extern bool _iAdInterstitialIsLoaded();

	// Checks to see if an interstitial ad is loaded.
	public static bool isInterstitalLoaded()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _iAdInterstitialIsLoaded();

		return false;
	}


	[DllImport("__Internal")]
	private static extern bool _iAdShowInterstitial();

	// Shows an interstitial ad.  Will return false if it isn't loaded.
	public static bool showInterstitial()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _iAdShowInterstitial();

		return false;
	}
	
}
