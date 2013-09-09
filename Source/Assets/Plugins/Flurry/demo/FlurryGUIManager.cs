using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class FlurryGUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE

	void OnGUI()
	{
		beginColumn();

		if( GUILayout.Button( "Start Flurry Session" ) )
		{
			// Optional information
			FlurryBinding.setAge( 12 );
			FlurryBinding.setGender( "M" );
			
			// replace with your Flurry Key!!!
			FlurryBinding.startSession( "XJHB5EGMQ9NCC6XWH43W" );
		}


		if( GUILayout.Button( "Log Event" ) )
		{
			FlurryBinding.logEvent( "Stuff Happened", false );
		}


		if( GUILayout.Button( "Log Event with Params" ) )
		{
			var dict = new Dictionary<string,string>();
			dict.Add( "akey1", "value1" );
			dict.Add( "bkey2", "value2" );
			dict.Add( "ckey3", "value3" );
			dict.Add( "dkey4", "value4" );

			FlurryBinding.logEventWithParameters( "EventWithParams", dict, false );
		}


		if( GUILayout.Button( "Log Timed Event" ) )
		{
			FlurryBinding.logEvent( "Timed Event", true );
		}


		if( GUILayout.Button( "End Timed Event" ) )
		{
			FlurryBinding.endTimedEvent( "Timed Event" );
		}


		if( GUILayout.Button( "Set Reports on Close" ) )
		{
			FlurryBinding.setSessionReportsOnCloseEnabled( true );
		}


		if( GUILayout.Button( "Set Reports on Pause" ) )
		{
			FlurryBinding.setSessionReportsOnPauseEnabled( true );
		}


		endColumn( true );


		if( GUILayout.Button( "Enable Ads" ) )
		{
			FlurryBinding.enableAds( true );
		}


		if( GUILayout.Button( "Fetch Ads" ) )
		{
			FlurryBinding.fetchAdForSpace( "adSpace", FlurryAdSize.Bottom );
			FlurryBinding.fetchAdForSpace( "splash", FlurryAdSize.Fullscreen );
		}


		if( GUILayout.Button( "Check if Ad Available" ) )
		{
			var isAvailable = FlurryBinding.isAdAvailableForSpace( "adSpace", FlurryAdSize.Bottom );
			Debug.Log( "is ad available: " + isAvailable );
		}


		if( GUILayout.Button( "Show Ad on Bottom" ) )
		{
			FlurryBinding.displayAdForSpace( "adSpace", FlurryAdSize.Bottom );
		}


		if( GUILayout.Button( "Fetch and Show Ad" ) )
		{
			FlurryBinding.fetchAndDisplayAdForSpace( "adSpace", FlurryAdSize.Top );
		}


		if( GUILayout.Button( "Show Full Screen Ad" ) )
		{
			FlurryBinding.fetchAndDisplayAdForSpace( "splash", FlurryAdSize.Fullscreen );
		}


		if( GUILayout.Button( "Remove Ad" ) )
		{
			FlurryBinding.removeAdFromSpace( "adSpace" );
		}

		endColumn();
	}

#endif
}
