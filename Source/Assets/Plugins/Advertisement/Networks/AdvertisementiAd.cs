using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementiAd : AdvertisementBase
{
	private const string API = "iAd";
	
	#if UNITY_IPHONE
		private bool on_bottom;
	#endif
	
	public AdvertisementiAd(): base(null, null)
	{
	}
	
	private bool firstTime = true;
	
	private bool Setup()
	{
		if (!firstTime)
			return true;
		
		firstTime = false;
		
		try
		{
			#if UNITY_IPHONE
				Initializate.AddPrefab("AdManager", typeof(AdManager));
			
				AdBinding.fireHideShowEvents(true);
				AdManager.adViewDidChange += onAdShowed;
			#endif
			
			return true;
		}
		catch
		{
			return Error(API, ERROR_STARTUP_OBJECT);
		}
	}
	
	private void onAdShowed(bool showed)
	{
		//Debug.Log("iad mudou: "+showed);
		banner_showed = showed;
	}
	
	public override void fetchBanner(AdvertisementManager.Positions pos)
	{
		try
		{
			base.fetchBanner(pos);
			
			if (Info.IsEditor() || !Info.IsPlatform(Info.Platform.iPhone) || (firstTime && !Setup()))
				return;
			
			#if UNITY_IPHONE
				on_bottom = pos == AdvertisementManager.Positions.BOTTOM;
			#endif
		}
		catch
		{
			Error(API, ERROR_LOADING_BANNER);
		}
	}
	
	public override bool? showBanner()
	{
		//Debug.Log("show iad");
		try
		{
			base.showBanner();
			
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("show return iad");
				return false;
			}
			#if UNITY_IPHONE
				//normal
				//AdBinding.createAdBanner(on_bottom);
			
				// forcar aparecer em baixo
				AdBinding.createAdBanner(true);
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
		//Debug.Log("hide iad");
		
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("return iAd");
				return;
			}
			#if UNITY_IPHONE
				AdBinding.destroyAdBanner();
				banner_showed = false;
			#endif
		}
		catch
		{
			Error(API, ERROR_TRY_HIDE_BANNER);
		}
	}
}
