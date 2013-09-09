using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Prime31;


public class EtceteraManager : AbstractManager
{
#if UNITY_IPHONE
	// Fired whenever any full screen view controller is dismissed
	public static event Action dismissingViewControllerEvent;
	
	// Fired when the user cancels the image picker
	public static event Action imagePickerCancelledEvent;
	
	// Fired when the user selects or takes a photo
	public static event Action<string> imagePickerChoseImageEvent;
	
	// Fired when an image is saved to the album
	public static event Action saveImageToPhotoAlbumSucceededEvent;
	
	// Fired when an image fails to be saved to the album
	public static event Action<string> saveImageToPhotoAlbumFailedEvent;
	
	// Delegate for handling loading a texture dynamically from the file system
	public delegate void EceteraTextureDelegate( Texture2D texture );
	
	// Delegate failure handler for loading a texture dynamically
	public delegate void EceteraTextureFailedDelegate( string error );
	
	// Fired when the user touches a button on the alert view
	public static event Action<string> alertButtonClickedEvent;
	
	// Fired when the user touches the cancel button on a prompt
	public static event Action promptCancelledEvent;
	
	// Fired when the user finishes entering text in the prompt
	public static event Action<string> singleFieldPromptTextEnteredEvent;
	
	// Fired when the user finishes entering text in a two field prompt
	public static event Action<string, string> twoFieldPromptTextEnteredEvent;
	
	// Fired when remote notifications are successfully registered for
	public static event Action<string> remoteRegistrationSucceededEvent;
	
	// Fired when remote notification registration fails
	public static event Action<string> remoteRegistrationFailedEvent;
	
	// Fired when Urban Airship registration succeeds
	public static event Action urbanAirshipRegistrationSucceededEvent;
	
	// Fired when Urban Airship registration fails
	public static event Action<string> urbanAirshipRegistrationFailedEvent;
	
	// Fired when Push.IO registration completes. If the parameter is null then there was no error. Non-null will contain an error message
	public static event Action<string> pushIORegistrationCompletedEvent;
	
	// Fired when a remote notification is received
	public static event Action<IDictionary> remoteNotificationReceivedEvent;
	
	// Fired when a remote notification launched your game
	public static event Action<IDictionary> remoteNotificationReceivedAtLaunchEvent;
	
	// Fired when a local notification is received
	public static event Action<IDictionary> localNotificationWasReceivedEvent;
	
	// Fired when a local notification is received at launch
	public static event Action<IDictionary> localNotificationWasReceivedAtLaunchEvent;
	
	// Fired when the mail composer is dismissed
	public static event Action<string> mailComposerFinishedEvent;
	
	// Fired when the SMS composer is dismissed
	public static event Action<string> smsComposerFinishedEvent;
	
	
	public static string deviceToken { get; private set; }
	public static string pushIOApiKey;
	public static string[] pushIOCategories;
	
	
    static EtceteraManager()
    {
		AbstractManager.initialize( typeof( EtceteraManager ) );
    }
	
	
	public void dismissingViewController()
	{
		if( dismissingViewControllerEvent != null )
			dismissingViewControllerEvent();
	}
	
	
	#region Image picker
	
	public void imagePickerDidCancel( string empty )
	{
		if( imagePickerCancelledEvent != null )
			imagePickerCancelledEvent();
	}
	
	
	public void imageSavedToDocuments( string filePath )
	{
		if( imagePickerChoseImageEvent != null )
			imagePickerChoseImageEvent( filePath );
	}


	public void saveImageToPhotoAlbumFailed( string error )
	{
		saveImageToPhotoAlbumFailedEvent.fire( error );
	}
	
	
	public void saveImageToPhotoAlbumSucceeded( string empty )
	{
		saveImageToPhotoAlbumSucceededEvent.fire();
	}

	
	// Loads up a Texture2D with the image at the given path
	public static IEnumerator textureFromFileAtPath( string filePath, EceteraTextureDelegate del, EceteraTextureFailedDelegate errorDel )
	{
		using( WWW www = new WWW( filePath ) )
		{
			yield return www;
			
			if( www.error != null )
			{
				if( errorDel != null )
					errorDel( www.error );
			}
			
			// Assign the texture to a local variable to avoid leaking it (Unity bug)
			Texture2D tex = www.texture;
	
			if( tex != null )
				del( tex );
		}
	}
	
	#endregion;
	
	
	#region Alert and Prompt
	
	public void alertViewClickedButton( string buttonTitle )
	{
		if( alertButtonClickedEvent != null )
			alertButtonClickedEvent( buttonTitle );
	}
	
	
	public void alertPromptCancelled( string empty )
	{
		if( promptCancelledEvent != null )
			promptCancelledEvent();
	}
	
	
	public void alertPromptEnteredText( string text )
	{
		// Was this one prompt or 2?
		string[] promptText = text.Split( new string[] {"|||"}, StringSplitOptions.None );
		if( promptText.Length == 1 )
		{
			if( singleFieldPromptTextEnteredEvent != null )
				singleFieldPromptTextEnteredEvent( promptText[0] );
		}
		
		if( promptText.Length == 2 )
		{
			if( twoFieldPromptTextEnteredEvent != null )
				twoFieldPromptTextEnteredEvent( promptText[0], promptText[1] );
		}
	}
	
	#endregion;
	
	
	#region Remote Notifications
	
	public void remoteRegistrationDidSucceed( string deviceToken )
	{
		EtceteraManager.deviceToken = deviceToken;
		if( remoteRegistrationSucceededEvent != null )
			remoteRegistrationSucceededEvent( deviceToken );
		
		// if we have Push.IO data perform registration
		if( pushIOApiKey != null )
			StartCoroutine( registerDeviceWithPushIO() );
	}
	
	
	private IEnumerator registerDeviceWithPushIO()
	{
		var url = string.Format( "https://api.push.io/r/{0}?di={1}&dt={2}", pushIOApiKey, SystemInfo.deviceUniqueIdentifier, deviceToken );
		
		// add categories if we have them
		if( pushIOCategories != null && pushIOCategories.Length > 0 )
			url += "&c=" + string.Join( ",", pushIOCategories );
		
		using( WWW www = new WWW( url ) )
		{
			yield return www;
			
			if( pushIORegistrationCompletedEvent != null )
				pushIORegistrationCompletedEvent( www.error );
		}
	}
	
	
	public void remoteRegistrationDidFail( string error )
	{
		if( remoteRegistrationFailedEvent != null )
			remoteRegistrationFailedEvent( error );
	}
	
	
	public void urbanAirshipRegistrationDidSucceed( string empty )
	{
		if( urbanAirshipRegistrationSucceededEvent != null )
			urbanAirshipRegistrationSucceededEvent();
	}
	
	
	public void urbanAirshipRegistrationDidFail( string error )
	{
		if( urbanAirshipRegistrationFailedEvent != null )
			urbanAirshipRegistrationFailedEvent( error );
	}
	
	
	public void remoteNotificationWasReceived( string json )
	{
		if( remoteNotificationReceivedEvent != null )
			remoteNotificationReceivedEvent( json.dictionaryFromJson() );
	}
	
	
	public void remoteNotificationWasReceivedAtLaunch( string json )
	{
		if( remoteNotificationReceivedAtLaunchEvent != null )
			remoteNotificationReceivedAtLaunchEvent( json.dictionaryFromJson() );
	}
	
	
	public void localNotificationWasReceived( string json )
	{
		localNotificationWasReceivedEvent.fire( json.dictionaryFromJson() );
	}
	
	
	public void localNotificationWasReceivedAtLaunch( string json )
	{
		remoteNotificationReceivedAtLaunchEvent.fire( json.dictionaryFromJson() );
	}
	
	#endregion;
	
	
	public void mailComposerFinishedWithResult( string result )
	{
		if( mailComposerFinishedEvent != null )
			mailComposerFinishedEvent( result );
	}
	
	
	public void smsComposerFinishedWithResult( string result )
	{
		if( smsComposerFinishedEvent != null )
			smsComposerFinishedEvent( result );
	}

#endif
}