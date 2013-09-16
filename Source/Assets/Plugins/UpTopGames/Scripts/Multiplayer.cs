using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Multiplayer : MonoBehaviour 
{
	public UIPanelManager panelManager;
	public GameObject loadingDialog;
	public GameObject messageOkDialog;
	public GameObject messageOkCancelDialog;
	
	// Tempo de espera entre consultas
	private const float MIN_WAITING_TIME = 4;
	private const float MAX_WAITING_TIME = 30;
	private const float START_WAITING_TIME = 10;
	private const float RATIO_WAITING_TIME = 1.25f;
	public static float waitingTime = START_WAITING_TIME;
	public static bool updatedAutomatically = false;
	
	// Facebook
	GameFacebook fb_account;
	
	// Use this for initialization
	void Start () 
	{
		WWWForm form = new WWWForm();
		form.AddField("lastUpdate",TimeZoneInfo.ConvertTimeToUtc(Flow.lastUpdate).ToString());
		new GameJsonAuthConnection(Flow.URL_BASE + "base/getgames.php", OnReceiveGames).connect(form);
	}
	
	void updatingAutomatically()
	{
		updatedAutomatically = true;
		
		WWWForm form = new WWWForm();
		form.AddField("lastUpdate",TimeZoneInfo.ConvertTimeToUtc(Flow.lastUpdate).ToString());
		new GameJsonAuthConnection(Flow.URL_BASE + "base/getgames.php", OnReceiveGames).connect(form);
	}
	
	void OnReceiveGames(string error, IJSonObject data)
	{
		if(error != null) Debug.Log(error);
		else 
		{
			Debug.Log(data);
						
			for(int i = 0 ; i < data["games"].Count ; i++)
			{
				if(!data["games"][i]["lastUpdate"].IsNull && Flow.lastUpdate < data["games"][i]["lastUpdate"].DateTimeValue) Flow.lastUpdate = data["games"][i]["lastUpdate"].DateTimeValue;
				
				string[] scores = {};
				string[] times = {};
				string[] pastMyScores = {};
				string[] pastMyTimes = {};
				string[] pastTheirScores = {};
				string[] pastTheirTimes = {};
				
				string faceID = "";
				int lastTurnID = -1;
				int tempWorldID = -1;
				string tempWorldName = "";
				string tempPastWorldName = "";
				
				List<Round> tempRoundList = new List<Round>();
				List<Round> tempPastMyRoundList = new List<Round>();
				List<Round> tempPastTheirRoundList = new List<Round>();
				
				string[] separator = {"|$@@$|"};
				
				if(data["games"][i]["turnStatus"].StringValue != "waitingTheme")
				{
					scores = data["games"][i]["scores"].StringValue.Split(separator,StringSplitOptions.None);
					times = data["games"][i]["times"].StringValue.Split(separator,StringSplitOptions.None);
				}
				if(!data["games"][i]["lastTurn"].IsNull) 
				{
					pastMyScores = data["games"][i]["myPastScores"].StringValue.Split(separator,StringSplitOptions.None);
					pastMyTimes = data["games"][i]["myPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
					pastTheirScores = data["games"][i]["theirPastScores"].StringValue.Split(separator,StringSplitOptions.None);
					pastTheirTimes = data["games"][i]["theirPastTimes"].StringValue.Split(separator,StringSplitOptions.None);
					
					for(int k = 0 ; k < pastMyScores.Length ; k++)
					{
						tempPastMyRoundList.Add(new Round(-1,-1,-1,pastMyTimes[k].ToFloat(),int.Parse(pastMyScores[k])));
						tempPastTheirRoundList.Add(new Round(-1,-1,-1,pastTheirTimes[k].ToFloat(),int.Parse(pastTheirScores[k])));
					}
				}
					
				if(!data["games"][i]["facebookID"].IsNull) faceID = data["games"][i]["facebookID"].StringValue;
				if(!data["games"][i]["lastTurn"].IsNull)lastTurnID = data["games"][i]["lastTurn"].Int32Value;
				if(!data["games"][i]["worldID"].IsNull)tempWorldID = data["games"][i]["worldID"].Int32Value;
				if(!data["games"][i]["worldName"].IsNull)tempWorldName = data["games"][i]["worldName"].StringValue;
				if(!data["games"][i]["pastWorldName"].IsNull)tempPastWorldName = data["games"][i]["pastWorldName"].StringValue;
				
				if(data["games"][i]["turnStatus"].StringValue != "waitingTheme")
				{
					for(int j = 0 ; j < 5 ; j++)
					{
						tempRoundList.Add(new Round(-1,data["games"][i]["turn"].Int32Value,data["games"][i]["friendID"].Int32Value,times[j].ToFloat(),scores[j].ToInt32()));
					}
				}
				
				bool foundGame = false;
				
				Friend tempFriend = new Friend();
				tempFriend = tempFriend.SetFriend(
					data["games"][i]["friendID"].StringValue,
					faceID,
					data["games"][i]["username"].StringValue,
					FriendshipStatus.NONE,
					data["games"][i]["hasApp"].StringValue.ToBool(),
					loadingDialog,
					messageOkDialog,
					messageOkCancelDialog);
					
				Game tempGame = new Game(
					data["games"][i]["gameID"].Int32Value,
					tempFriend,
					tempWorldID,
					tempWorldName,
					new List<Round>(),
					tempRoundList,
					data["games"][i]["turn"].Int32Value,
					lastTurnID,
					data["games"][i]["turnsWon"].Int32Value,
					data["games"][i]["turnsLost"].Int32Value,
					data["games"][i]["whoseMove"].StringValue,
					data["games"][i]["turnStatus"].StringValue,
					data["games"][i]["lastUpdate"].DateTimeValue,
					tempPastMyRoundList,
					tempPastTheirRoundList,
					tempPastWorldName);
				
				for(int h = 0 ; h < Flow.gameList.Count ; h++)
				{
					if(data["games"][i]["friendID"].Int32Value == int.Parse(Flow.gameList[h].friend.id))
					{
						if(data["games"][i]["lastUpdate"].DateTimeValue > Flow.gameList[h].lastUpdate)
						{
							//eh o mesmo e tah mais atualizado
							foundGame = true;
							tempGame.friend.picture = Flow.gameList[h].friend.picture;
							Flow.gameList[h] = tempGame;
						}
						else foundGame = true;
					}
				}
				if(!foundGame) Flow.gameList.Add(tempGame);
				
			}
			//Up Top Fix Me
			//sortList();
			
			// Recalcula o tempo de espera
			if(updatedAutomatically)
			{
				updatedAutomatically = false;
				if (data["games"].Count > 0)
				{
					//Debug.Log("chegou jogo novo");
					if (waitingTime > MIN_WAITING_TIME) waitingTime /= RATIO_WAITING_TIME;
				}
				else 
				{
					if (waitingTime < MAX_WAITING_TIME) waitingTime *= RATIO_WAITING_TIME;
				}
			}
		}
		
		Invoke("updatingAutomatically",waitingTime);
	}
	
	void FindFriends()
	{
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
		{
			panelManager.BringIn("InviteScenePanel");
		}
		else
		{
			fb_account = new GameFacebook(HandleLinkFacebook);
			Flow.game_native.startLoading(loadingDialog);
			fb_account.link();
		}
	}
	
	// Obtem o resultado da vinculacao com o Facebook
	private void HandleLinkFacebook(string error, IJSonObject data)
	{
		Debug.Log(data);
		
		Flow.game_native.stopLoading(loadingDialog);

		if (error != null)
		{
			Flow.game_native.showMessage(messageOkDialog, "Error", error);
			return;
		}
		
		panelManager.BringIn("InviteScenePanel");
	}
}
