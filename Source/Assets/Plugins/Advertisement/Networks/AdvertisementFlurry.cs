using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementFlurry : AdvertisementBase
{
	private const string API = "Flurry";
	
	private string banner_id;
	private string inters_id;
	
#if UNITY_ANDROID 
		private FlurryAdPlacement placement;
#elif UNITY_IPHONE
		private FlurryAdSize placement;
#endif
	
	public AdvertisementFlurry(string key_android, string key_ios, string banner_id, string inters_id): base(key_android, key_ios)
	{
		if (Info.IsEditor() || Info.IsWeb())
			return;
		
		this.banner_id = banner_id;
		this.inters_id = inters_id;

		#if UNITY_ANDROID
			placement = FlurryAdPlacement.BannerBottom;
		#elif UNITY_IPHONE
			placement =  FlurryAdSize.Bottom;
		#endif
	}
	
	private void onContentAvailable(string ad)
	{
		if (Info.IsEditor() || Info.IsWeb())
			return;
		
		if (ad == inters_id)
			interstitial_showed = true;
		else if (ad == banner_id)
			banner_showed = true;
	}
	
	private void onContentUnavailable(string ad)
	{
		if (Info.IsEditor() || Info.IsWeb())
			return;
		
		if (ad == inters_id)
			interstitial_showed = false;
		else if (ad == banner_id)
			banner_showed = false;
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
				FlurryAndroidManager.adAvailableForSpaceEvent += onContentAvailable;
				FlurryAndroidManager.adNotAvailableForSpaceEvent += onContentUnavailable;
			#endif
			
			Flurry.Start(key);
			
			return true;
		}
		catch
		{
			return Error(API, ERROR_STARTUP_OBJECT);
		}
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
				FlurryAndroid.fetchAdsForSpace(inters_id, FlurryAdPlacement.FullScreen);
				FlurryAndroid.checkIfAdIsAvailable(inters_id, FlurryAdPlacement.FullScreen, 2000);
			#elif UNITY_IPHONE
				FlurryBinding.fetchAdForSpace(inters_id, FlurryAdSize.Fullscreen);
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
			if (!Info.HasConnection())
				return false;
			
			base.showInterstitial();
			
			if (Info.IsEditor() || Info.IsWeb() || (firstTime && !Setup()))
				return false;
			
			#if UNITY_ANDROID
				FlurryAndroid.checkIfAdIsAvailable(inters_id, FlurryAdPlacement.FullScreen, 2000);
				FlurryAndroid.displayAd(inters_id, FlurryAdPlacement.FullScreen, 0);
				return null;
			
			#elif UNITY_IPHONE
				if (!FlurryBinding.isAdAvailableForSpace(inters_id, FlurryAdSize.Fullscreen))
					return false;
				
				FlurryBinding.fetchAndDisplayAdForSpace(inters_id, FlurryAdSize.Fullscreen);
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
			
			if (Info.IsEditor())
				return;
			
			#if UNITY_ANDROID
				if (pos == AdvertisementManager.Positions.BOTTOM)
					placement = FlurryAdPlacement.BannerBottom;
				else placement = FlurryAdPlacement.BannerTop;
				
				FlurryAndroid.fetchAdsForSpace(banner_id, placement);
			#elif UNITY_IPHONE
				if (pos == AdvertisementManager.Positions.BOTTOM)
					placement = FlurryAdSize.Bottom;
				else placement = FlurryAdSize.Top;
				
				FlurryBinding.fetchAdForSpace(banner_id, placement);
			#endif
		}
		catch
		{
			Error(API, ERROR_LOADING_BANNER);
		}	
	}
	
	public override bool? showBanner()
	{
		//Debug.Log("show flurry");
		try
		{
			base.showBanner();
			
			if (Info.IsEditor())
			{
				//Debug.Log("show return flurry");
				return false;
			}
#if UNITY_ANDROID
				FlurryAndroid.checkIfAdIsAvailable(banner_id, placement, 2000);
				FlurryAndroid.displayAd(banner_id, placement, 0);
				return null;
			
#elif UNITY_IPHONE
				if (!FlurryBinding.isAdAvailableForSpace(banner_id, placement))
				{
					//Debug.Log("show return 2 flurry");
					return false;
				}	
				FlurryBinding.displayAdForSpace(banner_id, placement);
				return true;
			
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
		//Debug.Log("hide flurry");
		try
		{
			if (Info.IsEditor())
			{
				//Debug.Log("return flurry");
				return;
			}
			#if UNITY_ANDROID
				FlurryAndroid.removeAd(banner_id);
			
			#elif UNITY_IPHONE
				FlurryBinding.removeAdFromSpace(banner_id);
			#endif
		}
		catch
		{
			Error(API, ERROR_TRY_HIDE_BANNER);
		}
	}
}
