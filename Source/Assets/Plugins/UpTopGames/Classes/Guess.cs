using UnityEngine;
using System.Collections;
	
public class Guess : MonoBehaviour
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