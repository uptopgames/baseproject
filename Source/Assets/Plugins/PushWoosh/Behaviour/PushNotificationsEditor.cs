using UnityEngine;
using System.Collections;

#if UNITY_EDITOR || UNITY_WEBPLAYER

public abstract class PushNotificationsEditor: MonoBehaviour
{
	public void Start()
	{
	}
	
	public static void setListenerName(string listenerName)
	{
	}

	public void setIntTag(string tagName, int tagValue)
	{
	}

	public void setStringTag(string tagName, string tagValue)
	{
	}

	public static string getPushDevice()
	{
		return null;
	}
	
	public static string getPushToken()
	{
		return null;
	}
}

#endif
