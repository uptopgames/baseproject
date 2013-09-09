using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


public class GameCenterAchievement
{
	public string identifier;
	public bool isHidden;
	public bool completed;
	public DateTime lastReportedDate;
	public float percentComplete;
	
	
	public static List<GameCenterAchievement> fromJSON( string json )
	{
		var achievementList = new List<GameCenterAchievement>();
		
		// decode the json
		var list = json.listFromJson();
		
		// create DTO's from the Hashtables
		foreach( Dictionary<string,object> ht in list )
			achievementList.Add( new GameCenterAchievement( ht ) );
		
		return achievementList;
	}
	
	
	public GameCenterAchievement( Dictionary<string,object> dict )
	{
		if( dict.ContainsKey( "identifier" ) )
			identifier = dict["identifier"] as string;
		
		if( dict.ContainsKey( "hidden" ) )
			isHidden = (bool)dict["hidden"];
		
		if( dict.ContainsKey( "completed" ) )
			completed = (bool)dict["completed"];
		
		if( dict.ContainsKey( "percentComplete" ) )
			percentComplete = float.Parse( dict["percentComplete"].ToString() );
		
		// grab and convert the date
		if( dict.ContainsKey( "lastReportedDate" ) )
		{
			double timeSinceEpoch = double.Parse( dict["lastReportedDate"].ToString() );
			DateTime intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			lastReportedDate = intermediate.AddSeconds( timeSinceEpoch );
		}
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Achievement> identifier: {0}, hidden: {1}, completed: {2}, percentComplete: {3}, lastReported: {4}",
			identifier, isHidden, completed, percentComplete, lastReportedDate );
	}

}
