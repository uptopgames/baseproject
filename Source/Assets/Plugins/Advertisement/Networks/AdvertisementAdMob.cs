using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementAdMob : AdvertisementBase
{
	private const string API = "AdMob";
	
	private string inters_id;
	
	#if UNITY_ANDROID
		AdMobAdPlacement placement;
	#endif
	
	
	public AdvertisementAdMob(string key_android, string inters_id): base(key_android, null)
	{	
		try
		{
			if (Info.IsEditor())
				return;
			
			#if UNITY_ANDROID
				this.inters_id = inters_id;
				
				placement = AdMobAdPlacement.BottomCenter;
			#endif
		}
		catch
		{
			Error(API, ERROR_STARTUP_CONSTRUCTOR);
		}
	}
	
	private bool firstTime = true;
	
	private bool Setup()
	{
		if (!firstTime)
			return true;
		
		firstTime = false;
		
		try
		{
#if UNITY_ANDROID
				Initializate.AddPrefab("AdMobAndroidManager", typeof(AdMobAndroidManager));
			
				AdMobAndroidManager.receivedAdEvent += onBannerShowed;
				AdMobAndroidManager.failedToReceiveAdEvent += onBannerFailed;
				
				AdMobAndroid.init(key);
#endif
			
			return true;
		}
		catch
		{
			return Error(API, ERROR_STARTUP_OBJECT);
		}
		
	}
	
	private void onBannerShowed()
	{
		banner_showed = true;
	}
	
	private void onBannerFailed(string stuff)
	{
		banner_showed = false;
	}
	
	public override void fetchInterstitial(bool force)
	{
		try
		{
			if (!force && tried_fetching_interstitial)
				return;
			
			base.fetchInterstitial(force);
			
			if (Info.IsEditor() || (firstTime && !Setup()))
				return;
			
			#if UNITY_ANDROID
				AdMobAndroid.requestInterstital(inters_id);
			#endif
		}
		catch
		{
			Error(API, ERROR_LOADING_INTERSTITIAL);
		}
	}
	
	public override bool? showInterstitial()
	{
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
				return false;
			
			#if UNITY_ANDROID
				if (!AdMobAndroid.isInterstitalReady())
					return false;
				AdMobAndroid.displayInterstital();
				
				// Obtem novamente o interstitial para a proxima execucao
				AdMobAndroid.requestInterstital(inters_id);
				
				return true;
			#else
				return false;
			#endif
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_INTERSTITIAL);
		}
	}
	
	public override void fetchBanner(AdvertisementManager.Positions pos)
	{
		try
		{
			base.fetchBanner(pos);
			
			if (Info.IsEditor() || (firstTime && !Setup()))
				return;
			
			#if UNITY_ANDROID
				placement = (pos == AdvertisementManager.Positions.BOTTOM)
					? AdMobAdPlacement.BottomCenter
					: AdMobAdPlacement.TopCenter;
			#endif
		}
		catch
		{
			Error(API, ERROR_LOADING_BANNER);
		}
	}
	
	public override bool? showBanner()
	{
		//Debug.Log("show admob");
		try
		{
			base.showBanner();
			
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("show return admob");
				return false;
			}
			#if UNITY_ANDROID
				AdMobAndroid.createBanner(AdMobAndroidAd.smartBanner, placement);
				return null;
			#else
				return false;
			#endif
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_BANNER);
		}
	}
	
	public override void hideBanner()
	{
		//Debug.Log("hide admob");
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("return admob");
				return;
			}
			#if UNITY_ANDROID
				AdMobAndroid.hideBanner(true);
			#endif
		}
		catch
		{
			Error(API, ERROR_TRY_HIDE_BANNER);
		}
	}
}
