using UnityEngine;
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
	
	// Use this for initialization
	void Start ()
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(FillValues);
	}
	
	void FillValues(EZTransition transition)
	{
		Debug.Log(Save.GetString(PlayerPrefsKeys.NAME.ToString()));
		
		userName.Text = Save.GetString(PlayerPrefsKeys.NAME.ToString());
		friendName.Text = Flow.currentGame.friend.name;
		turnsLost.Text = "Victories: " + Flow.currentGame.turnsLost.ToString();
		turnsWon.Text = "Defeats: " + Flow.currentGame.turnsWon.ToString();
		userScore.Text = "Score: " + Flow.currentGame.myTotalScore.ToString();
		for(int i = 0; i<userTimes.Length; i++)
		{
			userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.myRoundList[i].time.ToString();
		}
		if(Flow.thereIsAnotherPlayer)
		{
			for(int i = 0; i<friendTimes.Length; i++)
			{
				friendTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.theirRoundList[i].time.ToString();
			}
			friendScore.Text = "Score: " + Flow.currentGame.theirTotalScore.ToString();
		}
		else
		{
			for(int i = 0; i<friendTimes.Length; i++) friendTimes[i].Text = "";
			friendScore.Text = "waiting...";
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
