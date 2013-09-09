using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class GUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE
	bool isPad;
	
	void Start()
	{
		// hack to detect iPad 3 until Unity adds official support
		this.isPad = ( Screen.width >= 1024 || Screen.height >= 1024 );
		
		if( isPad )
		{
			// listen to interstital events for illustration purposes
			AdManager.interstitalAdFailed += delegate( string error )
			{
				Debug.Log( "interstitalAdFailed: " + error );
			};
			
			AdManager.interstitialAdLoaded += delegate()
			{
				Debug.Log( "interstitialAdLoaded" );
			};
		}
	}
	
	
	void OnGUI()
	{
		beginColumn();

		
		if( GUILayout.Button( "Create Ad Banner" ) )
		{
			AdBinding.createAdBanner( true );
		}
		
		
		if( GUILayout.Button( "Destroy Ad Banner" ) )
		{
			AdBinding.destroyAdBanner();
		}

		
		if( isPad )
		{
			if( GUILayout.Button( "Initialize Interstitial" ) )
			{
				bool result = AdBinding.initializeInterstitial();
				Debug.Log( "initializeInterstitial: " + result );
			}
			
			
			if( GUILayout.Button( "Is Interstitial Loaded?" ) )
			{
				bool result = AdBinding.isInterstitalLoaded();
				Debug.Log( "isInterstitalLoaded: " + result );
			}
			
			
			if( GUILayout.Button( "Show Interstitial" ) )
			{
				bool result = AdBinding.showInterstitial();
				Debug.Log( "showInterstitial: " + result );
			}
		}
		
		endColumn();
	}
#endif
}
