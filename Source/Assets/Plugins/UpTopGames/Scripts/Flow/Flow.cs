using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public enum GameMode { Multiplayer, SinglePlayer, None }

public enum PlayerPrefsKeys
{
	TOKEN = 0,
	TOKEN_EXPIRATION = 1,
	FACEBOOK_TOKEN = 2,
	NAME = 3,
	ID = 4,
	EMAIL = 5,
	PASSWORD = 6,
	FACEBOOK_ID = 7,
	FIRST_NAME = 8,
	LAST_NAME = 9,
	LOCATION = 10,
	GENDER = 11,
	DATE_DAY = 12,
	DATE_MONTH = 13,
	DATE_YEAR = 14,
	APP_ID = 15,
	COINS = 16,
	NOADS = 17
}

public enum PanelToLoad
{
	Menu,
	WinLose,
	BattleStatus
}

public enum TurnStatus
{
	BeginGame, AnswerGame, ShowPast
}

public class Flow: MonoBehaviour
{
	public static GameNativeGUI game_native = new GameNativeGUI();
	
	
#if UNITY_IPHONE || UNITY_WEBPLAYER
	public static string URL_BASE = "https://uptopgames.com/";
#elif UNITY_ANDROID
	public static string URL_BASE = "http://uptopgames.com/";
#endif
	
	
	
	private static Header _header;
	public static Header header
	{
		get
		{
			if(_header == null)
			{
				_header = config.GetComponent<ConfigManager>().headerObject.GetComponent<Header>();
			}
			return _header;
		}
	}
	
	//public static Header header;
		
	private static GameObject _loadingDialog;
	public static GameObject loadingDialog
	{
		get
		{
			if(_loadingDialog == null)
			{
				_loadingDialog = config.GetComponent<ConfigManager>().loading;
			}
			return _loadingDialog;
		}
	}
	
	private static GameObject _messageOkDialog;
	public static GameObject messageOkDialog
	{
		get
		{
			if(_messageOkDialog == null)
			{
				_messageOkDialog = config.GetComponent<ConfigManager>().messageOk;
			}
			return _messageOkDialog;
		}
	}
	
	private static GameObject _messageOkCancelDialog;
	public static GameObject messageOkCancelDialog
	{
		get
		{
			if(_messageOkCancelDialog == null)
			{
				_messageOkCancelDialog = config.GetComponent<ConfigManager>().messageOkCancel;
			}
			return _messageOkCancelDialog;
		}
	}
	
	private static GameObject _config;
	public static GameObject config
	{
		get
		{
			if(_config == null)
			{
				_config = GameObject.FindGameObjectWithTag(ConfigManager.API);
			}
			return _config;
		}
	}
	
	private static ShopManager _shopManager;
	public static ShopManager shopManager
	{
		get
		{
			if(_shopManager == null)
			{
				_shopManager = config.GetComponent<ShopManager>();
			}
			return _shopManager;
		}
	}
	
	//public static GameObject config;
	//public static GameObject messageOkCancelDialog;
	//public static GameObject messageOkDialog;
	//public static GameObject loadingDialog;
	//public static ShopManager shopManager;
	
	//variáveis que estavam dentro de Game
	public static string[] availableMaps = { "usa", "brazil", "uk", "southafrica", "world", "china", "france", "australia" };
	
	public static PanelToLoad nextPanel = PanelToLoad.Menu;
	
	public static List<Game> gameList = new List<Game>();
	//public static List<Texture> gamePictures = new List<Texture>();
	public static int yourTurnGames = 0;
	public static int theirTurnGames = 0;
	public static Game currentGame = new Game();
	public static GameMode currentMode = GameMode.None;
	public static int selectedListIndex = -1;
	
	public static TurnStatus path = TurnStatus.BeginGame;
	
	// setar aqui o nome do arquivo xml a ser usado, se houver necessidade de mudar
	public static string xmlFileName = "gameData.xml";
	public static Texture2D playerPhoto = null;
	public static int gamesPassedMulti = 0;
	public static int gamesPassedSingle = 0;
	public static bool isDownloadingPlayerPhoto = false;
	public static DateTime lastUpdate = new DateTime(1970,1,1);
	public static int selectedWorldID = -1;
	
	public static Dictionary<int,World> worldDict = new Dictionary<int,World>();
	public static DateTime gameDataLastDate = new DateTime(1970,1,1);
	
	public static List<Theme> localThemes = new List<Theme>();
	public static List<List<Locale>> currentLevelList = new List<List<Locale>>();
	public static Dictionary<string,List<Locale>> levelDict = new Dictionary<string, List<Locale>>();
	public static int currentLevel;
	public static int currentScore;
	public static int MAX_LEVEL_NUMBER = 10;
	public const int ROUNDS_PER_TURN = 3;
	
	public static float soundVolume = 0.5f;
	
	public static void getPlayerPhoto(string error, WWW data)
    {
        Flow.isDownloadingPlayerPhoto = false;
        if (error != null)
        {
            if (error.IndexOf("404") >= 0)
            {
				Texture2D tx = new Texture2D(1,1);
				tx.SetPixel(0,0,Color.clear);
				tx.Apply();
                Flow.playerPhoto = tx;
            }
        }
        else
        {
            Flow.playerPhoto = data.texture;
        }
    }
	
	
	
	public static void Reset()
	{
		//playerName = "";
		playerPhoto = null;
		lastUpdate = new DateTime(1970,1,1);
		Game.Reset();
		gameList = new List<Game>();
	}
	
	public static void OnLogoutFromServer()
	{
		// Deletar aqui todas as suas keys do Save que devem ser deletadas quando um usuario da logout
		
		Debug.Log("Execute flow! (OnLogoutFromServer)");
		Flow.Reset();
		
		Debug.Log("ct"+localThemes.Count);
		for(int i = 0 ; i < 100 ; i++)
		{
			//Debug.Log("zabumba"+i);
			Save.Delete("purchasedTheme"+i);
			Save.Delete("datePurchasedTheme"+i);
		}
		
		Save.Delete("FeatureInvite");
		Save.Delete("FeatureLike");
		Save.Delete("FeatureVideo");
		Save.Delete("FeatureRate");
		Save.Delete("FeatureWidget");
		Save.Delete("FeatureShare");
		Save.Delete("userFeatures");
		Save.Delete("purchasedItems");
		Save.Delete("datePurchasedItems");
		
		Save.SaveAll();
		
		// Zerar coins
	}
}

public class World
{
	public Texture2D image;
	public bool isDownloading = false;
	public string imageName;
	public Dictionary<int,Level> levelDict = new Dictionary<int,Level>();
	public string appleBundle;
	public string androidBundle;
	public int id;
	public string name;
	public int enemiesToUnlock;
	public DateTime lastUpdate;
	
	public World(int id, string name, int enemiesToUnlock, string appleBundle, string androidBundle, string imageName, DateTime lastUpdate)
	{
		this.id = id;
		this.name = name;
		this.appleBundle = appleBundle;
		this.androidBundle = androidBundle;
		this.imageName = imageName;
		this.lastUpdate = lastUpdate;
		this.enemiesToUnlock = enemiesToUnlock;
	}
}

public class Level
{
	public int id = 0;
	public string name = "";
	public int enemies = 0;
	public float time = 0f;
	public DateTime lastUpdate = new DateTime(1);
	public Texture2D image;
	public bool isDownloading = false;
	
	public Level(int id, string name, int enemies, float time, DateTime lastUpdate)
	{
		this.id = id;
		this.name = name;
		this.enemies = enemies;
		this.time = time;
		this.lastUpdate = lastUpdate;
	}
}

[System.Serializable]
public class Theme
{
	public int id;
	public string name;
	public string code;
	public int tier;
	public int parentTheme;
	public int pointsToUnlock;
	public int themeGroup;
	public Texture2D picture;
	public string appleBundle;
	public string androidBundle;
	public bool isDownloading = false;
	
	public Theme(int themeID, string themeName, string mapCode, int tGroup, int priceTier, int pTheme, int points, string apple, string android)
	{
		this.id = themeID;
		this.name = themeName;
		this.code = mapCode;
		this.tier = priceTier;
		this.themeGroup = tGroup;
		this.parentTheme = pTheme;
		this.pointsToUnlock = points;
		this.appleBundle = apple;
		this.androidBundle = android;
	}
}

[System.Serializable]
public class Locale
{
	public int localeID;
	public Theme localeTheme;
	public float latitude;
	public float longitude;
	public string localeName;
	public DateTime date;
	
	public Locale(int id, string name, Theme theme, float latitude, float longitude, DateTime date)
	{
		this.localeID = id;
		this.localeName = name;
		this.localeTheme = theme;
		this.latitude = latitude;
		this.longitude = longitude;
		this.date = date;
		
	}
}