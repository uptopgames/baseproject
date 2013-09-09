using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementAdColony : AdvertisementBase
{
	private const string API = "AdColony";
	
	private string version;
	private string zone;
	
	public AdvertisementAdColony(string key_android, string key_ios, string version, string zone): base(key_android, key_ios)
	{
		this.version = version;
		this.zone = zone;
	}
	
	// Obtem o video
	public override void fetchVideo(bool force)
	{
		try
		{
			if (!force && tried_fetching_video)
				return;
			
			base.fetchVideo(force);
			
			if (!Info.IsEditor() && !Info.IsWeb())
				AdColony.Configure(version, key, zone);
		}
		catch
		{
			Error(API, ERROR_STARTUP_OBJECT);
		}
	}
	
	public override bool isVideoAvailable()
	{
		try
		{
			if (Info.IsEditor() || Info.IsWeb())
				return false;
			
			return AdColony.IsVideoAvailable();
		}
		catch
		{
			return Error(API, ERROR_CHECK_VIDEO);
		}
	}
	
	// Mostra o video
	public override bool showVideo()
	{
		try
		{
			if (Info.IsEditor() || Info.IsWeb() || !isVideoAvailable())
				return false;
			
			return AdColony.ShowVideoAd();
		}
		catch
		{
			return Error(API, ERROR_PLAY_VIDEO);
		}
	}
}
