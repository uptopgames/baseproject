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
	
	public void showMessage(GameObject messageOkDialog, string title = "", string message = "", string button = "")
	{
		base.showMessage(messageOkDialog, title, message, button);
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
	
	public void showMessageOkCancel(GameObject messageOkCancelDialog, MonoBehaviour classScript, string okMethod, Action<string> nativeMethod, string cancelMethod = "", 
		string title = "", string message = "", string okButton = "", string cancelButton = "")
	{
		
#if UNITY_EDITOR || UNITY_WEBPLAYER
		messageOkCancelDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().scriptWithMethodToInvoke = classScript;
		messageOkCancelDialog.transform.FindChild("ConfirmButtonPanel").FindChild("ConfirmButton").GetComponent<UIButton>().methodToInvoke = okMethod;
		if(cancelMethod == "")
		{
			Debug.Log("sem cancel");
			messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().scriptWithMethodToInvoke = messageOkCancelDialog.GetComponent<DialogCancel>();
			messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().methodToInvoke = "BaseCancel";
		}
		else
		{
			messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().scriptWithMethodToInvoke = classScript;
			messageOkCancelDialog.transform.FindChild("CancelButtonPanel").FindChild("CancelButton").GetComponent<UIButton>().methodToInvoke = cancelMethod;
		}
#else
		base.addActionShowMessage(nativeMethod);		
#endif
		
		base.showMessageOkCancel(messageOkCancelDialog, title, message, okButton, cancelButton);
	}
	
	public void startLoading(GameObject loadingDialog, string title=null, string message=null)
	{
		//if (title == null) title = LOADING_TITLE;
		//if (message == null) message = LOADING_MESSAGE;
		
		loadingMessage(loadingDialog, title, message);
	}
}
