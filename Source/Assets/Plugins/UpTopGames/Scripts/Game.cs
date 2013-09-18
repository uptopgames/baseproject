using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public int id = -1;
	public int pastIndex = -1;
	public bool isNewGame = false;
	public Friend friend = new Friend();
	public List<Round> myRoundList = new List<Round>();
	public List<Round> theirRoundList = new List<Round>();
	public List<Round> pastMyRoundList = new List<Round>();
	public List<Round> pastTheirRoundList = new List<Round>();
	public string pastWorldName = "";
	public int worldID = -1;
	public string worldName = "";
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
		this.myRoundList = new List<Round>();
		this.theirRoundList = new List<Round>();
		this.worldID = -1;
		this.worldName = "";
		this.turnID = 0;
		this.lastTurnID = 0;
		this.turnsWon = 0;
		this.turnsLost = 0;
		this.whoseMove = "";
		this.status = "";
		this.lastUpdate = new DateTime(1970,1,1);
		this.pastMyRoundList = new List<Round>();
		this.pastTheirRoundList = new List<Round>();
		this.pastWorldName = "";
	}
	
	public Game(int id, Friend friend, int worldID, 
		string worldName, List<Round> myList = null, List<Round> theirList = null, 
		int turnID = -1, int lastTurnID = -1, int turnsWon = -1, int turnsLost = -1, string whoseMove = null, string status = null, DateTime? time = null, 
		List<Round> myPastList = null, List<Round> theirPastList = null, string pastWorldName = null)
	{
		this.id = id;
		this.friend = friend;
		this.myRoundList = myList;
		this.theirRoundList = theirList;
		this.worldID = worldID;
		this.worldName = worldName;
		this.turnID = turnID;
		this.lastTurnID = lastTurnID;
		this.turnsWon = turnsWon;
		this.turnsLost = turnsLost;
		this.whoseMove = whoseMove;
		this.status = status;
		this.lastUpdate = (time == null)? new DateTime(0) : (DateTime) time;
		this.pastMyRoundList = myPastList;
		this.pastTheirRoundList = theirPastList;
		this.pastWorldName = pastWorldName;
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
	
	public void SetGame(Game game)
	{
		Game tempGame = new Game(game.id, game.friend, game.worldID, game.worldName, game.myRoundList, game.theirRoundList, game.turnID, game.lastTurnID,
			game.turnsWon, game.turnsLost, game.whoseMove, game.status, game.lastUpdate, game.pastMyRoundList, game.pastTheirRoundList, game.pastWorldName);
		
		id = game.id;
		friend = game.friend;
		worldID = game.worldID;
		worldName = game.worldName;
		myRoundList = game.myRoundList;
		theirRoundList = game.theirRoundList;
		turnID = game.turnID;
		lastTurnID = game.lastTurnID;
		turnsWon = game.turnsWon;
		turnsLost = game.turnsLost;
		whoseMove = game.whoseMove;
		status = game.status;
		lastUpdate = game.lastUpdate;
		pastMyRoundList = game.pastMyRoundList;
		pastTheirRoundList = game.pastTheirRoundList;
		pastWorldName = game.pastWorldName;
	}
	
	public void AnswerGame()
	{
		Flow.currentGame = this;
		Debug.Log("ID: " + Flow.currentGame.id.ToString() + "\nPast Index: " + Flow.currentGame.pastIndex.ToString() + 
			"\nIs New Game: " + Flow.currentGame.isNewGame.ToString() + "\nFriend: " + Flow.currentGame.friend.ToString() + 
			"\nMy Round List: " + Flow.currentGame.myRoundList.ToString() + "\nTheir Round List: " + Flow.currentGame.theirRoundList.ToString() + 
			"\nPast My Round List: " + Flow.currentGame.pastMyRoundList.ToString() + "\nPast Their Round List: " +
			Flow.currentGame.pastTheirRoundList.ToString() + "\nPast World Name: " + Flow.currentGame.pastWorldName.ToString() + 
			"\nWorld ID: " + Flow.currentGame.worldID.ToString() + "\nWorld Name: " + Flow.currentGame.worldName.ToString() + "\nTurn ID: " + 
			Flow.currentGame.turnID.ToString() + "\nLast Turn ID: " + Flow.currentGame.lastTurnID.ToString() + "\nTurns Won: " + 
			Flow.currentGame.turnsWon.ToString() + "\nTurns Lost: " + Flow.currentGame.turnsLost.ToString() + "\nWhose Move: " + 
			Flow.currentGame.whoseMove.ToString() + "\nStatus: " + Flow.currentGame.status.ToString() + "\nLast Update: " + 
			Flow.currentGame.lastUpdate.ToString() + "\nMy Total Score: " + Flow.currentGame.myTotalScore.ToString() + "\nTheir Total Score: " +
			Flow.currentGame.theirTotalScore.ToString());
		
		for(int i = 0; i<Flow.currentGame.theirRoundList.Count; i++)
		{
			Debug.Log(Flow.currentGame.theirRoundList[i].score);
		}
		
		UIPanelManager.instance.BringIn("WorldSelectionScenePanel");
	}
}