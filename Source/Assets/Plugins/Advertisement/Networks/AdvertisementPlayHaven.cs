using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System.Runtime.CompilerServices;

public class AdvertisementPlayHaven : AdvertisementBase
{
	private const string API = "PlayHaven";
	
	private string inters_id;
	
	public AdvertisementPlayHaven(string inters_id): base(null, null)
	{
		this.inters_id = inters_id;
	}
	
	private bool firstTime = true;
	
	private bool Setup()
	{
		if (!firstTime)
			return true;
		
		firstTime = false;
		
		try
		{
			string prefab = API + "Manager";
			
			if (!GameObject.Find(prefab))
			{
				GameObject comp = (GameObject)Resources.Load(API + "/" + prefab, typeof(GameObject));
				
				if (comp != null)
				{
					GameObject instance = (GameObject)GameObject.Instantiate(comp) as GameObject;
					instance.name = prefab;
				}
			}
			
			PlayHaven.PlayHavenManager.instance.OnDidDisplayContent += onWidgetShow;
			PlayHaven.PlayHavenManager.instance.OnErrorCrossPromotionWidget += onWidgetError;
			
			PlayHaven.PlayHavenManager.instance.OnDidDisplayContent += onInterstitialShow;
			PlayHaven.PlayHavenManager.instance.OnErrorContentRequest += onInterstitialError;
			
			return true;
		}
		catch
		{
			return Error(API, ERROR_STARTUP_OBJECT);
		}
	}
	
	public override bool? showWidget()
	{
		try
		{
			base.showWidget();
		
			if (Info.IsEditor() || (firstTime && !Setup()))
				return false;

			PlayHaven.PlayHavenManager.instance.ShowCrossPromotionWidget();
			return null;
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_WIDGET);
		}
	}
	
	private void onInterstitialShow(int request_id)
	{
		interstitial_showed = true;
	}
	
	private void onInterstitialError(int request_id, PlayHaven.Error error)
	{
		interstitial_showed = false;
		
		Error(API, ERROR_TRY_SHOW_INTERSTITIAL + "\n" +
			"Code: " + error.code + " | Description: " + error.description);
	}
	
	private void onWidgetShow(int request_id)
	{
		widget_showed = true;
	}
	
	private void onWidgetError(int request_id, PlayHaven.Error error)
	{
		widget_showed = false;
		
		Error(API, ERROR_TRY_SHOW_WIDGET + "\n" +
			"Code: " + error.code + " | Description: " + error.description);
	}
	
	public override void fetchInterstitial(bool force)
	{
		try
		{
			if (!force && tried_fetching_interstitial)
				return;
			base.fetchInterstitial(force);
			
			if (firstTime && !Setup())
				return;
			
			if (!Info.IsEditor())
				PlayHaven.PlayHavenManager.instance.OpenNotification();
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
			
			if (Info.IsEditor() || (firstTime && !Setup()))
				return false;
			
			PlayHaven.PlayHavenManager.instance.ContentRequest(inters_id);
			return null;
		}
		catch
		{
			return Error(API, ERROR_TRY_SHOW_INTERSTITIAL);
		}
	}
}
