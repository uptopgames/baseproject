using UnityEngine;
using System;
using System.Collections;

public class BattleStatus : MonoBehaviour
{
	public SpriteText userName;
	public SpriteText friendName;
	public UIInteractivePanel userPicture;
	public UIInteractivePanel friendPicture;
	public UIInteractivePanel userPortrait;
	public UIInteractivePanel friendPortrait;
	public SpriteText turnsLost;
	public SpriteText turnsWon;
	public SpriteText userScore;
	public SpriteText friendScore;
	public SpriteText[] userTimes;
	public SpriteText[] friendTimes;
	
	public GameObject loadingDialog;
	public GameObject messageOkDialog;
	public GameObject messageOkCancelDialog;
	
	public UIInteractivePanel lastPastPanel;
	public UIInteractivePanel firstPastPanel;
	
	public UIScrollList scroll;
	public GameObject multiplayerPrefab;
	
	// Use this for initialization
	void Start ()
	{
		firstPastPanel.transform.position = firstPastPanel.transitions.list[0].animParams[0].axis;
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(FillValues);		
	}
	
	void FillValues(EZTransition transition)
	{
		Debug.Log(Save.GetString(PlayerPrefsKeys.NAME.ToString()));
		
		if(Flow.path == TurnStatus.BeginGame)
		{
			Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.myRoundList)
			{
				Flow.currentGame.myTotalScore += r.score;
			}
			
			for(int i = 0; i<friendTimes.Length; i++) friendTimes[i].Text = "";
			friendScore.Text = "Waiting...";
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.myRoundList[i].time.ToString();
			}
		}
		else if(Flow.path == TurnStatus.AnswerGame)
		{	
			Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.myRoundList)
			{
				Flow.currentGame.myTotalScore += r.score;
			}
			Flow.currentGame.theirTotalScore = 0;
			foreach(Round r in Flow.currentGame.theirRoundList)
			{
				Flow.currentGame.theirTotalScore += r.score;
			}
			
			for(int i = 0; i<friendTimes.Length; i++)
			{
				friendTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.theirRoundList[i].time.ToString();
			}
			friendScore.Text = "Score: " + Flow.currentGame.theirTotalScore.ToString();
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.myRoundList[i].time.ToString();
			}
		}
		else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.pastMyRoundList)
			{
				Flow.currentGame.myTotalScore += r.score;
			}
			Flow.currentGame.theirTotalScore = 0;
			foreach(Round r in Flow.currentGame.pastTheirRoundList)
			{
				Flow.currentGame.theirTotalScore += r.score;
			}
			
			Debug.Log(Flow.currentGame.pastTheirRoundList.Count);
			
			for(int i = 0; i<friendTimes.Length; i++)
			{
				friendTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.pastTheirRoundList[i].time.ToString();
			}
			friendScore.Text = "Score: " + Flow.currentGame.theirTotalScore.ToString();
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.pastMyRoundList[i].time.ToString();
			}
			
			ShowPastPanel();
		}
		
		userName.Text = Save.GetString(PlayerPrefsKeys.NAME.ToString());
		friendName.Text = Flow.currentGame.friend.name;
		turnsLost.Text = "Victories: " + Flow.currentGame.turnsLost.ToString();
		turnsWon.Text = "Defeats: " + Flow.currentGame.turnsWon.ToString();
		userScore.Text = "Score: " + Flow.currentGame.myTotalScore.ToString();
	}
	
	public void ShowPastPanel()
	{
		Debug.Log("ShowPastPanel");
		StartPastTransition();
	}
	
	void NextButton()
	{
		if(Flow.path == TurnStatus.BeginGame)
		{
			Debug.Log ("current Game ID: " + Flow.currentGame.id);
			Debug.Log ("past Index: " + Flow.currentGame.pastIndex);
			if (Flow.currentGame.id != -1) 
			{
				Flow.yourTurnGames--;
				Flow.theirTurnGames++;
				Flow.gameList[Flow.currentGame.pastIndex].whoseMove = "their";
			}
			else
			{
				//tempContainer.transform.FindChild("ContainerButton").gameObject.SetActive(false);
				//tempContainer.transform.FindChild("ContainerSprite").gameObject.SetActive(true);
				
				Debug.Log ("id : " + Flow.currentGame.id);
				Debug.Log ("listCount: " + Flow.gameList.Count);
				Debug.Log ("tt count: " +Flow.theirTurnGames);
				
				if(Flow.theirTurnGames == 0)
				{
					Game g = new Game();
					g.id = -999;
					g.whoseMove = "their";
					g.lastUpdate = new DateTime(2999,12,31);
					g.friend = new Friend();
					g.friend.id = "-999";
					if(Flow.yourTurnGames > 0) g.pastIndex = Flow.yourTurnGames+1;
					else g.pastIndex = 0;
					Debug.Log("adicionei label theirturn na gamelist");
					Flow.gameList.Add(g);
				}
				
				Flow.theirTurnGames++;
				Flow.currentGame.whoseMove = "their";
				
				Flow.currentGame.pastIndex = Flow.gameList.Count;
				Flow.gameList.Add (Flow.currentGame);
			}
			
			//Flow.currentGame.ResetGame();
			
			UIPanelManager.instance.BringIn("MultiplayerScenePanel");
		}
		else if(Flow.path == TurnStatus.AnswerGame)
		{
			Flow.path = TurnStatus.BeginGame;
			UIPanelManager.instance.BringIn("WorldSelectionScenePanel");
		}
		else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.path = TurnStatus.AnswerGame;
			Application.LoadLevel("Game");
		}
	}
	
	void StartPastTransition()
	{
		BringFirstPastPanel(firstPastPanel.transitions.list[0]);
		lastPastPanel.transform.position = lastPastPanel.transitions.list[0].animParams[0].axis;
	}
	
	void BringFirstPastPanel(EZTransition transition)
	{
		firstPastPanel.transitions.list[0].AddTransitionEndDelegate(DismissFirstPastPanel);
		firstPastPanel.BringIn();
	}
	
	void DismissFirstPastPanel(EZTransition transition)
	{
		firstPastPanel.transitions.list[2].AddTransitionEndDelegate(BringSecondPastPanel);
		firstPastPanel.Dismiss();
	}
	
	void BringSecondPastPanel(EZTransition transition)
	{
		lastPastPanel.transitions.list[0].AddTransitionEndDelegate(DismissSecondPastPanel);
		lastPastPanel.BringIn();
	}
	
	void DismissSecondPastPanel(EZTransition transition)
	{
		lastPastPanel.transitions.list[2].AddTransitionEndDelegate(StartGame);
		lastPastPanel.Dismiss();
	}
	
	void StartGame(EZTransition transition)
	{
		if(Flow.path == TurnStatus.AnswerGame)
		{
			Flow.path = TurnStatus.BeginGame;
			// Fix Me UPTOP Mandar para world Selection
			Application.LoadLevel("Game");
		}
		else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.path = TurnStatus.AnswerGame;
			Application.LoadLevel("Game");
		}
	}
}