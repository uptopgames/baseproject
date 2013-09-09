using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID
public class VungleAndroidManager : AbstractManager
{
	// Fired when a Vungle ad starts
	public static event Action onVungleAdStartEvent;

	// Fired when a Vungle ad finishes
	public static event Action onVungleAdEndEvent;

	// Fired when a Vungle video is dismissed
	public static event Action<double,double> onVungleViewEvent;


	static VungleAndroidManager()
	{
		AbstractManager.initialize( typeof( VungleAndroidManager ) );
	}


	public void onVungleAdStart( string empty )
	{
		if( onVungleAdStartEvent != null )
			onVungleAdStartEvent();
	}


	public void onVungleAdEnd( string empty )
	{
		if( onVungleAdEndEvent != null )
			onVungleAdEndEvent();
	}


	public void onVungleView( string str )
	{
		if( onVungleViewEvent != null )
		{
			var parts = str.Split( new char[] { '-' } );
			if( parts.Length == 2 )
				onVungleViewEvent( double.Parse( parts[0] ), double.Parse( parts[1] ) );
		}
	}
	
}
#endif
