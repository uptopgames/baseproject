using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class VungleUIManager : MonoBehaviourGUI
{
#if UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();


		if( GUILayout.Button( "Init" ) )
		{
			// replace with your app ID!!!
			VungleAndroid.init( "com.prime31.Vungle" );
		}


		if( GUILayout.Button( "Is Video Available?" ) )
		{
			Debug.Log( "is video available? " + VungleAndroid.isVideoAvailable() );
		}


		if( GUILayout.Button( "Is Sound Enabled?" ) )
		{
			Debug.Log( "is sound enabled? " + VungleAndroid.isSoundEnabled() );
		}



		endColumn( true );

		if( GUILayout.Button( "Display Advert" ) )
		{
			VungleAndroid.displayAdvert();
		}


		if( GUILayout.Button( "Display Incentivized Advert" ) )
		{
			VungleAndroid.displayIncentivizedAdvert( true );
		}

		endColumn();
	}
#endif
}
