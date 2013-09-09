using UnityEngine;
using System.Collections;

public class GameNativeGUI
#if UNITY_EDITOR || UNITY_WEBPLAYER
	: GameNativeDefault
#elif UNITY_IPHONE
	: GameNativeIos
#elif UNITY_ANDROID
	: GameNativeAndroid
#endif
{
	public GameNativeGUI(): base()
	{
	}
	
	public void showMessage(string title, string message)
	{
		showMessage(title, message, null);
	}
	
	public bool openAuthUrlInline(string url)
	{
		if (!Info.HasConnection(true))
			return false;
		
		url = url + "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-","") + "&token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		
		return base.openUrlInline(url);
	}
	
	public override bool openUrlInline(string url)
	{
		if (!Info.HasConnection(true))
			return false;
		return base.openUrlInline(url);
	}
	
	public void showMessageOkCancel(string title, string message)
	{
		showMessageOkCancel(title, message, null, null);
	}
	
	public void startLoading(string title=null, string message=null)
	{
		if (title == null) title = LOADING_TITLE;
		if (message == null) message = LOADING_MESSAGE;
		
		loadingMessage(title, message);
	}
}