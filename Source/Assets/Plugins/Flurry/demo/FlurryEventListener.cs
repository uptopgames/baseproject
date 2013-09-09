using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class FlurryEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		FlurryManager.spaceDidDismissEvent += spaceDidDismissEvent;
		FlurryManager.spaceWillLeaveApplicationEvent += spaceWillLeaveApplicationEvent;
		FlurryManager.spaceDidFailToRenderEvent += spaceDidFailToRenderEvent;
		FlurryManager.spaceDidReceiveAdEvent += spaceDidReceiveAdEvent;
		FlurryManager.spaceDidFailToReceiveAdEvent += spaceDidFailToReceiveAdEvent;
		FlurryManager.onCurrencyValueUpdatedEvent += onCurrencyValueUpdatedEvent;
		FlurryManager.onCurrencyValueFailedToUpdateEvent += onCurrencyValueFailedToUpdateEvent;
		FlurryManager.videoDidFinishEvent += videoDidFinishEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		FlurryManager.spaceDidDismissEvent -= spaceDidDismissEvent;
		FlurryManager.spaceWillLeaveApplicationEvent -= spaceWillLeaveApplicationEvent;
		FlurryManager.spaceDidFailToRenderEvent -= spaceDidFailToRenderEvent;
		FlurryManager.spaceDidReceiveAdEvent -= spaceDidReceiveAdEvent;
		FlurryManager.spaceDidFailToReceiveAdEvent -= spaceDidFailToReceiveAdEvent;
		FlurryManager.onCurrencyValueUpdatedEvent -= onCurrencyValueUpdatedEvent;
		FlurryManager.onCurrencyValueFailedToUpdateEvent -= onCurrencyValueFailedToUpdateEvent;
		FlurryManager.videoDidFinishEvent -= videoDidFinishEvent;
	}



	void spaceDidDismissEvent( string space )
	{
		Debug.Log( "spaceDidDismissEvent: " + space );
	}


	void spaceWillLeaveApplicationEvent( string space )
	{
		Debug.Log( "spaceWillLeaveApplicationEvent: " + space );
	}


	void spaceDidFailToRenderEvent( string space )
	{
		Debug.Log( "spaceDidFailToRenderEvent: " + space );
	}


	void spaceDidReceiveAdEvent( string space )
	{
		Debug.Log( "spaceDidReceiveAdEvent: " + space );
	}


	void spaceDidFailToReceiveAdEvent( string space )
	{
		Debug.Log( "spaceDidFailToReceiveAdEvent: " + space );
	}


	void onCurrencyValueFailedToUpdateEvent( P31Error error )
	{
		Debug.LogError( "onCurrencyValueFailedToUpdateEvent: " + error );
	}


	void onCurrencyValueUpdatedEvent( string currency, float amount )
	{
		Debug.LogError( "onCurrencyValueUpdatedEvent. currency: " + currency + ", amount: " + amount );
	}


	void videoDidFinishEvent( string space )
	{
		Debug.Log( "videoDidFinishEvent: " + space );
	}

#endif
}


