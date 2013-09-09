using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;


#if UNITY_ANDROID
public class RevMobAndroid : RevMob {
	private AndroidJavaObject session;

	public RevMobAndroid(string appId, string gameObjectName) {
		this.gameObjectName = gameObjectName;
		AndroidJavaClass unityRevMobClass = new AndroidJavaClass("com.revmob.unity.UnityRevMob");
		this.session = unityRevMobClass.CallStatic<AndroidJavaObject>("start",
	                                                               RevMobAndroid.CurrentActivity(),
	                                                               appId,
	                                                               "unity-android",
	                                                               REVMOB_VERSION);
	}


	public static AndroidJavaObject CurrentActivity() {
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public override bool IsDevice() {
		return (Application.platform == RuntimePlatform.Android);
	}

	private AndroidJavaObject adUnitWrapperCall(string methodName, string placementId, string adUnit) {
		if (placementId == null) {
			placementId = "";
		}
		AndroidJavaObject publisherListener = CreateRevMobListener(this.gameObjectName, adUnit);
		AndroidJavaObject obj = this.session.Call<AndroidJavaObject>(methodName, CurrentActivity(), placementId, publisherListener);
		return obj;
	}

	private AndroidJavaObject CreateRevMobListener(String gameObjectName, String adUnityType) {
		return new AndroidJavaObject("com.revmob.unity.RevMobAdsUnityListener", gameObjectName, adUnityType);
	}



	public override void PrintEnvironmentInformation() {
		session.Call("printEnvironmentInformation", CurrentActivity());
	}

	public override void SetTestingMode(RevMob.Test test) {
		session.Call("setTestingMode", (int)test);
	}

	public override void SetTimeoutInSeconds(int timeout) {
		session.Call("setTimeoutInSeconds", timeout);
	}



	public override RevMobFullscreen ShowFullscreen(string placementId) {
		return new RevMobAndroidFullscreen(this.adUnitWrapperCall("showFullscreen", placementId, "Fullscreen"));
	}

	public override RevMobFullscreen CreateFullscreen(string placementId) {
		if (!IsDevice ()) return null;
		AndroidJavaObject javaObject = this.adUnitWrapperCall("createFullscreen", placementId, "Fullscreen");
		return new RevMobAndroidFullscreen(javaObject);
	}



	public override void CreateBanner(RevMob.Position position, int x, int y, int w, int h) {
		if (!IsDevice ()) return;
		this.session.Call("createBanner", CurrentActivity(), CreateRevMobListener(this.gameObjectName, "Banner"), (int)position, x, y, w, h);
	}

	public override void ShowBanner() {
		this.session.Call("showBanner", CurrentActivity());
	}

	public override void HideBanner() {
		this.session.Call("hideBanner", CurrentActivity());
	}

	public override void ReleaseBanner() {
		this.session.Call("releaseBanner", CurrentActivity());
	}



	public override RevMobLink OpenAdLink(string placementId) {
		return new RevMobAndroidLink(this.adUnitWrapperCall("openAdLink", placementId, "Link"));
	}

	public override RevMobLink CreateAdLink(string placementId)	{
		if (!IsDevice ()) return null;
		AndroidJavaObject javaObject = this.adUnitWrapperCall("createAdLink", placementId, "Link");
		return new RevMobAndroidLink(javaObject);
	}



	public override RevMobPopup ShowPopup(string placementId) {
		return new RevMobAndroidPopup(this.adUnitWrapperCall("showPopup", placementId, "Popup"));
	}

	public override RevMobPopup CreatePopup(string placementId) {
		return new RevMobAndroidPopup(this.adUnitWrapperCall("createPopup", placementId, "Popup"));
	}
}
#endif