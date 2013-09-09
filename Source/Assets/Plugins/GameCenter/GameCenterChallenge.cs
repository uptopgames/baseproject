using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


public enum GameCenterChallengeState
{
	Invalid = 0,
	Pending = 1, // The challenge has been issued, but neither completed nor declined
	Completed = 2, // The challenge has been completed by the receiving player
	Declined = 3, // The challenge has been declined by the receiving player
}


public class GameCenterChallenge
{
	public string issuingPlayerID;
	public string receivingPlayerID;
	public GameCenterChallengeState state;
	public DateTime issueDate;
	public DateTime completionDate;
	public string message;
	public uint hash;

	// either a score or an achievement will be present but not both
	public GameCenterScore score;
	public GameCenterAchievement achievement;

	public GameCenterChallenge( Dictionary<string,object> dict )
	{
		if( dict.ContainsKey( "issuingPlayerID" ) )
			issuingPlayerID = dict["issuingPlayerID"] as string;

		if( dict.ContainsKey( "receivingPlayerID" ) )
			receivingPlayerID = dict["receivingPlayerID"] as string;

		if( dict.ContainsKey( "state" ) )
		{
			var intState = int.Parse( dict["state"].ToString() );
			state = (GameCenterChallengeState)intState;
		}

		// grab and convert the dates
		if( dict.ContainsKey( "issueDate" ) )
		{
			var timeSinceEpoch = double.Parse( dict["issueDate"].ToString() );
			var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			issueDate = intermediate.AddSeconds( timeSinceEpoch );
		}

		if( dict.ContainsKey( "completionDate" ) )
		{
			var timeSinceEpoch = double.Parse( dict["completionDate"].ToString() );
			var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			completionDate = intermediate.AddSeconds( timeSinceEpoch );
		}

		if( dict.ContainsKey( "message" ) )
			message = dict["message"] as string;

		// do we have a score or an achievement?
		if( dict.ContainsKey( "score" ) )
			score = new GameCenterScore( dict["score"] as Dictionary<string,object> );

		if( dict.ContainsKey( "achievement" ) )
			achievement = new GameCenterAchievement( dict["achievement"] as Dictionary<string,object> );

		if( dict.ContainsKey( "hash" ) )
			hash = uint.Parse( dict["hash"].ToString() );
	}
	
	public static List<GameCenterChallenge> fromJson( string json )
	{
		var rawDataList = json.listFromJson();
		var challenges = new List<GameCenterChallenge>();
		
		foreach( Dictionary<string,object> dict in rawDataList )
			challenges.Add( new GameCenterChallenge( dict ) );
		
		return challenges;
	}

	public override string ToString()
	{
		return string.Format( "<Challenge> issuingPlayerID: {0}, receivingPlayerID: {1}, message: {2}, state: {3}, score: {4}, achievement: {5}, hash: {6}",
			issuingPlayerID, receivingPlayerID, message, state, score, achievement, hash );
	}

}