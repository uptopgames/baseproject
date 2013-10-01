using UnityEngine;
using System;
using System.Collections;

public delegate void PressButtonDelegate();
public delegate void PressCancelDelegate();

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
	
	public void showMessage(string title = "", string message = "", string button = "")
	{
		base.showMessage(title, message, button);
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
	
	public void showMessageOkCancel(MonoBehaviour classScript, string okMethod, Action<string> nativeMethod, string cancelMethod = "", 
		string title = "", string message = "", string okButton = "", string cancelButton = "")
	{
		
#if UNITY_EDITOR || UNITY_WEBPLAYER
		Flow.messageOkCancelDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().scriptWithMethodToInvoke = classScript;
		Flow.messageOkCancelDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().methodToInvoke = okMethod;
		if(cancelMethod == "")
		{
			Flow.messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().scriptWithMethodToInvoke = Flow.messageOkCancelDialog.GetComponent<DialogCancel>();
			Flow.messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().methodToInvoke = "BaseCancel";
		}
		else
		{
			Flow.messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().scriptWithMethodToInvoke = classScript;
			Flow.messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().methodToInvoke = cancelMethod;
		}
#else
		base.addActionShowMessage(nativeMethod);		
#endif
		
		base.showMessageOkCancel(title, message, okButton, cancelButton);
	}
	
	public void startLoading(string title=null, string message=null)
	{
		//if (title == null) title = LOADING_TITLE;
		//if (message == null) message = LOADING_MESSAGE;
		
		loadingMessage(title, message);
	}
}
