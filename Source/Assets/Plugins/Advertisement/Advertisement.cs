// Classe estatica para manipular Advertisement
// Banners, Interstitials, PopUps, WidGets, Videos

using UnityEngine;
using System.Collections;

public static class Advertisement
{
	// Ao comprar o item "No-Ads", é necessario executar o método NoAdsPurchased()
	// para informar a API de Advertisement que o usuario realmente tem o item
	//
	// Ex: Advertisement.NoAdsPurchased();
	//
	// Irá habilitar o item "No-Ads" e remover os Ads atuais (Banners) sendo exibidos
	// Se um Interstitial ou Video estiver sendo exibido, continuara sendo exibido
	public static void NoAdsPurchased()
	{
#if !UNITY_WEBPLAYER
		string id = SystemInfo.deviceUniqueIdentifier.ToMD5();
		Save.Set("__ads", id);
		
		Banner.Hide();
#endif
	}
	
	// Se a API de Advertisement realmente esta sendo utilizada
	// 
	// Quando o metodo retorna false:
	// - Se a Prefab "AdvertisementManager" (inserida dentro da Prefab "#Configuration#") for nula
	// - Se a bool isActive do "AdvertisementManager" estiver como false
	// - Se nenhum Advertisement conseguir ser instanciado
	//
	// Ex: if (Advertisement.IsRunning())
	//	      Debug.log("Advertisement API esta funcionando");
	public static bool IsRunning()
	{
#if !UNITY_WEBPLAYER
		return AdvertisementManager.isEnabled();	
#else
		return false;
#endif
		
	}
	
	public static class Banner
	{	
		public static void Fetch(Advertisement.Position pos = Advertisement.Position.Top)
		{
#if !UNITY_WEBPLAYER
			if (!canShowAds)
				return;
			
			AdvertisementManager.fetchBanner(pos == Advertisement.Position.Top? AdvertisementManager.Positions.TOP : AdvertisementManager.Positions.BOTTOM);
#endif
		}
		
		public static bool Show()
		{
#if !UNITY_WEBPLAYER
			//Debug.Log("show 1");
			if (!canShowAds)
			{
				return false;
			}		
			return AdvertisementManager.showBanner();
#else
			return false;
#endif
		}
		
		public static void Hide()
		{
			//Debug.Log("hide 1");
#if !UNITY_WEBPLAYER
			AdvertisementManager.hideBanner();
#endif
		}
	}
	
	public static class Video
	{
		
		public static void Fetch()//float delay = 0f)
		{
#if !UNITY_WEBPLAYER
			/*if (delay > 0f) new KTimer(delay, AdvertisementManager.fetchVideo);
			else */AdvertisementManager.fetchVideo();
#endif
		}
		
		public static bool IsDownloaded()
		{
#if !UNITY_WEBPLAYER
			return AdvertisementManager.isVideoAvailable();
#else
			return false;
#endif
		}
		
		public static bool Play()
		{
#if !UNITY_WEBPLAYER
			return AdvertisementManager.showVideo();
#else
			return false;
#endif
		}
	}
	
	public static class Interstitial
	{
		public static void Fetch()//float delay = 0f)
		{
#if !UNITY_WEBPLAYER
			if (!canShowAds)
				return;
			
			/*if (delay > 0f)
				new KTimer(delay, AdvertisementManager.fetchInterstitial);
			else */AdvertisementManager.fetchInterstitial();
#endif
		}
		
		public static void Show()//float delay = 0f)
		{
#if !UNITY_WEBPLAYER
			if (!canShowAds)
				return;
			
			/*if (delay > 0f)
				new KTimer(delay, AdvertisementManager.showInterstitial);
			else */AdvertisementManager.showInterstitial();
#endif
		}
	}
	
	public static class More
	{
		public static void Widget()//float delay = 0f)
		{
#if !UNITY_WEBPLAYER
			if (!canShowAds)
				return;
			
			/*if (delay > 0f)
				new KTimer(delay, AdvertisementManager.showWidget);
			else */AdvertisementManager.showWidget();
#endif
		}
		
		public static bool PopUp()
		{
#if !UNITY_WEBPLAYER
			return AdvertisementManager.showPopup();
#else
			return false;
#endif
		}
	}
	
	// TODOS MÉTODOS ABAIXO SAO UTILIZADOS PARA USO INTERNO SOMENTE
	
	public static string API = "Advertisement";
	
	public enum Position
	{
		Top, Bottom
	};
	
	private static bool canShowAds
	{
		get
		{
			// Se o jogo é versao full
			if (Info.appType == Info.AppType.Pay)
				return false;
			
			// Se o usuario comprou o item NoAds
			string id = SystemInfo.deviceUniqueIdentifier.ToMD5();
			return Save.GetString("__ads") != id;
		}
	}
}
