using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class EtceteraGUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE
	void Start()
	{
		// example of setting the popover rect (only used on the iPad when showing the photo picker)
		EtceteraBinding.setPopoverPoint( 500, 200 );
	}
	

	void OnGUI()
	{
		beginColumn();
		

		if( GUILayout.Button( "Get Current Language" ) )
		{
			Debug.Log( "current launguage: " + EtceteraBinding.getCurrentLanguage() );
		}
		
		
		if( GUILayout.Button( "Get Locale Info for Keys" ) )
		{
			Debug.Log( "currency symbol: " + EtceteraBinding.localeObjectForKey( true, "kCFLocaleCurrencySymbolKey" ) );
			Debug.Log( "country code: " + EtceteraBinding.localeObjectForKey( true, "kCFLocaleCountryCodeKey" ) );
		}
		
		
		if( GUILayout.Button( "Get Localized String" ) )
		{
			string loc = EtceteraBinding.getLocalizedString( "hello", "hello in English" );
			Debug.Log( "'hello' localized: " + loc );
		}
		
		
		if( GUILayout.Button( "Alert with one Button" ) )
		{
			var buttons = new string[] { "OK" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons( "This is the title", "You should really read this before pressing OK", buttons );
		}


		if( GUILayout.Button( "Alert with three Buttons" ) )
		{
			var buttons = new string[] { "OK", "In The Middle", "Cancel" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons( "This is another title", "You should really read this before pressing a button", buttons );
		}
		

		if( GUILayout.Button( "Show Prompt with 1 Field" ) )
		{
			EtceteraBinding.showPromptWithOneField( "Enter your name", "This is the name of the main character", "name", false );
		}
		
		
		// Second row
		endColumn( true );
		
		
		if( GUILayout.Button( "Show Prompt with 2 Fields" ) )
		{
			EtceteraBinding.showPromptWithTwoFields( "Enter your credentials", "", "username", "password", false );
		}
		
		
		if( GUILayout.Button( "Open Web Page" ) )
		{
			// you can also use a local file that is in your .app bundle or elsewhere
			/*
			var path = Application.dataPath.Replace( "Data", "" );
			path = System.IO.Path.Combine( path, "file.html" );
			*/
			
			EtceteraBinding.showWebPage( "http://www.prime31.com", true );
		}
		
		
		if( GUILayout.Button( "Show Mail Composer" ) )
		{
			EtceteraBinding.showMailComposer( "support@somecompany.com", "Tell us what you think", "I <b>really</b> like this game!", true );
		}
		
		
		if( GUILayout.Button( "Show SMS Composer" ) )
		{
			// Make sure SMS is available before we try to show the composer
			if( EtceteraBinding.isSMSAvailable() )
				EtceteraBinding.showSMSComposer( "some text to prefill the message with" );
		}
		
		
		if( GUILayout.Button( "Mail Composer with Screenshot" ) )
		{
			// we call this as a coroutine so that it can use a couple frames to hande its business
			StartCoroutine( EtceteraBinding.showMailComposerWithScreenshot( null, "Game Screenshot", "I like this game!", false ) );
		}
		
		
		if( GUILayout.Button( "Take Screen Shot" ) )
		{
			StartCoroutine( EtceteraBinding.takeScreenShot( "someScreenshot.png", imagePath =>
			{
				Debug.Log( "Screenshot taken and saved to: " + imagePath );
			}) );
		}
		
		
		endColumn();
		
		
		if( bottomRightButton( "Next Scene" ) )
		{
			Application.LoadLevel( "EtceteraTestSceneTwo" );
		}
	}

#endif
}
