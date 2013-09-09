using UnityEngine;
using System.Collections;

public abstract class PushNotifications
#if UNITY_EDITOR || UNITY_WEBPLAYER
	: PushNotificationsEditor
#elif UNITY_ANDROID
	: PushNotificationsAndroid
#elif UNITY_IPHONE
	: PushNotificationsIOS
#endif
{
	public abstract void onRegisteredForPushNotifications(string token);

	public abstract void onFailedToRegisteredForPushNotifications(string error);

	public abstract void onPushNotificationsReceived(string payload);
}
