using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class GamePlay : MonoBehaviour
{
	private int rounds = 0;
	private float counter = 0;
	
	public SpriteText counterSprite;
	public SpriteText roundSprite;
	
	public GameObject messageOkDialog;
	public GameObject messageOkCancelDialog;
	public GameObject loadingDialog;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		counter+=Time.deltaTime;
		counterSprite.Text = "Time: " + counter.ToString();
		roundSprite.Text = "Round: " + rounds.ToString();
	}
	
	void EndGame()
	{
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE+"base/managegame.php", HandleEndGame);
		WWWForm form = new WWWForm();
		for(int i = 0; i<Flow.ROUNDS_PER_TURN; i++)
		{
			form.AddField("times["+i+"]", Flow.currentGame.myRoundList[i].time.ToString());
		}
		form.AddField("friendID", Flow.currentGame.friend.id);
		form.AddField("worldID", Flow.currentGame.worldID);
		conn.connect(form);
	}
			
	void HandleEndGame(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();
		
		if(error!=null)
		{
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		Debug.Log(data);
		//Application.LoadLevel("GamePlay");
		
		//Flow.currentGame.id = data["gameID"].Int32Value;
		
		foreach(IJSonObject score in data.ArrayItems)
			Flow.currentGame.myTotalScore += score.Int32Value;
		
		Debug.Log( Flow.currentGame.myTotalScore);
		
		if(Flow.currentMode == GameMode.SinglePlayer)
		{
			Flow.nextPanel = PanelToLoad.WinLose;
		}
		else
		{
			Flow.nextPanel = PanelToLoad.BattleStatus;
		}
		
		Application.LoadLevel("Mainmenu");
	}
	
	void EndRound()
	{
		rounds++;
		Flow.currentGame.myRoundList.Add(new Round(-1,-1,-1,counter,-1));
		counter = 0;
		
		if(rounds>=Flow.ROUNDS_PER_TURN)
		{
			Flow.game_native.startLoading();
			Debug.Log("endGame");
			EndGame();
		}
	}
}