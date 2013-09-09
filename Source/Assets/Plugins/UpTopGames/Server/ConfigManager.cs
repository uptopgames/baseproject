// Tem a funcao de inicializar objetos necessarios para o jogo, sem precisar de uma "buffer scene"
// Instancia SOMENTE o que é necessario para o jogo

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public class ConfigManager : MonoBehaviour
{
	public static UpdateOffline offlineUpdater = new UpdateOffline();
	
	protected const string
		// Mensagem a ser exibida caso nenhuma description seja setada pelo Servidor
		MESSAGE_VERSION_OLD	=	"You are running an old version. Please update!",
		
		// Botoes a serem exibidos ao solicitar Update
		BUTTON_UPDATE_YES	=	"Update!",
		BUTTON_UPDATE_NO	=	"Cancel!";
			
	public static string API = "#Config#";
	
	// Cache
	private static ConfigManager manager;
	
	// Variaveis pre-definidas, case ocorra algum erro e algo nao seja carregado
	public int appId = 0;
	
	public float
		appVersion = 1.0f,
		facebookDelayTime = 10f, cameraScale = 320f;
	
	public bool
		isSingleGame = false,
		showAds = false,
		showSavePrefs = false,
		showServerSettings = false;
	
	public string
		appName = "", appProtocol = "", firstScene = "", androidKey = "", androidKeyFull = "", androidKeyCustom1 = "", androidKeyCustom2 = "", androidKeyCustom3 = "",
		facebookId = "", facebookSecret = "", facebookCanvas = "", facebookPageId = "",
		serverSettingKey = "", deleteServerSetting = "", headerComponent = "",
		
		androidFreeBundle = "", androidPayBundle = "", androidCustom1Bundle = "", androidCustom2Bundle = "", androidCustom3Bundle = "",
		appleFreeBundle = "", applePayBundle = "", appleCustom1Bundle = "", appleCustom2Bundle = "", appleCustom3Bundle = "",
		appleFreeId = "", applePayId = "", appleCustom1Id = "", appleCustom2Id = "", appleCustom3Id = "",
	
		applePushFreeId = "", applePushPayId = "", applePushCustom1Id = "", applePushCustom2Id = "", applePushCustom3Id = "",
		androidPushFreeId = "", androidPushPayId = "", androidPushCustom1Id = "", androidPushCustom2Id = "", androidPushCustom3Id = "",
		androidPushFreePj = "", androidPushPayPj = "", androidPushCustom1Pj = "", androidPushCustom2Pj = "", androidPushCustom3Pj = "";
	
	public Info.AppType gameType = Info.AppType.Free;
	public Info.Enum.DebugType debugType = Info.Enum.DebugType.ShowOnDebug;
	public Info.Enum.DeleteType deleteType = Info.Enum.DeleteType.DeleteOnEditor;
	public Info.Enum.SandboxType sandboxType = Info.Enum.SandboxType.EnableOnDebug;

    public enum ShopType { StartByCode, StartOnLoadGame };
    public ShopType shopType = ShopType.StartByCode;
	
	public UnityEngine.Object headerObject = new UnityEngine.Object(), firstSceneObject = new UnityEngine.Object();
	
	// Classe de Settings do Servidor
	[System.Serializable]
	public class CachedServerSettings
	{
		public string key;
		public ServerSettings.Serializable setting;
	}
	
	// Arrays de Settings do Servidor
	// Utilizados pelo Editor, ao anterar alguma Setting
	public CachedServerSettings[] serverSettings;
	public ServerSettings.Serializable newSetting = new ServerSettings.Serializable("", ServerSettings.Type.String);
	public ServerSettings.Type oldSetting = ServerSettings.Type.String;
	
	void Awake ()
	{
		// Checa para nao criar outro quando entrar em uma cena que tenha o prefab de config
		GameObject[] configs = GameObject.FindGameObjectsWithTag("#Config#");
		foreach(GameObject g in configs)
		{
			if(gameObject != g)
			{
				GameObject.Destroy(gameObject);
				return;
			}
		}
		
		// Seta a prefab #Configuration# para nao ser destruida na troca de cenas
		DontDestroyOnLoad(gameObject);
		Flow.config = gameObject;
	}
	
	// Use this for initialization
	void Start ()
	{
		// Tenta autenticar usuario no GameCenter, caso necessario
		if (Info.IsPlatform(Info.Platform.iPhone) && Ranking.GameCenterAvailable()) Ranking.AuthenticateUser();
		
		if(Info.IsWeb())
		{
			if(gameObject.GetComponent(typeof(GamePersistentConnection))) gameObject.AddComponent<GamePersistentConnection>();
		}
		
        // Inicia o shop
        //if (shopType == ShopType.StartOnLoadGame){InApps.Register(gameObject);}
		
		// Se for a primeira vez o jogo esta sendo executado, broadcastAll com o método (void)OnFirstGameLoad()
		// E adiciona as Settings default salvas na Prefab, antes de tentar atualizar via Servidor
		/*if (!Save.GetBool("__first_load"))
		{
			//KGameObject.BroadcastAll("OnFirstGameLoad");
			Save.Set("__first_load", true);
			
			
		}*/
		for (int i = 0; i < serverSettings.Length; i++) ServerSettings.Add(serverSettings[i].key, serverSettings[i].setting);
		
		// Tenta atualizar as Settings pelo servidor
		ServerSettings.Load();
		// Fix crash da Shop forçando fetch do RevMob (interstitial)
#if !UNITY_WEBPLAYER
		Advertisement.Interstitial.Fetch();
#endif
		// Nao abre outras conexoes, caso seja executado pelo Editor (sem ir pela primeira scene)
		if (Application.loadedLevelName != "Init") return;
		
		
		// Se for Web (Facebook), informar que esta dentro do jogo a cada X segundos, para não spamar o usuario com notificacoes
		// ps: Precisa fazer o sistema em PHP, nao foi desenvolvido VER ISSO
        if (Info.IsWeb())
		{
			// Se logar pela Web, automaticamente desloga o usuario para solicitar Login pelo canvas
			Flow.OnLogoutFromServer();
			OnLogoutFromServer();
		}
		else 
		{
			StartCoroutine(CheckVersion());
		}
		
		// Inicializa métodos necessarios ao abrir o jogo
		// (load Prefabs, delete SavePrefs (se setado no config)
		//Initializate.Start(false);
		
		// Seta referencia da próxima scene, caso executado direto pela Login, no Editor
		// Up Top Fix Me
		//Scene.Login.SetNext(Info.firstScene);
		
		// Seta o som inicial para 50%
		// Up Top Fix Me
		//Sound.SetVolume(0.5f);
		
		// Se nao for um jogo offline (jogo aonde existe uma scene antes do Login)
		if (!Info.offlineGame)
		{
			// Verifica se o usuario ja esta logado
			if (PlayerPrefs.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
			{
				// Se ja esta logado, manda para a primeira scene
				// Sem precisar passar pelo Login
				// Up Top Fix Me
				//Scene.Splash.Load(Info.firstScene);
				return;
			}
		}
		
		// Redireciona para o usuario scene de Login
		//if (!Info.offlineGame) Scene.Login.SplashLoad(Info.firstScene);
		//else Scene.Splash.Load(Info.firstScene);
		
	}
	
	// Retorna instancia do ConfigManager, estaticamente
	private static ConfigManager GetConfig()
	{
		if (ConfigManager.manager != null)
			return ConfigManager.manager;
		
		GameObject obj = GameObject.Find(ConfigManager.API);
		if (obj == null) return default(ConfigManager);
		
		ConfigManager config = obj.GetComponent<ConfigManager>();
		if (config == null) return default(ConfigManager);
		
		ConfigManager.manager = config;
		
		return ConfigManager.manager;
	}
	
	// Retorna Settings a partir da instancia do ConfigManager
	public static void GetSettings()
	{
		ConfigManager config = ConfigManager.GetConfig();
		if (config == null || config == default(ConfigManager))
			return;
	}
	
	// Se for Web (Facebook) e o usuario estiver logado, informar que esta dentro do jogo a cada X segundos, para não spamar o usuario com notificacoes
	// ps: Precisa fazer o sistema em PHP, nao foi desenvolvido
	
	bool checkingVersion = false, isUpdatedVersion = false;
	
	// Checa se o usuario tem a versao atual do jogo
	IEnumerator CheckVersion()
	{
		if (!isUpdatedVersion)
		{
#if UNITY_IPHONE 
			//string platform = (Mobile.IsMobile()) ? Mobile.GetOS().ToString() : "";
			string platform = "Apple";
#elif UNITY_ANDROID
			string platform = "Android";
#else
			string platform = "";
#endif
						
			if (platform != "")
			{
				checkingVersion = true;
				WWW data = new WWW(Flow.URL_BASE + "login/version.php", new WWWForm().Add("platform", platform).Add("app_id", appId));
				yield return data;
				
				if (!Info.HasConnection() || data.error == null || data.text != "Version check failed!")
				{
					IJSonObject json = data.text.ToJSon();
					OnCheckVersion(null, json);
				}
				else StartCoroutine(CheckVersion());
			}
		}
		yield return true;
	}
	
	// Callback chamado ao receber as informacoes da versao atual, comparando com a ultima versao do jogo
	void OnCheckVersion(string error, IJSonObject data)
    {
		// Se o usuario ja estiver com a ultima versao do jogo, retornar
		if (isUpdatedVersion)
            return;
		
		// Se houver algum erro, tentar novamente e retornar
		if (error != null || data == null || data.IsEmpty() || data.IsError() || data.ToString() == "Version check failed!")
		{
			CheckVersion();
			return;
		}
		
		// Pega a ultima versao obrigatoria (force update) e a ultima versao em geral
		float
			last = data.Get("last").GetFloat("version"),
			update = data.Get("update").GetFloat("version");
		
		// Se a versao do usuario for menor que a versao obrigatoria, força usuario a atualizar
		if (appVersion < update)
		{
			string description = data.Get("update").GetString("description");
			// Up Top Fix Me
			//game_native.showMessage(appName, description, BUTTON_UPDATE_YES);
			return;
		}
		else
			isUpdatedVersion = true;
		
		// Se a versao do usuario for menor que a ultima versao, apenas perguntar se o usuario deseja atualizar
		if (appVersion < last)
		{
			string description = data.Get("update").GetString("description");
			
			// Up Top Fix Me
			//game_native.showMessageOkCancel(appName,(description != null && description != "") ? description: MESSAGE_VERSION_OLD,BUTTON_UPDATE_YES, BUTTON_UPDATE_NO);
		}
	}
	
	// Callback nativo chamado ao apertar algum botao das PopUps
	void NativeCallback(string button)
	{
		if (!checkingVersion)
			return;
		
		if (button == BUTTON_UPDATE_YES || button == BUTTON_UPDATE_NO)
		{
			checkingVersion = false;
			
	        if (button == BUTTON_UPDATE_YES)
			{
				//Mobile.AppUrl(Info.bundle, Info.appleId, true);
#if UNITY_IPHONE
				Application.OpenURL ("itms-apps: //ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewSoftware&id=" + Info.appleId);
#elif UNITY_ANDROID
				Application.OpenURL ("market://details?id=" + Info.bundle);
#endif
			
			}
			
			
		}
	}
	
	// Callback nativo das PopUps, utilizado para checagens de versionamento
	void OnEnable()
	{
		// Up Top Fix Me
		//if (game_native != null)
			//game_native.addActionShowMessage(NativeCallback);
	}
	
    void OnDisable()
	{
		// Up Top Fix Me
		//if (game_native != null)
			//game_native.removeActionShowMessage(NativeCallback);
	}
	
	void Update()
	{
		// Se o usuario estiver jogando pela Web e ainda não estiver logado,
		// Instancia script de login pelo Canvas do Facebook
		
		if (!Info.IsWeb()) return;
				
		GameObject apiUp = GameObject.Find(ConfigManager.API);
		
		if (apiUp == null) return;
				
		if (!apiUp.GetComponent<GameBrowserConnection>()) 
		{
			apiUp.AddComponent<GameBrowserConnection>();
		}
	}
	
	// Informacoes default salvas no SavePrefs que deletadas automaticamente, ao deslogar o usuario do jogo
	private PlayerPrefsKeys[] saveKeys = new PlayerPrefsKeys[]
	{
		// Keys global de token
		// Ao deslogar, todos os Tokens se tornam invalidos
		PlayerPrefsKeys.TOKEN,
		PlayerPrefsKeys.TOKEN_EXPIRATION,
		PlayerPrefsKeys.FACEBOOK_TOKEN,
		PlayerPrefsKeys.NAME,
		PlayerPrefsKeys.ID,
		
		// Keys da scene de Settings
		// Remover para serem re-solicitadas, ao entrar na scene novamente
		PlayerPrefsKeys.EMAIL,
		PlayerPrefsKeys.PASSWORD,
		PlayerPrefsKeys.FACEBOOK_ID,
		
		PlayerPrefsKeys.FIRST_NAME,
		PlayerPrefsKeys.LAST_NAME,
		
		PlayerPrefsKeys.LOCATION,
		PlayerPrefsKeys.GENDER,
		
		PlayerPrefsKeys.DATE_DAY,
		PlayerPrefsKeys.DATE_MONTH,
		PlayerPrefsKeys.DATE_YEAR,
	};
	
	// Ao deslogar o usuario do jogo, remover as informacoes (somente o necessario) salvas no SavePrefs
	void OnLogoutFromServer()
	{
		foreach(PlayerPrefsKeys key in saveKeys) Save.Delete(key.ToString());
		
		Save.SaveAll();
	}
	
	
	// TODAS FUNCOES ABAIXO SAO UTILIZADAS SOMENTE PARA CONFIGURAR A PREFAB NO EDITOR
	// NENHUMA DAS FUNCOES ABAIXO SAO UTILIZADAS DURANTE O GAMEPLAY
	public string GetAppleId()
	{
		if (gameType == Info.AppType.Free)
			return appleFreeId;
		else if (gameType == Info.AppType.Pay)
			return applePayId;
		else if (gameType == Info.AppType.Custom1)
			return appleCustom1Id;
		else if (gameType == Info.AppType.Custom2)
			return appleCustom2Id;
		else if (gameType == Info.AppType.Custom3)
			return appleCustom3Id;
		
		return default(string);
	}
	
	public void SetAppleId(string id)
	{
		if (gameType == Info.AppType.Free)
			this.appleFreeId = id;
		else if (gameType == Info.AppType.Pay)
			this.applePayId = id;
		else if (gameType == Info.AppType.Custom1)
			this.appleCustom1Id = id;
		else if (gameType == Info.AppType.Custom2)
			this.appleCustom2Id = id;
		else if (gameType == Info.AppType.Custom3)
			this.appleCustom3Id = id;
	}
	
	public string GetAppleBundleId()
	{
		if (gameType == Info.AppType.Free)
			return appleFreeBundle;
		else if (gameType == Info.AppType.Pay)
			return applePayBundle;
		else if (gameType == Info.AppType.Custom1)
			return appleCustom1Bundle;
		else if (gameType == Info.AppType.Custom2)
			return appleCustom2Bundle;
		else if (gameType == Info.AppType.Custom3)
			return appleCustom3Bundle;
		
		return default(string);
	}
	
	public void SetAppleBundleId(string bundle)
	{
		if (gameType == Info.AppType.Free)
			this.appleFreeBundle = bundle;
		else if (gameType == Info.AppType.Pay)
			this.applePayBundle = bundle;
		else if (gameType == Info.AppType.Custom1)
			this.appleCustom1Bundle = bundle;
		else if (gameType == Info.AppType.Custom2)
			this.appleCustom2Bundle = bundle;
		else if (gameType == Info.AppType.Custom3)
			this.appleCustom3Bundle = bundle;
	}
	
	public string GetAndroidBundleId()
	{
		if (gameType == Info.AppType.Free)
			return androidFreeBundle;
		else if (gameType == Info.AppType.Pay)
			return androidPayBundle;
		else if (gameType == Info.AppType.Custom1)
			return androidCustom1Bundle;
		else if (gameType == Info.AppType.Custom2)
			return androidCustom2Bundle;
		else if (gameType == Info.AppType.Custom3)
			return androidCustom3Bundle;
		
		return default(string);
	}
	
	public string GetAndroidKey()
	{
		if (gameType == Info.AppType.Free)
			return androidKey;
		else if (gameType == Info.AppType.Pay)
			return androidKeyFull;
		else if (gameType == Info.AppType.Custom1)
			return androidKeyCustom1;
		else if (gameType == Info.AppType.Custom2)
			return androidKeyCustom2;
		else if (gameType == Info.AppType.Custom3)
			return androidKeyCustom3;
		
		return default(string);
	}
	
	public void SetAndroidKey(string key)
	{
		if (gameType == Info.AppType.Free)
			this.androidKey = key;
		else if (gameType == Info.AppType.Pay)
			this.androidKeyFull = key;
		else if (gameType == Info.AppType.Custom1)
			this.androidKeyCustom1 = key;
		else if (gameType == Info.AppType.Custom2)
			this.androidKeyCustom2 = key;
		else if (gameType == Info.AppType.Custom3)
			this.androidKeyCustom3 = key;
	}
	
	public void SetAndroidBundleId(string bundle)
	{
		if (gameType == Info.AppType.Free)
			this.androidFreeBundle = bundle;
		else if (gameType == Info.AppType.Pay)
			this.androidPayBundle = bundle;
		else if (gameType == Info.AppType.Custom1)
			this.androidCustom1Bundle = bundle;
		else if (gameType == Info.AppType.Custom2)
			this.androidCustom2Bundle = bundle;
		else if (gameType == Info.AppType.Custom3)
			this.androidCustom3Bundle = bundle;
	}
	
	public string GetApplePushId()
	{
		if (gameType == Info.AppType.Free)
			return applePushFreeId;
		else if (gameType == Info.AppType.Pay)
			return applePushPayId;
		else if (gameType == Info.AppType.Custom1)
			return applePushCustom1Id;
		else if (gameType == Info.AppType.Custom2)
			return applePushCustom2Id;
		else if (gameType == Info.AppType.Custom3)
			return applePushCustom3Id;
		
		return default(string);
	}
	
	public void SetApplePushId(string id)
	{
		if (gameType == Info.AppType.Free)
			this.applePushFreeId = id;
		else if (gameType == Info.AppType.Pay)
			this.applePushPayId = id;
		else if (gameType == Info.AppType.Custom1)
			this.applePushCustom1Id = id;
		else if (gameType == Info.AppType.Custom2)
			this.applePushCustom2Id = id;
		else if (gameType == Info.AppType.Custom3)
			this.applePushCustom3Id = id;
	}
	
	public string GetAndroidPushId()
	{
		if (gameType == Info.AppType.Free)
			return androidPushFreeId;
		else if (gameType == Info.AppType.Pay)
			return androidPushPayId;
		else if (gameType == Info.AppType.Custom1)
			return androidPushCustom1Id;
		else if (gameType == Info.AppType.Custom2)
			return androidPushCustom2Id;
		else if (gameType == Info.AppType.Custom3)
			return androidPushCustom3Id;
		
		return default(string);
	}
	
	public void SetAndroidPushId(string id)
	{
		if (gameType == Info.AppType.Free)
			this.androidPushFreeId = id;
		else if (gameType == Info.AppType.Pay)
			this.androidPushPayId = id;
		else if (gameType == Info.AppType.Custom1)
			this.androidPushCustom1Id = id;
		else if (gameType == Info.AppType.Custom2)
			this.androidPushCustom2Id = id;
		else if (gameType == Info.AppType.Custom3)
			this.androidPushCustom3Id = id;
	}
	
	public string GetAndroidProjectId()
	{
		if (gameType == Info.AppType.Free)
			return androidPushFreePj;
		else if (gameType == Info.AppType.Pay)
			return androidPushPayPj;
		else if (gameType == Info.AppType.Custom1)
			return androidPushCustom1Pj;
		else if (gameType == Info.AppType.Custom2)
			return androidPushCustom2Pj;
		else if (gameType == Info.AppType.Custom3)
			return androidPushCustom3Pj;
		
		return default(string);
	}
	
	public void SetAndroidProjectId(string id)
	{
		if (gameType == Info.AppType.Free)
			this.androidPushFreePj = id;
		else if (gameType == Info.AppType.Pay)
			this.androidPushPayPj = id;
		else if (gameType == Info.AppType.Custom1)
			this.androidPushCustom1Pj = id;
		else if (gameType == Info.AppType.Custom2)
			this.androidPushCustom2Pj = id;
		else if (gameType == Info.AppType.Custom3)
			this.androidPushCustom3Pj = id;
	}
}
