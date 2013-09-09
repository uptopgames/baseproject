using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IPHONE
public class VungleManager : AbstractManager
{
	// Fired when a video has finished playing
	public static event Action<Dictionary<string,object>> vungleMoviePlayedEvent;

	// Fired when the Vungle SDK has a status update
	public static event Action<Dictionary<string,object>> vungleStatusUpdateEvent;

	// Fired when the video is dismissed
	public static event Action vungleViewDidDisappearEvent;

	// Fired when the video is shown
	public static event Action vungleViewWillAppearEvent;


	static VungleManager()
	{
		AbstractManager.initialize( typeof( VungleManager ) );
	}


	public void vungleMoviePlayed( string json )
	{
		if( vungleMoviePlayedEvent != null )
			vungleMoviePlayedEvent( json.dictionaryFromJson() );
	}


	public void vungleStatusUpdate( string json )
	{
		if( vungleStatusUpdateEvent != null )
			vungleStatusUpdateEvent( json.dictionaryFromJson() );
	}


	public void vungleViewDidDisappear( string empty )
	{
		if( vungleViewDidDisappearEvent != null )
			vungleViewDidDisappearEvent();
	}


	public void vungleViewWillAppear( string empty )
	{
		if( vungleViewWillAppearEvent != null )
			vungleViewWillAppearEvent();
	}
}
#endif

