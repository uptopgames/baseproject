using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class VungleComboUI : MonoBehaviourGUI
{
#if UNITY_IPHONE || UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();


		if( GUILayout.Button( "Start" ) )
		{
			Vungle.init( "com.prime31.Vungle", "vungleTest" );
			//Vungle.init( "ANDROID_APP_ID", "IOS_APP_ID" );
		}


		if( GUILayout.Button( "Is Ad Available" ) )
		{
			Debug.Log( "is ad available: " + Vungle.isAdvertAvailable() );
		}


		if( GUILayout.Button( "Display Ad" ) )
		{
			Vungle.displayAdvert( true );
		}


		if( GUILayout.Button( "Display Insentivized Ad" ) )
		{
			Vungle.displayIncentivizedAdvert( true, "user-tag" );
		}

		endColumn();
	}


	#region Optional: Example of Subscribing to All Events

	void OnEnable()
	{
		Vungle.onAdStartedEvent += onAdStartedEvent;
		Vungle.onAdEndedEvent += onAdEndedEvent;
		Vungle.onAdViewedEvent += onAdViewedEvent;
	}


	void OnDisable()
	{
		Vungle.onAdStartedEvent -= onAdStartedEvent;
		Vungle.onAdEndedEvent -= onAdEndedEvent;
		Vungle.onAdViewedEvent -= onAdViewedEvent;
	}


	void onAdStartedEvent()
	{
		Debug.Log( "onAdStartedEvent" );
	}


	void onAdEndedEvent()
	{
		Debug.Log( "onAdEndedEvent" );
	}


	void onAdViewedEvent( double watched, double length )
	{
		Debug.Log( "onAdViewedEvent. watched: " + watched + ", length: " + length );
	}

	#endregion

#endif
}
