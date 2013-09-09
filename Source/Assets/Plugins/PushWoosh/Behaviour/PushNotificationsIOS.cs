using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IPHONE && !UNITY_EDITOR

public abstract class PushNotificationsIOS : MonoBehaviour {

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setListenerName(string listenerName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public System.IntPtr _getPushToken();
	
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setIntTag(string tagName, int tagValue);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setStringTag(string tagName, string tagValue);
	
	// Use this for initialization
	public void Start()
	{
		setListenerName(gameObject.name);
	}

	public static string getPushDevice()
	{
		return SystemInfo.deviceUniqueIdentifier.Replace("-", "");
	}

	public static string getPushToken()
	{
		return Marshal.PtrToStringAnsi(_getPushToken());
	}
}

#endif
