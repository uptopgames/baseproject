using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class EtceteraUIManagerTwo : MonoBehaviourGUI
{
#if UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();
	

		if( GUILayout.Button( "Show Inline Web View" ) )
		{
			EtceteraAndroid.inlineWebViewShow( "http://prime31.com/", 160, 430, Screen.width - 160, Screen.height - 100 );
		}
	
	
		if( GUILayout.Button( "Close Inline Web View" ) )
		{
			EtceteraAndroid.inlineWebViewClose();
		}
	
	
		if( GUILayout.Button( "Set Url of Inline Web View" ) )
		{
			EtceteraAndroid.inlineWebViewSetUrl( "http://google.com" );
		}
	
	
		if( GUILayout.Button( "Set Frame of Inline Web View" ) )
		{
			EtceteraAndroid.inlineWebViewSetFrame( 80, 50, 300, 400 );
		}
		
		
		endColumn( true );
		
		
		if( GUILayout.Button( "Schedule Notification in 5 Seconds" ) )
		{
			EtceteraAndroid.scheduleNotification( 5, "Notiifcation Title", "The subtitle of the notification", "Ticker text gets ticked", "my-special-data" );
		}
		
		
		if( GUILayout.Button( "Schedule Notification in 10 Seconds" ) )
		{
			EtceteraAndroid.scheduleNotification( 10, "Notiifcation Title", "The subtitle of the notification", "Ticker text gets ticked", "my-special-data" );
		}
		
		
		if( GUILayout.Button( "Check for Noitifications" ) )
		{
			EtceteraAndroid.checkForNotifications();
		}
		
		
		if( GUILayout.Button( "Quit App" ) )
		{
			Application.Quit();
		}
		
		
		endColumn();
		

		if( bottomRightButton( "Previous Scene" ) )
		{
			Application.LoadLevel( "EtceteraTestScene" );
		}
	}

#endif
}
