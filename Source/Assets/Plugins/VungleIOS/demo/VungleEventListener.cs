using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class VungleEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		VungleManager.vungleMoviePlayedEvent += vungleMoviePlayedEvent;
		VungleManager.vungleStatusUpdateEvent += vungleStatusUpdateEvent;
		VungleManager.vungleViewDidDisappearEvent += vungleViewDidDisappearEvent;
		VungleManager.vungleViewWillAppearEvent += vungleViewWillAppearEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		VungleManager.vungleMoviePlayedEvent -= vungleMoviePlayedEvent;
		VungleManager.vungleStatusUpdateEvent -= vungleStatusUpdateEvent;
		VungleManager.vungleViewDidDisappearEvent -= vungleViewDidDisappearEvent;
		VungleManager.vungleViewWillAppearEvent -= vungleViewWillAppearEvent;
	}



	void vungleMoviePlayedEvent( Dictionary<string,object> data )
	{
		Debug.Log( "vungleMoviePlayedEvent" );
		Prime31.Utils.logObject( data );
	}


	void vungleStatusUpdateEvent( Dictionary<string,object> data )
	{
		Debug.Log( "vungleStatusUpdateEvent" );
		Prime31.Utils.logObject( data );
	}


	void vungleViewDidDisappearEvent()
	{
		Debug.Log( "vungleViewDidDisappearEvent" );
	}


	void vungleViewWillAppearEvent()
	{
		Debug.Log( "vungleViewWillAppearEvent" );
	}

#endif
}


