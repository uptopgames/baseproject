using UnityEngine;
using System.Collections;
using Prime31;


public class EtceteraGUIManagerThree : MonoBehaviourGUI
{
#if UNITY_IPHONE
	void OnGUI()
	{
		beginColumn();
		
		
		if( GUILayout.Button( "Show Inline WebView" ) )
		{
			// remember, iOS uses points not pixels for positioning and layout!
			EtceteraBinding.inlineWebViewShow( 50, 10, 260, 300 );
			EtceteraBinding.inlineWebViewSetUrl( "http://google.com" );
		}
		
		
		if( GUILayout.Button( "Close Inline WebView" ) )
		{
			EtceteraBinding.inlineWebViewClose();
		}
		
		
		if( GUILayout.Button( "Set Url of Inline WebView" ) )
		{
			EtceteraBinding.inlineWebViewSetUrl( "http://prime31.com" );
		}
		
		
		if( GUILayout.Button( "Set Frame of Inline WebView" ) )
		{
			// remember, iOS uses points not pixels for positioning and layout!
			EtceteraBinding.inlineWebViewSetFrame( 10, 200, 250, 250 );
		}
		
		
		// Second row
		endColumn( true );
		
		
		if( GUILayout.Button( "Get Badge Count" ) )
		{
			Debug.Log( "badge count is: " + EtceteraBinding.getBadgeCount() );
		}
		
		
		if( GUILayout.Button( "Set Badge Count" ) )
		{
			EtceteraBinding.setBadgeCount( 46 );
		}
		
		
		if( GUILayout.Button( "Get Orientation" ) )
		{
			UIInterfaceOrientation orient = EtceteraBinding.getStatusBarOrientation();
			Debug.Log( "status bar orientation: " + orient.ToString() );
		}
		
		endColumn();
		
		
		// Next scene button
		if( bottomRightButton( "Back" ) )
		{
			Application.LoadLevel( "EtceteraTestScene" );
		}
	}
#endif
}
