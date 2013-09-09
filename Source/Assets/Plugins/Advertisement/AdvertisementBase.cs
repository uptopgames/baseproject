using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public abstract class AdvertisementBase
{
	protected const string
		ERROR_STARTUP_OBJECT = "Something is wrong with '$network', crash when startup object!",
		ERROR_STARTUP_CONSTRUCTOR = "Something is wrong with '$network', crash when loading constructor!",
		
		ERROR_LOADING_BANNER = "Something is wrong with '$network', crash when loading banner!",
		ERROR_TRY_SHOW_BANNER = "Something is wrong with '$network', crash when trying show banner!",
		ERROR_TRY_HIDE_BANNER = "Something is wrong with '$network', crash when trying hide banner!",
		
		ERROR_LOADING_INTERSTITIAL = "Something is wrong with '$network', crash when loading interstitial!",
		ERROR_TRY_SHOW_INTERSTITIAL = "Something is wrong with '$network', crash when trying show interstitial!",
		ERROR_TRY_HIDE_INTERSTITIAL = "Something is wrong with '$network', crash when trying hide interstitial!",
		
		ERROR_LOADING_WIDGET = "Something is wrong with '$network', crash when loading widget!",
		ERROR_TRY_SHOW_WIDGET = "Something is wrong with '$network', crash when trying show widget!",
		ERROR_TRY_HIDE_WIDGET = "Something is wrong with '$network', crash when trying hide widget!",
		
		ERROR_LOADING_POPUP = "Something is wrong with '$network', crash when loading popup!",
		ERROR_TRY_SHOW_POPUP = "Something is wrong with '$network', crash when trying show popup!",
		ERROR_TRY_HIDE_POPUP = "Something is wrong with '$network', crash when trying hide popup!",
		
		ERROR_CHECK_VIDEO = "Something is wrong with '$network', crash when check video available!",
		ERROR_LOADING_VIDEO = "Something is wrong with '$network', crash when trying play video!",
		ERROR_PLAY_VIDEO = "Something is wrong with '$network', crash when trying play video!";
		
	protected bool Error(string network, string error)
	{
		//Debug.Log("errooooor!!");
		Debug.LogWarning(
			(Advertisement.API + ": " + error.Replace("$network", network))
		);
		
		return false;
	}
	
	// Chaves por sistema operacional
	protected string key_android;
	protected string key_ios;
	
	// Indica se tentou obter o componente
	protected bool tried_fetching_interstitial;
	protected bool tried_fetching_video;
	
	// Indica se o componente foi exibido
	protected bool? interstitial_showed;
	protected bool? widget_showed;
	protected bool? banner_showed;
	
	// Chave para o sistema atual
	protected string key;
	
	public AdvertisementBase(string key_android, string key_ios)
	{
		this.key_android = key_android;
		this.key_ios = key_ios;
		chooseKey();
	}
	
	// Escolhe a chave com base no sistema operacional
	protected void chooseKey()
	{
#if !UNITY_EDITOR
		
#if UNITY_ANDROID
		key = key_android;
#elif UNITY_IPHONE
		key = key_ios;
#endif
		
#endif
	}
	
	// Obtem o video
	public void fetchVideo()
	{
		fetchVideo(false);
	}
	
	public virtual void fetchVideo(bool force)
	{
		tried_fetching_video = true;
	}
	
	// Mostra o video
	public virtual bool showVideo()
	{
		return false;
	}
	
	// Obtem o popup
	public virtual void fetchPopup(bool force)
	{
	}
	
	// Mostra o popup
	public virtual bool showPopup()
	{
		return false;
	}
	
	// Obtem o widget
	public virtual void fetchWidget(bool force)
	{
	}
	
	// Mostra o widget
	public virtual bool? showWidget()
	{
		widget_showed = null;
		return false;
	}
	
	// Verifica se o widget foi mostrado
	public virtual bool? showedWidget()
	{
		return widget_showed;
	}
	
	// Obtem o interstitial
	public void fetchInterstitial()
	{
		fetchInterstitial(false);
	}
	
	public virtual void fetchInterstitial(bool force)
	{
		tried_fetching_interstitial = true;
	}
	
	// Mostra o interstitial
	public virtual bool? showInterstitial()
	{
		interstitial_showed = null;
		return false;
	}
	
	// Verifica se o interstitial foi mostrado
	public virtual bool? showedInterstitial()
	{
		return interstitial_showed;
	}
	
	// Obtem o banner
	public virtual void fetchBanner(AdvertisementManager.Positions pos)
	{
	}
	
	// Mostra o banner
	public virtual bool? showBanner()
	{
		banner_showed = null;
		return false;
	}
	
	// Verifica se o banner foi mostrado
	public virtual bool? showedBanner()
	{
		return banner_showed;
	}
	
	// Esconde o banner
	public virtual void hideBanner()
	{
	}
	
	public virtual bool isVideoAvailable()
	{
		return false;	
	}
}
