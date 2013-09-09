using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class VungleGUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE
	void OnGUI()
	{
		beginColumn();


		if( GUILayout.Button( "Start" ) )
		{
			// replace with your app ID!!!
			VungleBinding.startWithAppId( "vungleTest" );
		}


		if( GUILayout.Button( "Start with Custom User Data" ) )
		{
			var userData = new Dictionary<string,object>();
			userData["gender"] = 1; // 0 unknown, 1 male, 2 female
			userData["adOrientation"] = 1; // 0 unknown, 1 portrait, 2 landscape
			userData["locationEnabled"] = true;
			userData["age"] = 21;
			VungleBinding.startWithAppIdAndUserData( "vungleTest", userData );
		}


		if( GUILayout.Button( "Is Ad Available" ) )
		{
			Debug.Log( "is ad available: " + VungleBinding.isAdAvailable() );
		}


		if( GUILayout.Button( "Play Modal Ad" ) )
		{
			VungleBinding.playModalAd( true );
		}


		endColumn( true );
		

		if( GUILayout.Button( "Set Cache Size" ) )
		{
			VungleBinding.setCacheSize( 12 );
		}


		if( GUILayout.Button( "Enable Logging" ) )
		{
			VungleBinding.enableLogging( true );
		}


		if( GUILayout.Button( "Play Insentivised Ad" ) )
		{
			VungleBinding.playIncentivizedAd( "user", true );
		}

		endColumn();
	}
#endif
}
