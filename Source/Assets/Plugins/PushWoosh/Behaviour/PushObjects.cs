using UnityEngine;
using System.Collections;

public class PushObjects
{

#if !UNITY_EDITOR
	
#if UNITY_ANDROID
	public static AndroidJavaObject pushwoosh = null;
	
	static PushObjects()
	{
		using(var pluginClass = new AndroidJavaClass("com.arellomobile.android.push.PushwooshProxy"))
		pushwoosh = pluginClass.CallStatic<AndroidJavaObject>("instance");
	}
#elif UNITY_IOS
	
#endif
	
#endif
}
