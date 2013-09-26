using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;


public abstract class RevMob {
	protected static readonly string REVMOB_VERSION = "6.3.0";
	protected string gameObjectName;

	public enum Test {
		DISABLED = 0,
		WITH_ADS = 1,
		WITHOUT_ADS = 2
	}
#if UNITY_ANDROID
	public enum Position {
		BOTTOM = 0,
		TOP = 1
	}
#endif
	public abstract void PrintEnvironmentInformation();
	public abstract void SetTestingMode(RevMob.Test test);
	public abstract void SetTimeoutInSeconds(int timeout);
	public abstract bool IsDevice();

	public abstract RevMobFullscreen ShowFullscreen(string placementId);
	public abstract RevMobFullscreen CreateFullscreen(string placementId);
#if UNITY_ANDROID
	public abstract void CreateBanner(RevMob.Position position, int x, int y, int w, int h);
	public abstract void ShowBanner();
	public abstract void HideBanner();
	public abstract void ReleaseBanner();
#elif UNITY_IPHONE
	public abstract RevMobBanner CreateBanner(float x, float y, float width, float height, string placementId, ScreenOrientation[] orientations);
#endif
	public abstract RevMobLink OpenAdLink(string placementId);
	public abstract RevMobLink CreateAdLink(string placementId);

	public abstract RevMobPopup ShowPopup(string placementId);
	public abstract RevMobPopup CreatePopup(string placementId);


	public static RevMob Start(Dictionary<string, string> appIds) {
		return Start(appIds, null);
	}

	public static RevMob Start(Dictionary<string, string> appIds, string gameObjectName) {
		Debug.Log("Creating RevMob Session");
#if UNITY_EDITOR
		Debug.Log("It Can't run in Unity Editor. Only in iOS or Android devices.");
		return null;
#elif UNITY_ANDROID
		RevMob session = new RevMobAndroid(appIds["Android"], gameObjectName);
		return session;
#elif UNITY_IPHONE
		RevMob session = new RevMobIos(appIds["IOS"], gameObjectName);
		return session;
#else
		return null;
#endif
	}

	public RevMobFullscreen ShowFullscreen() {
		return this.ShowFullscreen(null);
	}

	public RevMobFullscreen CreateFullscreen() {
		return this.CreateFullscreen(null);
	}

#if UNITY_ANDROID
	public void CreateBanner() {
		this.CreateBanner(Position.BOTTOM, 0, 0, 0, 0);
	}

	public void CreateBanner(RevMob.Position position) {
		this.CreateBanner(position, 0, 0, 0, 0);
	}
#elif UNITY_IPHONE
	public RevMobBanner CreateBanner() {
		return this.CreateBanner(0, 0, 0, 0, null, null);
	}

	public RevMobBanner CreateBanner(string placementId) {
	    return this.CreateBanner(0, 0, 0, 0, placementId, null);
    }

    public RevMobBanner CreateBanner(string placementId, ScreenOrientation[] orientations) {
	    return this.CreateBanner(0, 0, 0, 0, placementId, orientations);
	}
#endif

	public RevMobLink OpenAdLink() {
		return this.OpenAdLink(null);
	}

	public RevMobLink CreateAdLink() {
		return this.CreateAdLink(null);
	}

	public RevMobPopup ShowPopup() {
		return this.ShowPopup(null);
	}

	public RevMobPopup CreatePopup() {
		return this.CreatePopup(null);
	}

}

