using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class GameCenterPlayer
{
	public string playerId;
	public string alias;
	public string displayName;
	public bool isFriend;

	public bool hasProfilePhoto
	{
		get
		{
			return File.Exists( profilePhotoPath );
		}
	}
	
	public Texture2D profilePhoto
	{
		get
		{
			if( !hasProfilePhoto )
				return null;
			
			var bytes = File.ReadAllBytes( profilePhotoPath );
			var tex = new Texture2D( 0, 0 );
			if( !tex.LoadImage( bytes ) )
				return null;
			
			return tex;
		}
	}
	
	private string _profilePhotoPath;

	public string profilePhotoPath
	{
		get
		{
			if( _profilePhotoPath == null )
				_profilePhotoPath = Path.Combine( Application.persistentDataPath, string.Format( "{0}.png", playerId ).Replace( ":", string.Empty ) );
			return _profilePhotoPath;
		}
	}


	public static List<GameCenterPlayer> fromJSON( string json )
	{
		List<GameCenterPlayer> scoreList = new List<GameCenterPlayer>();
		
		// decode the json
		var list = json.listFromJson();
		
		// create DTO's from the Hashtables
		foreach( Dictionary<string,object> ht in list )
			scoreList.Add( new GameCenterPlayer( ht ) );
		
		return scoreList;
	}

	
	public GameCenterPlayer( Dictionary<string,object> dict )
	{
		if( dict.ContainsKey( "playerId" ) )
			playerId = dict["playerId"] as string;
		
		if( dict.ContainsKey( "alias" ) )
			alias = dict["alias"] as string;
		
		if( dict.ContainsKey( "displayName" ) )
			displayName = dict["displayName"] as string;
		
		if( dict.ContainsKey( "isFriend" ) )
			isFriend = (bool)dict["isFriend"];
	}

	
	public override string ToString()
	{
		return string.Format( "<Player> playerId: {0}, alias: {1}, displayName: {2}, isFriend: {3}", playerId, alias, displayName, isFriend );
	}

}