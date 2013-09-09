using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class GameCenterAchievementMetadata
{
	public string identifier;
	public string description;
	public string unachievedDescription;
	public bool isHidden;
	public int maximumPoints;
	public string title;
	
	
	public static List<GameCenterAchievementMetadata> fromJSON( string json )
	{
		List<GameCenterAchievementMetadata> metadataList = new List<GameCenterAchievementMetadata>();
		
		// decode the json
		var list = json.listFromJson();
		
		// create DTO's from the Hashtables
		foreach( Dictionary<string,object> ht in list )
			metadataList.Add( new GameCenterAchievementMetadata( ht ) );
		
		return metadataList;
	}
	
	
	public GameCenterAchievementMetadata( Dictionary<string,object> dict )
	{
		if( dict.ContainsKey( "identifier" ) )
			identifier = dict["identifier"] as string;
		
		if( dict.ContainsKey( "achievedDescription" ) )
			description = dict["achievedDescription"] as string;
		
		if( dict.ContainsKey( "unachievedDescription" ) )
			unachievedDescription = dict["unachievedDescription"] as string;
		
		if( dict.ContainsKey( "hidden" ) )
			isHidden = (bool)dict["hidden"];
		
		if( dict.ContainsKey( "maximumPoints" ) )
			maximumPoints = int.Parse( dict["maximumPoints"].ToString() );
		
		if( dict.ContainsKey( "title" ) )
			title = dict["title"] as string;
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<AchievementMetaData> identifier: {0}, hidden: {1}, maxPoints: {2}, title: {3} desc: {4}, unachDesc: {5}",
		 	identifier, isHidden, maximumPoints, title, description, unachievedDescription );
	}

}
