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
	APP_ID = 15
}

public class Flow: MonoBehaviour
{
#if UNITY_IPHONE || UNITY_WEBPLAYER
	public static string URL_BASE = "https://uptopgames.com/";
#elif UNITY_ANDROID
	public static string URL_BASE = "http://uptopgames.com/";
#endif
	
	private static GameObject _config;
	public static GameObject config
	{
		get
		{
			if(_config == null)
			{
				_config = GameObject.FindGameObjectWithTag("#Config#");
			}
			return _config;
		}
		set
		{
			_config = value;
		}
	}
	
	
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
	public static int CITIES_PER_LEVEL = 5;
	
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
	
	public class Guess
	{
		public int guessID;	
		public int turnID;
		public int userID;
		public int localeID;
		public float latitude;
		public float longitude;
		public float distance;
		public float time;
		public int score;
		
		public Guess(int id, int turn, int user, int locale, float latitude, float longitude, float distance, float time, int score)
		{
			this.guessID = id;
			this.turnID = turn;
			this.userID = user;
			this.localeID = locale;
			this.latitude = latitude;
			this.longitude = longitude;
			this.distance = distance;
			this.time = time;
			this.score = score;
		}
	}
	
	public class Game
	{
		public static string[] availableMaps = { "usa", "brazil", "uk", "southafrica", "world", "china", "france", "australia" };
		
		public static string path = "";
		public static bool thereIsAnotherPlayer = false;
		
		public static List<Game> gameList = new List<Game>();
		public static Game currentGame = new Game();
		public static GameMode currentMode = GameMode.None;
		public static int selectedListIndex = -1;
				
		public int id = -1;
		public int friendID = -1;
		public string friendName = "";
		public string facebookID = "";
		public Texture2D friendPhoto;
		public bool isDownloadingPhoto = false;
		public bool hasApp = false;
		public List<Guess> myGuessList = new List<Guess>();
		public List<Guess> theirGuessList = new List<Guess>();
		public List<Locale> localeList = new List<Locale>();
		public List<Locale> pastLocaleList = new List<Locale>();
		public List<Guess> pastMyGuessList = new List<Guess>();
		public List<Guess> pastTheirGuessList = new List<Guess>();
		public string pastThemeName = "";
		public int themeID = -1;
		public string themeName = "";
		public string mapCode = "";
		public int turnID = 0;
		public int lastTurnID = 0;
		public int turnsWon = 0;
		public int turnsLost = 0;
		public string whoseMove = "";
		public string status = "";
		public DateTime lastUpdate = new DateTime(1970,1,1);
		public int myTotalScore = 0;
		public int theirTotalScore = 0;
		
		public Game()
		{
			this.friendID = -1;
			this.id = -1;
			this.friendName = "";
			this.hasApp = false;
			this.facebookID = "";
			this.myGuessList = new List<Guess>();
			this.theirGuessList = new List<Guess>();
			this.localeList = new List<Locale>();
			this.themeID = -1;
			this.themeName = "";
			this.mapCode = "";
			this.turnID = 0;
			this.lastTurnID = 0;
			this.turnsWon = 0;
			this.turnsLost = 0;
			this.whoseMove = "";
			this.status = "";
			this.lastUpdate = new DateTime(1970,1,1);
			this.pastMyGuessList = new List<Guess>();
			this.pastTheirGuessList = new List<Guess>();
			this.pastLocaleList = new List<Locale>();
			this.pastThemeName = "";
		}
		public Game(int id, int friendID, string friendName, bool hasApp, string facebookID, int themeID, 
			string themeName, string mapCode, List<Locale> localeList, List<Guess> myList = null, List<Guess> theirList = null, 
			int turnID = -1, int lastTurnID = -1, int turnsWon = -1, int turnsLost = -1, string whoseMove = null, string status = null, DateTime? time = null, 
			List<Locale> pastLocaleList = null, List<Guess> myPastList = null, List<Guess> theirPastList = null, string pastThemeName = null)
		{
			this.id = id;
			this.friendID = friendID;
			this.friendName = friendName;
			this.hasApp = hasApp;
			this.facebookID = facebookID;
			this.localeList = localeList;
			this.myGuessList = myList;
			this.theirGuessList = theirList;
			this.themeID = themeID;
			this.themeName = themeName;
			this.mapCode = mapCode;
			this.turnID = turnID;
			this.lastTurnID = lastTurnID;
			this.turnsWon = turnsWon;
			this.turnsLost = turnsLost;
			this.whoseMove = whoseMove;
			this.status = status;
			this.lastUpdate = (time == null)? new DateTime(0) : (DateTime) time;
			this.pastMyGuessList = myPastList;
			this.pastTheirGuessList = theirPastList;
			this.pastLocaleList = pastLocaleList;
			this.pastThemeName = pastThemeName;
		}
		
		public void getPhoto(string error, WWW data)
	    {
	        isDownloadingPhoto = false;
	        if (error != null)
	        {
	            if (error.IndexOf("404") >= 0)
	            {
					Texture2D tx = new Texture2D(1,1);
					tx.SetPixel(0,0,Color.clear);
					tx.Apply();
	                friendPhoto = tx;
	            }
	        }
	        else
	        {
	            friendPhoto = data.texture;
	        }
	    }
		
		public static void EndGame()
		{
			currentGame = new Game();
			selectedListIndex = -1;
		}
		
		public static void ExitMode()
		{
			currentMode = GameMode.None;
		}
		
		public static void Reset()
		{
			//Debug.Log("deletando tudo...");
			currentGame = new Game();
			selectedListIndex = -1;
			currentMode = GameMode.None;
			path = "";
			thereIsAnotherPlayer = false;
		}
		
	}
	
	public static void Reset()
	{
		//playerName = "";
		playerPhoto = null;
		lastUpdate = new DateTime(1970,1,1);
		Flow.Game.Reset();
		Game.gameList = new List<Game>();
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
		
		Header.coins = 0;
	}
}
