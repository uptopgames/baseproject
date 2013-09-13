using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
	
public class Game
{			
	public int id = -1;
	public Friend friend = new Friend();
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
		this.id = -1;
		this.friend = new Friend();
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
	
	public Game(int id, Friend friend, int themeID, 
		string themeName, string mapCode, List<Locale> localeList, List<Guess> myList = null, List<Guess> theirList = null, 
		int turnID = -1, int lastTurnID = -1, int turnsWon = -1, int turnsLost = -1, string whoseMove = null, string status = null, DateTime? time = null, 
		List<Locale> pastLocaleList = null, List<Guess> myPastList = null, List<Guess> theirPastList = null, string pastThemeName = null)
	{
		this.id = id;
		this.friend = friend;
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
	
	public static void EndGame()
	{
		Flow.currentGame = new Game();
		Flow.selectedListIndex = -1;
	}
	
	public static void ExitMode()
	{
		Flow.currentMode = GameMode.None;
	}
	
	public static void Reset()
	{
		//Debug.Log("deletando tudo...");
		Flow.currentGame = new Game();
		Flow.selectedListIndex = -1;
		Flow.currentMode = GameMode.None;
		Flow.path = "";
		Flow.thereIsAnotherPlayer = false;
	}
}