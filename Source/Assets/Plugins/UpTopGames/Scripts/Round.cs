using UnityEngine;
using System.Collections;
	
public class Round
{
	public int roundID;	
	public int turnID;
	public int userID;
	public float time;
	public int score;
	
	public Round(int id, int turn, int user, float time, int score)
	{
		this.roundID = id;
		this.turnID = turn;
		this.userID = user;
		this.time = time;
		this.score = score;
	}
}