using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class FlurryAndroidManager : AbstractManager
{
	// Fired when an ad is available for a space
	public static event Action<string> adAvailableForSpaceEvent;

	// Fired when an ad is not available for a space
	public static event Action<string> adNotAvailableForSpaceEvent;

	// Fired when an ad is closed
	public static event Action<string> onAdClosedEvent;

	// Fired when an ad touch exits the application
	public static event Action<string> onApplicationExitEvent;

	// Fired when an ad render fails
	public static event Action<string> onRenderFailedEvent;

	// Fired when an ad space fails to receive an ad
	public static event Action<string> spaceDidFailToReceiveAdEvent;

	// Fired when an ad space receives an ad
	public static event Action<string> spaceDidReceiveAdEvent;
	
	// Fired when an ad is clicked
	public static event Action<string> onAdClickedEvent;
	
	// Fired when an ad is opened
	public static event Action<string> onAdOpenedEvent;
	
	// Fired when a video completes
	public static event Action<string> onVideoCompletedEvent;
	
	// Fired when the currency value updates. This can occur when you explicity increment/decrement the value or if the user is rewarded
	public static event Action<string,float> onCurrencyValueUpdatedEvent;

	// Fired when the currency value fails to update
	public static event Action<string> onCurrencyValueFailedToUpdateEvent;

	
	
	static FlurryAndroidManager()
	{
		AbstractManager.initialize( typeof( FlurryAndroidManager ) );
	}


	public void adAvailableForSpace( string adSpace )
	{
		if( adAvailableForSpaceEvent != null )
			adAvailableForSpaceEvent( adSpace );
	}


	public void adNotAvailableForSpace( string adSpace )
	{
		if( adNotAvailableForSpaceEvent != null )
			adNotAvailableForSpaceEvent( adSpace );
	}


	public void onAdClosed( string adSpace )
	{
		if( onAdClosedEvent != null )
			onAdClosedEvent( adSpace );
	}


	public void onApplicationExit( string adSpace )
	{
		if( onApplicationExitEvent != null )
			onApplicationExitEvent( adSpace );
	}


	public void onRenderFailed( string adSpace )
	{
		if( onRenderFailedEvent != null )
			onRenderFailedEvent( adSpace );
	}


	public void spaceDidFailToReceiveAd( string adSpace )
	{
		if( spaceDidFailToReceiveAdEvent != null )
			spaceDidFailToReceiveAdEvent( adSpace );
	}


	public void spaceDidReceiveAd( string adSpace )
	{
		if( spaceDidReceiveAdEvent != null )
			spaceDidReceiveAdEvent( adSpace );
	}


	public void onAdClicked( string adSpace )
	{
		onAdClickedEvent.fire( adSpace );
	}


	public void onAdOpened( string adSpace )
	{
		onAdOpenedEvent.fire( adSpace );
	}


	public void onVideoCompleted( string adSpace )
	{
		onVideoCompletedEvent.fire( adSpace );
	}
	
	
	public void onCurrencyValueFailedToUpdate( string error )
	{
		onCurrencyValueFailedToUpdateEvent.fire( error );
	}


	public void onCurrencyValueUpdated( string response )
	{
		if( onCurrencyValueUpdatedEvent != null )
		{
			var parts = response.Split( new char[] { ',' } );
			if( parts.Length == 2 )
				onCurrencyValueUpdatedEvent( parts[0], float.Parse( parts[1] ) );
		}
	}

}

