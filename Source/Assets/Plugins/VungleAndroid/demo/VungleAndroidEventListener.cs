using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




public class VungleAndroidEventListener : MonoBehaviour
{
#if UNITY_ANDROID
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		VungleAndroidManager.onVungleAdStartEvent += onVungleAdStartEvent;
		VungleAndroidManager.onVungleAdEndEvent += onVungleAdEndEvent;
		VungleAndroidManager.onVungleViewEvent += onVungleViewEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		VungleAndroidManager.onVungleAdStartEvent -= onVungleAdStartEvent;
		VungleAndroidManager.onVungleAdEndEvent -= onVungleAdEndEvent;
		VungleAndroidManager.onVungleViewEvent -= onVungleViewEvent;
	}



	void onVungleAdStartEvent()
	{
		Debug.Log( "onVungleAdStartEvent" );
	}


	void onVungleAdEndEvent()
	{
		Debug.Log( "onVungleAdEndEvent" );
	}


	void onVungleViewEvent( double watched, double length )
	{
		Debug.Log( "onVungleViewEvent. watched: " + watched + ", length: " + length );
	}

#endif
}
