using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class AdvertisementManager : MonoBehaviour, IRevMobListener
{
	private static string API = "AdvertisementManager";
	
	public bool isActive = true;
	
	string urlToOpenOnEditor = "https://play.google.com/store/apps/details?id=com.rovio.angrybirds";
	
	protected static AdvertisementManager manager
	{
		get
		{
			if (AdvertisementManager._manager != null)
				return AdvertisementManager._manager;
				
			GameObject gameAds = GameObject.Find(ConfigManager.API);
			if (gameAds != null)
				return gameAds.GetComponent<AdvertisementManager>();
			
			gameAds = GameObject.Find(AdvertisementManager.API);
			if (gameAds == null)
				return default(AdvertisementManager);
			
			AdvertisementManager._manager = gameAds.GetComponent<AdvertisementManager>();
			
			return AdvertisementManager._manager;
		}
	}
	private static AdvertisementManager _manager;
	
	protected const int
		AMOUNT_FRAMES_TO_WAIT = 360,
		MAX_SINGLE_CONNECTION_TRIES = 5;
	
	protected const float
		DELAY_CONNECTION = 0.5f;
	
	protected const string
		ADS_TYPE_NONE = "None",
		
		ADS_TYPE_BANNER = "banner",
		ADS_TYPE_INTERSTITIAL = "interstitial",
		ADS_TYPE_POPUP = "popup",
		ADS_TYPE_VIDEO = "video",
		ADS_TYPE_WIDGET = "widget",
		
		ADS_TYPE_REVMOB_BANNER = "Banner",
		ADS_TYPE_REVMOB_INTERSTITIAL = "fullscreen";

	public string
		admob,
		adbmob_inters;//,
		//tapgage;
	
	public AdvertisementManager.Apps
		adcolony,
		adcolony_zoneid,
		
		flurry,
		flurry_banner,
		flurry_inters,
		revmob,
		playhaven_inters,
		vungle,
	
		// Servicos default por atividade
		default_banner,
		default_interstitial,
		default_popup,
		default_video,
		default_widget;
		
	// Objetos dos servicos
	AdvertisementAdColony o_adcolony;
	AdvertisementAdMob o_admob;
	AdvertisementFlurry o_flurry;
	AdvertisementiAd o_iad;
	AdvertisementPlayHaven o_playhaven;
	AdvertisementRevMob o_revmob;
	//AdvertisementTapGage o_tapgage;
	AdvertisementVungle o_vungle;
	
	public string[]
		default_on_banners,
		default_on_interstitials,
		default_on_popups,
		default_on_videos,
		default_on_widgets,
		
		default_off_banners,
		default_off_interstitials,
		default_off_popups,
		default_off_videos,
		default_off_widgets;
	
	public string curEditorAds = "", curPreferenceAds = "", curListAds = "";
	
	public enum Positions {
		BOTTOM, TOP
	}
	
	public enum Networks {
		NONE, ADCOLONY, ADMOB, FLURRY, IAD, PLAYHAVEN, REVMOB, VUNGLE
	}
	
	[System.Serializable]
	public class Apps
	{
		public string android, ios;
		
		public string value
		{
			get
			{
#if UNITY_WEBPLAYER
				return null;
#elif UNITY_IPHONE 	
				return ios;
#elif UNITY_ANDROID
				return android;
#endif
			}
		}
	}
	
	// Lista de preferencias dos ads
	protected List<AdvertisementBase>
		banner,
		interstitial,
		popup,
		video,
		widget;
	
	void Awake()
	{		
		//DontDestroyOnLoad(gameObject);

		// Cria os objetos dos ads
		o_adcolony = new AdvertisementAdColony(adcolony.android, adcolony.ios, Info.version.ToString(), adcolony_zoneid.value);
		o_admob = new AdvertisementAdMob(admob, adbmob_inters);
		o_flurry = new AdvertisementFlurry(flurry.android, flurry.ios, flurry_banner.value, flurry_inters.value);
		o_iad = new AdvertisementiAd();
		o_playhaven = new AdvertisementPlayHaven(playhaven_inters.value);
        o_revmob = new AdvertisementRevMob(revmob.android, revmob.ios, gameObject.name);
		//o_tapgage = new AdvertisementTapGage(tapgage);
		o_vungle = new AdvertisementVungle(vungle.android, vungle.ios);
		
		// Faz a atribuicao do ultimo request salvo
		if (!defaultInternal())
			
		// Faz a atribuicao padrao
		defaultList();
		
		// Obtem a ordem dos ads com o servidor
		getAdList();
	}
		
	// Obtem a lista de ads com o servidor
	protected static void getAdList()
	{	
		if (!AdvertisementManager.isEnabled())
			return;
		
		GameConnection conn = new GameConnection(Flow.URL_BASE + "login/ads.php", AdvertisementManager.manager.handleGetAdList);
		
		WWWForm form = new WWWForm();
		form.AddField("app", Info.appId);

#if UNITY_ANDROID
		form.AddField("os", "android");
#elif UNITY_IPHONE
		form.AddField("os", "ios");
#else 
		form.AddField("os", "web");
#endif
		
		AdvertisementManager.manager.StartCoroutine(conn.startConnection(form));
	}
	
	protected void loadDefault(List<AdvertisementBase> list, AdvertisementManager.Apps ad)
	{
		if (list != null)
			return;
		
		list = new List<AdvertisementBase>(1);
		AdvertisementBase adsb = fromString(ad.value, ADS_TYPE_NONE);
		
		if (adsb != null)
			list.Add(adsb);
	}
	
	// Cria a lista default dos ads
	protected void defaultList()
	{
		if (banner == null) banner = new List<AdvertisementBase>();
		if (interstitial == null) interstitial = new List<AdvertisementBase>();
		if (popup == null) popup = new List<AdvertisementBase>();
		if (video == null) video = new List<AdvertisementBase>();
		if (widget == null) widget = new List<AdvertisementBase>();
		
		
		if (default_on_banners.Length > 0)
		{
			for (int i = 1; i < default_on_banners.Length; i++)
			{
				AdvertisementBase ad = fromString(default_on_banners[i], ADS_TYPE_NONE);
				if (ad != null && !banner.Contains(ad))
					banner.Add(ad);
			}	
		}
		
		if (default_on_interstitials.Length > 0)
		{
			for (int i = 1; i < default_on_interstitials.Length; i++)
			{
				//Debug.Log("ad "+i+" habilitado: "+default_on_interstitials[i]);
				AdvertisementBase ad = fromString(default_on_interstitials[i], ADS_TYPE_NONE);
				if (ad != null && !interstitial.Contains(ad))
					interstitial.Add(ad);
			}	
		}
		
		if (default_on_popups.Length > 0)
		{
			for (int i = 1; i < default_on_popups.Length; i++)
			{
				AdvertisementBase ad = fromString(default_on_popups[i], ADS_TYPE_NONE);
				if (ad != null && !popup.Contains(ad))
					popup.Add(ad);
			}	
		}
		
		if (default_on_videos.Length > 0)
		{
			for (int i = 1; i < default_on_videos.Length; i++)
			{
				AdvertisementBase ad = fromString(default_on_videos[i], ADS_TYPE_NONE);
				if (ad != null && !video.Contains(ad))
					video.Add(ad);
			}	
		}
		
		if (default_on_widgets.Length > 0)
		{
			for (int i = 1; i < default_on_widgets.Length; i++)
			{
				AdvertisementBase ad = fromString(default_on_widgets[i], ADS_TYPE_NONE);
				if (ad != null && !widget.Contains(ad))
					widget.Add(ad);
			}	
		}
		
		/*loadDefault(banner, default_banner);
		loadDefault(interstitial, default_interstitial);
		loadDefault(popup, default_popup);
		loadDefault(video, default_video);
		loadDefault(widget, default_widget);*/
	}
	
	protected string fixAdType(string ad)
	{
		return ad.ToLower();
	}
	
	protected void resetLists()
	{
		banner = 
		interstitial =
		popup =
		video =
		widget =
		null;
	}
	
	// Cria uma lista a partir do json
	protected List<AdvertisementBase> fromJson(IJSonObject data, string defaultAd)
	{
		if (data.Count == 0)
			return null;
		
		List<AdvertisementBase> adsList = new List<AdvertisementBase>();
		
		foreach (IJSonObject ad in data.ArrayItems)
		{
			AdvertisementBase curAd = fromString(ad.ToString(), defaultAd);
			
			if (ad != null && curAd != null)
				adsList.Add(curAd);
		}
		
		return adsList;
	}
	
	protected void handleGetAdList(string error, WWW conn)
	{
		// Reseta as listas
		resetLists();
		
		IJSonObject data = "{}".ToJSon();
			
		if (!conn.text.IsEmpty())
			data = conn.text.ToJSon();
		
		if (error != null || conn.error != null || data.IsEmpty() || data.IsError())
		{
			defaultList();
			return;
		}
				
		Save.Set("__ads:arr", data.ToString());
		
		// Salva as preferencias de cada ad
		banner = fromJson(data[ADS_TYPE_BANNER], default_banner.value);
		interstitial = fromJson(data[ADS_TYPE_INTERSTITIAL], default_interstitial.value);
		popup = fromJson(data[ADS_TYPE_POPUP], default_popup.value);
		video = fromJson(data[ADS_TYPE_VIDEO], default_video.value);
		widget = fromJson(data[ADS_TYPE_WIDGET], default_widget.value);
	}
	
	// Cria ads baseado no ultimo request salvoScene
	protected bool defaultInternal()
	{
		string last = Save.GetString("__ads:arr");
		IJSonObject data = last.ToJSon();
				
		if (data.IsEmpty() || data.IsError())
			return false;
		
		banner = fromJson(data[ADS_TYPE_BANNER], default_banner.value);
		interstitial = fromJson(data[ADS_TYPE_INTERSTITIAL], default_interstitial.value);
		popup = fromJson(data[ADS_TYPE_POPUP], default_popup.value);
		video = fromJson(data[ADS_TYPE_VIDEO], default_video.value);
		widget = fromJson(data[ADS_TYPE_WIDGET], default_widget.value);
		
		return true;
	}
	
	// Obtem a network a partir da sua string
	public AdvertisementBase fromString(string network, string defaultn)
	{
		AdvertisementManager.Networks n;
		
		try
		{
			n = (AdvertisementManager.Networks) System.Enum.Parse(typeof(AdvertisementManager.Networks), network, true);
		}
		catch (System.ArgumentException)
		{
			n = (AdvertisementManager.Networks) System.Enum.Parse(typeof(AdvertisementManager.Networks), defaultn, true);
		}
		
		switch (n)
		{
			case AdvertisementManager.Networks.ADCOLONY: return o_adcolony;
			case AdvertisementManager.Networks.ADMOB: return o_admob;
			case AdvertisementManager.Networks.FLURRY: return o_flurry;
			case AdvertisementManager.Networks.IAD: return o_iad;
			case AdvertisementManager.Networks.PLAYHAVEN: return o_playhaven;
            case AdvertisementManager.Networks.REVMOB: return o_revmob;
			//case AdvertisementManager.Networks.TAPGAGE: return o_tapgage;
			case AdvertisementManager.Networks.VUNGLE: return o_vungle;
			case AdvertisementManager.Networks.NONE: return null;
		}
		
		return null;
	}
	
	// Mostra o video
	public static void fetchVideo()
	{
		if (!AdvertisementManager.isEnabled()) return;
		
		if (Info.IsEditor() || AdvertisementManager.manager.video == null)
		{
			return;	
		}
		
		foreach (AdvertisementBase ad in AdvertisementManager.manager.video) ad.fetchVideo();
	}
	
	// Mostra o video
	public static bool showVideo()
	{
		if (!AdvertisementManager.isEnabled())
			return false;
		
		if (Info.IsEditor() || AdvertisementManager.manager.video == null)
			return false;

		foreach (AdvertisementBase ad in AdvertisementManager.manager.video)
			if (ad.showVideo())
				return true;
		
		return false;
	}
	
	public static bool isVideoAvailable()
	{
		if (!AdvertisementManager.isEnabled())
			return false;
		
		if (Info.IsEditor() || AdvertisementManager.manager.video == null)
			return false;

		foreach (AdvertisementBase ad in AdvertisementManager.manager.video)
			if (ad.isVideoAvailable())
				return true;
		
		return false;
	}
	
	// Mostra o popup
	public static bool showPopup()
	{
		/*if (Info.IsEditor())
		{
			GameGUI.game_native.showMessage(Info.name, "Do you want to download a FREE game?", "Yes, of course!");
			return true;
		}*/

		if (!AdvertisementManager.isEnabled() || AdvertisementManager.manager.popup == null)
			return false;
	
		foreach (AdvertisementBase ad in AdvertisementManager.manager.popup)
			if (ad.showPopup())
				return true;
		
		return false;
	}
	
	// Envia
	private void sendToCallback(System.Action<bool> on_showed, bool data)
	{
		if (on_showed != null)
			on_showed(data);
	}
	
	// Mostra o widget
	public static void showWidget(System.Action<bool> on_showed=null)
	{
		if (!AdvertisementManager.isEnabled())
			return;
		
		if (Info.IsEditor())
		{
			Application.OpenURL(AdvertisementManager.manager.urlToOpenOnEditor);
			return;
		}
		
		AdvertisementManager.manager.StartCoroutine(
			AdvertisementManager.manager.showWidgetCoroutine(on_showed)
		);
	}
	
	private IEnumerator showWidgetCoroutine(System.Action<bool> on_showed)
	{
		if (widget == null)
			yield break;
		
		foreach (AdvertisementBase ad in widget)
		{
			bool? showed = ad.showWidget();
			
			// Se o resultado foi conclusivo, termina a busca
			if (showed == true)
			{
				sendToCallback(on_showed, true);
				yield break;
			}
			
			// Tenta obter o resultado
			int i = 0;
			do
			{
				yield return new WaitForEndOfFrame();
				
				showed = ad.showedWidget();
				i++;
			}
			while (showed == null && i < 2 * AMOUNT_FRAMES_TO_WAIT);

			// Se conseguiu exibir, termina
			if (showed == true)
			{
				sendToCallback(on_showed, true);
				yield break;
			}
		}
		
		sendToCallback(on_showed, false);
	}
	
	// Obtem o interstitial
	public static void fetchInterstitial()
	{
		if (!AdvertisementManager.isEnabled())
			return;
		
		if (Info.IsEditor())
		{
			//AdvertisementEditorInterstitial.Fetch();
			return;
		}
		
		foreach (AdvertisementBase ad in AdvertisementManager.manager.interstitial)
		{

			ad.fetchInterstitial(true);
		}
	}
	
	// Mostra o interstitial
	public static void showInterstitial(System.Action<bool> on_showed=null)
	{
		if (!AdvertisementManager.isEnabled())
			return;
		
		if (Info.IsEditor())
		{
			//AdvertisementEditorInterstitial.Show();
			return;
		}

		AdvertisementManager.manager.StartCoroutine(
			AdvertisementManager.manager.showInterstitialCoroutine(on_showed)
		);
	}
	
	private IEnumerator showInterstitialCoroutine(System.Action<bool> on_showed)
	{
		foreach (AdvertisementBase ad in interstitial)
		{
			bool? showed = ad.showInterstitial();
			
			// Se o resultado foi conclusivo, termina a busca
			if (showed == true)
			{
				sendToCallback(on_showed, true);
				yield break;
			}
			
			// Tenta obter o resultado
			int i = 0;
			do
			{
				yield return new WaitForEndOfFrame();
				
				showed = ad.showedInterstitial();
				i++;
			}
			while (showed == null && i < AMOUNT_FRAMES_TO_WAIT);
			
			// Se conseguiu exibir, termina
			if (showed == true)
			{
				sendToCallback(on_showed, true);
				yield break;
			}
		}
		yield return true;
		sendToCallback(on_showed, false);
	}
	
	// Obtem o banner
	public static void fetchBanner(AdvertisementManager.Positions pos)
	{
		if (!AdvertisementManager.isEnabled())
			return;
		
		if (Info.IsEditor() || AdvertisementManager.manager.banner == null)
		{
			if (Info.IsEditor()) return;	
		}
		
		foreach (AdvertisementBase ad in AdvertisementManager.manager.banner)
			ad.fetchBanner(pos);
	}
	
	public static bool bannerEnabled = true;
	
	// Mostra o banner
	public static bool showBanner()
	{
		if (!bannerEnabled)
		{
			//Debug.Log("show return 1");
			return false;
		}
		if (totalBanners > 0)
		{
			//Debug.Log("show return 2");
			return true;
		}
		if (Info.IsEditor())
		{
			//Debug.Log("show return 3");
			return false;//AdvertisementEditorBanner.Show();
		}
		if (!AdvertisementManager.isEnabled() || isTryingBanner)
		{
			//Debug.Log("show return 4");
			return false;
		}
		
		AdvertisementManager.manager.StartCoroutine(AdvertisementManager.manager.showBannerCoroutine());
		
		return false;
	}
	
	public static int totalBanners
	{
		get
		{
			if (Info.IsEditor())
				return 0;

			int total = 0;
			
			if (AdvertisementManager.manager.banner != null)
			{
				foreach(AdvertisementBase ad in AdvertisementManager.manager.banner)
				{
					if (ad.showedBanner() == true) 
					{
						//Debug.Log("tipo do ad: "+ad.GetType());
						total++;
					}
				}
			}
			
			return total;
		}
	}
	
	private static bool isTryingBanner = false;
	
	private IEnumerator showBannerCoroutine()
	{
		if (banner == null || totalBanners > 0)
		{
			//Debug.Log("show return 5");
			yield break;
		}
		
		isTryingBanner = true;
		
		foreach (AdvertisementBase ad in banner)
		{
			if (totalBanners > 0)
			{
				isTryingBanner = false;
				//Debug.Log("show return 6");
				yield break;
			}
			
			bool? showed = ad.showBanner();
			
			// Se o resultado foi conclusivo, termina a busca
			if (showed == true)
			{
				isTryingBanner = false;
				//Debug.Log("show return 7");
				yield break;
			}
			
			// Tenta obter o resultado
			int i = 0;
			do
			{
				yield return new WaitForSeconds(DELAY_CONNECTION);
				
				if (totalBanners > 0)
				{
					isTryingBanner = false;
					//Debug.Log("show return 8");
					yield break;
				}
				
				showed = ad.showedBanner();
				i++;
			}
			while (showed == null && i < MAX_SINGLE_CONNECTION_TRIES);
			
			// Se conseguiu exibir, termina
			if (showed == true)
			{
				isTryingBanner = false;
				//Debug.Log("show 2");
				yield break;
			}
		}
		
		isTryingBanner = false;
	}
	
	// Esconde o banner
	public static void hideBanner()
	{
		//Debug.Log("hide 2");
		if (!AdvertisementManager.isEnabled() || Info.IsEditor())
		{
			//Debug.Log("hide 3");
			//if (Info.IsEditor()) 
			//{
				//AdvertisementEditorBanner.Hide();
				//Debug.Log("hide 4");
			//}
			return;
		}
		
		foreach (AdvertisementBase ad in AdvertisementManager.manager.banner)
		{
			//Debug.Log("hide 5");
			ad.hideBanner();
		}
	}
	
	// Callbacks do Revmob
	public void AdDidReceive(string type)
	{
		((AdvertisementRevMob) o_revmob).receivedWidget(true);
	}
	
	public void AdDidFail(string type)
	{
		AdvertisementRevMob revmob = ((AdvertisementRevMob) o_revmob);
		
		if (type == ADS_TYPE_REVMOB_INTERSTITIAL)
			revmob.receivedInterstitial(false);
		else if (type == ADS_TYPE_REVMOB_BANNER)
			revmob.receivedBanner(false);
		else revmob.receivedWidget(false);
	}
	
	public void AdDisplayed(string type)
	{
		AdvertisementRevMob revmob = ((AdvertisementRevMob) o_revmob);
			
		if (type == ADS_TYPE_REVMOB_BANNER)
			revmob.receivedBanner(true);
		else revmob.receivedInterstitial(true);
	}
	
	public void UserClickedInTheAd(string type) {}
	public void UserClosedTheAd(string type) {}
	public void InstallDidReceive(string type) {}
	public void InstallDidFail(string type) {}
	
	public static bool isEnabled()
	{	
		if (!AdvertisementManager.manager.isActive)
			return false;
		
		if (Info.IsEditor() || AdvertisementManager.manager != null)
			return true;
		
		GameObject gameAds = GameObject.Find(ConfigManager.API);
		
		if (gameAds && gameAds.GetComponent<AdvertisementManager>() && gameAds.GetComponent<AdvertisementManager>().isActive)
			return true;
		
		gameAds = GameObject.Find(AdvertisementManager.API);
		if (gameAds && gameAds.GetComponent<AdvertisementManager>() && gameAds.GetComponent<AdvertisementManager>().isActive)
			return true;
		
		return false;
	}
	
	void Update()
	{		
		// Se existir banners duplicados, remover o ultimo da lista de preferencia
		if (totalBanners > 1)
			for (int i = banner.Count - 1; i >= 0; i--)
				if (banner[i].showedBanner() == true)
				{
					banner[i].hideBanner();
					break;
				}
	}
	
	void WidgetCallback(string button)
	{
		if (button != "Yes, of course!")
			return;
		
		Application.OpenURL(AdvertisementManager.manager.urlToOpenOnEditor);
	}
	
	/*void OnEnable()
	{
		if (GameGUI.game_native != null) GameGUI.game_native.addActionShowMessage(WidgetCallback);
	}
	
    void OnDisable()
	{
		if (GameGUI.game_native != null) GameGUI.game_native.removeActionShowMessage(WidgetCallback);
	}*/
}
