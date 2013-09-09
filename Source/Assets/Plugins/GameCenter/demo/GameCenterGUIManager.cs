using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Prime31;


public class GameCenterGUIManager : MonoBehaviourGUI
{
#if UNITY_IPHONE
	// some useful ivars to hold information retrieved from GameCenter. These will make it easier
	// to test this code with your GameCenter enabled application because they allow the sample
	// to not hardcode any values for leaderboards and achievements.
	private List<GameCenterLeaderboard> _leaderboards;
	private List<GameCenterAchievementMetadata> _achievementMetadata;
	private List<GameCenterPlayer> _friends;
	
	// bools used to hide/show different buttons based on what is loaded
	private bool _hasFriends;
	private bool _hasLeaderboardData;
	private bool _hasAchievementData;
	
	
	void Start()
	{
		// use anonymous delegates for this simple example for gathering data from GameCenter. In production you would want to
		// add and remove your event listeners in OnEnable/OnDisable!
		GameCenterManager.categoriesLoaded += delegate( List<GameCenterLeaderboard> leaderboards )
		{
			_leaderboards = leaderboards;
			_hasLeaderboardData = _leaderboards != null && _leaderboards.Count > 0;
		};
		
		GameCenterManager.achievementMetadataLoaded += delegate( List<GameCenterAchievementMetadata> achievementMetadata )
		{
			_achievementMetadata = achievementMetadata;
			_hasAchievementData = _achievementMetadata != null && _achievementMetadata.Count > 0;
		};
		
		// after authenticating grab the players profile image
		GameCenterManager.playerAuthenticated += () =>
		{
			GameCenterBinding.loadProfilePhotoForLocalPlayer();
			loadFriends();
		};
		
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
	}
	
	
	private void loadFriends()
	{
		GameCenterManager.playerDataLoaded += friends =>
		{
			_friends = friends;
			_hasFriends = _friends != null && _friends.Count > 0;
		};
		
		Debug.Log( "player is authenticated so we are loading friends" );
		GameCenterBinding.retrieveFriends( true, true );
	}
	
	
	void OnGUI()
	{
		beginColumn();		
		
		if( GUILayout.Button( "Get Player Alias" ) )
		{
			string alias = GameCenterBinding.playerAlias();
			Debug.Log( "Player alias: " + alias );
		}

		
		if( _hasFriends )
		{
			// see if we have any friends with a profile image on disk
			var friendWithProfileImage = _friends.Where( f => f.hasProfilePhoto ).FirstOrDefault();
			GUI.enabled = friendWithProfileImage != null;
			if( GUILayout.Button( "Show Friends Profile Image" ) )
			{
				var tex = friendWithProfileImage.profilePhoto;
				
				// grab our cube and display it with the texture
				var cube = GameObject.Find( "Cube" );
				cube.renderer.enabled = true;
				cube.renderer.material.mainTexture = tex;
			}
			GUI.enabled = true;
		}
		
		
		if( GUILayout.Button( "Load Received Challenges" ) )
		{
			GameCenterBinding.loadReceivedChallenges();
		}
		
		
		if( GUILayout.Button( "Show GC Leaderboards (iOS 6+)" ) )
		{
			GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
		}
		
		
		if( GUILayout.Button( "Show GC Achievements (iOS 6+)" ) )
		{
			GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Achievements );
		}
		
		
		endColumn( true );
		

		
		// toggle to show two different sets of buttons
		if( toggleButtonState( "Show Achievement Buttons" ) )
			leaderboardsGUI();
		else
			achievementsGUI();
		toggleButton( "Show Achievement Buttons", "Show Leaderboard Buttons" );
		
		
		endColumn();
		
		
		if( bottomLeftButton( "Load Multiplayer Scene (Requires Multiplayer Plugin!)", 340 ) )
		{
			Application.LoadLevel( "GameCenterMultiplayerTestScene" );
		}
	}
	
	
	void leaderboardsGUI()
	{
		if( GUILayout.Button( "Load Leaderboard Data" ) )
		{
			GameCenterBinding.loadLeaderboardTitles();
		}
		
		
		if( GUILayout.Button( "Show Leaderboards" ) )
		{
			GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );
		}
		
		
		if( !_hasLeaderboardData )
		{
			GUILayout.Label( "Load leaderboard data to see more options" );
			return;
		}
		
		if( GUILayout.Button( "Post Score" ) )
		{
			Debug.Log( "about to report a random score for leaderboard: " + _leaderboards[0].leaderboardId );
			GameCenterBinding.reportScore( Random.Range( 1, 99999 ), _leaderboards[0].leaderboardId );
		}
		
		
		if( GUILayout.Button( "Issue Score Challenge" ) )
		{
			var playerIds = new string[] { "player1", "player2" };
			var score = Random.Range( 1, 9999 );
			GameCenterBinding.issueScoreChallenge( score, 0, _leaderboards[0].leaderboardId, playerIds, "Beat this score!" );
		}
		
		
		if( GUILayout.Button( "Get Raw Score Data" ) )
		{
			GameCenterBinding.retrieveScores( false, GameCenterLeaderboardTimeScope.AllTime, 1, 25, _leaderboards[0].leaderboardId );
		}

		
		if( GUILayout.Button( "Get Scores for Me" ) )
		{
			GameCenterBinding.retrieveScoresForPlayerId( GameCenterBinding.playerIdentifier(), _leaderboards[0].leaderboardId );
		}
	}

	
	void achievementsGUI()
	{
		if( GUILayout.Button( "Load Achievement Metadata" ) )
		{
			GameCenterBinding.retrieveAchievementMetadata();
		}
		
		
		if( GUILayout.Button( "Get Raw Achievements" ) )
		{
			GameCenterBinding.getAchievements();
		}
		
		
		if( GUILayout.Button( "Show Achievements" ) )
		{
			GameCenterBinding.showAchievements();
		}
		
		
		if( !_hasAchievementData )
		{
			GUILayout.Label( "Load achievement metadata to see more options" );
			return;
		}
		
		if( GUILayout.Button( "Post Achievement" ) )
		{
			int percentComplete = (int)Random.Range( 2, 60 );
			Debug.Log( "sending percentComplete: " + percentComplete );
			GameCenterBinding.reportAchievement( _achievementMetadata[0].identifier, percentComplete );
		}
		
		
		if( GUILayout.Button( "Issue Achievement Challenge" ) )
		{
			var playerIds = new string[] { "player1", "player2" };
			GameCenterBinding.issueAchievementChallenge( _achievementMetadata[0].identifier, playerIds, "I challenge you" );
		}
	}
	
#endif
}
