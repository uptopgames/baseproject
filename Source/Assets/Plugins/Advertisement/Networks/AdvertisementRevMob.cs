using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementRevMob : AdvertisementBase
{
	private const string API = "RevMob";
	
	// Instancia do revmob para manipulacao
	private RevMob instance;
	
	// Banner
	private RevMobBanner banner;
	
	private Dictionary<string, string> dict;
	private string adName;
	
	public AdvertisementRevMob(string key_android, string key_ios, string name): base(key_android, key_ios)
	{
		if (Info.IsEditor())
			return;
		
		dict = new Dictionary<string, string>()
		{
			{"Android", key_android},
			{"IOS", key_ios}
		};
		
		adName = name;
	}
	
	private bool firstTime = true;
	
	private bool Setup()
	{
		if (!firstTime)
			return true;
		
		firstTime = false;
		
		try
		{
			if (!Info.IsEditor())
				instance = RevMob.Start(dict, adName);
			
			return true;
		}
		catch
		{
			return Error(API, ERROR_STARTUP_OBJECT);
		}
	}
	
	// Mostra o popup
	public override bool showPopup()
	{
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
				return false;
			
			instance.ShowPopup();
			return true;
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_POPUP);
		}
	}
	
	public override bool? showWidget()
	{
		try
		{
			base.showWidget();
			
			if (Info.IsEditor() || (firstTime && !Setup()))
				return false;
	
			instance.OpenAdLink();
			return null;
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_WIDGET);
		}
	}
	
	public override bool? showInterstitial()
	{
		try
		{
			base.showInterstitial();
			
			if (Info.IsEditor() || !Info.HasConnection() || (firstTime && !Setup()))
				return false;
			
			instance.ShowFullscreen();
			return null;
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
			
			#if UNITY_IPHONE
				ScreenOrientation or;
				
				if (pos == AdvertisementManager.Positions.BOTTOM) or = Screen.orientation;
				else
				{
					switch (Screen.orientation)
					{
						case ScreenOrientation.Portrait: or = ScreenOrientation.PortraitUpsideDown; break;
						case ScreenOrientation.PortraitUpsideDown: or = ScreenOrientation.Portrait; break;
						case ScreenOrientation.LandscapeLeft: or = ScreenOrientation.LandscapeRight; break;
						case ScreenOrientation.LandscapeRight: or = ScreenOrientation.LandscapeLeft; break;
						default: or = ScreenOrientation.PortraitUpsideDown; break;
					}
				}
				
				ScreenOrientation[] ors = new ScreenOrientation[1] {or};
				
				banner = instance.CreateBanner(null, ors);
			#endif
		}
		catch
		{
			Error(API, ERROR_LOADING_BANNER);
		}
	}
	
	public override bool? showBanner()
	{
		//Debug.Log("show revmob");
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("show return revmob");
				return false;
			}
			#if UNITY_IPHONE
				if (banner == null) 
				{
					//Debug.Log("show return 2 revmob");
					return false;
				}
				banner.Show();
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
		//Debug.Log("hide revmob");
		try
		{
			if (Info.IsEditor() || (firstTime && !Setup()))
			{
				//Debug.Log("return revmob");
				return;
			}
			#if UNITY_IPHONE
				if (banner != null) banner.Hide();
			#endif
		}
		catch
		{
			Error(API, ERROR_TRY_HIDE_BANNER);
		}
	}
	
	public void receivedWidget(bool received)
	{
		widget_showed = received;
	}
	
	public void receivedInterstitial(bool received)
	{
		interstitial_showed = received;
	}
	
	public void receivedBanner(bool received)
	{
		banner_showed = received;
	}
}
