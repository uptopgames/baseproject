using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class FlurryManager : AbstractManager
{
	// Fired when an ad space dismisses the ad
	public static event Action<string> spaceDidDismissEvent;

	// Fired when an ad touch will leave the app
	public static event Action<string> spaceWillLeaveApplicationEvent;

	// Fired when an ad space fails to render
	public static event Action<string> spaceDidFailToRenderEvent;

	// Fired when an ad space fails to download an ad
	public static event Action<string> spaceDidFailToReceiveAdEvent;

	// Fired when an ad space receives an ad
	public static event Action<string> spaceDidReceiveAdEvent;

	// Fired when the currency value updates. This can occur when you explicity increment/decrement the value or if the user is rewarded
	public static event Action<string,float> onCurrencyValueUpdatedEvent;

	// Fired when the currency value fails to update
	public static event Action<P31Error> onCurrencyValueFailedToUpdateEvent;

	// Fired when a video completes
	public static event Action<string> videoDidFinishEvent;



	static FlurryManager()
	{
		AbstractManager.initialize( typeof( FlurryManager ) );
	}


	void spaceDidDismiss( string space )
	{
		if( spaceDidDismissEvent != null )
			spaceDidDismissEvent( space );
	}


	void spaceWillLeaveApplication( string space )
	{
		if( spaceWillLeaveApplicationEvent != null )
			spaceWillLeaveApplicationEvent( space );
	}


	void spaceDidFailToRender( string space )
	{
		if( spaceDidFailToRenderEvent != null )
			spaceDidFailToRenderEvent( space );
	}


	void spaceDidFailToReceiveAd( string space )
	{
		if( spaceDidFailToReceiveAdEvent != null )
			spaceDidFailToReceiveAdEvent( space );
	}


	void spaceDidReceiveAd( string space )
	{
		if( spaceDidReceiveAdEvent != null )
			spaceDidReceiveAdEvent( space );
	}


	void onCurrencyValueFailedToUpdate( string json )
	{
		onCurrencyValueFailedToUpdateEvent.fire( P31Error.errorFromJson( json ) );
	}


	void onCurrencyValueUpdated( string response )
	{
		if( onCurrencyValueUpdatedEvent != null )
		{
			var parts = response.Split( new char[] { ',' } );
			if( parts.Length == 2 )
				onCurrencyValueUpdatedEvent( parts[0], float.Parse( parts[1] ) );
		}
	}


	void videoDidFinish( string space )
	{
		if( videoDidFinishEvent != null )
			videoDidFinishEvent( space );
	}

}