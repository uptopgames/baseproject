using UnityEngine;
using System.Collections;

#if UNITY_ANDROID && !UNITY_EDITOR

public abstract class PushNotificationsAndroid: MonoBehaviour
{
	public void Start()
	{
		setListenerName(gameObject.name);
	}
	
	public static void setListenerName(string listenerName)
	{
		PushObjects.pushwoosh.Call("setListenerName", listenerName);
	}

	public static void setIntTag(string tagName, int tagValue)
	{
		PushObjects.pushwoosh.Call("setIntTag", tagName, tagValue);
	}

	public static void setStringTag(string tagName, string tagValue)
	{
		PushObjects.pushwoosh.Call("setStringTag", tagName, tagValue);
	}
	
	public static string getPushDevice()
	{
		return getPushToken();
	}
	
	public static string getPushToken()
	{
		return PushObjects.pushwoosh.Call<string>("getPushToken");
	}
}

#endif
