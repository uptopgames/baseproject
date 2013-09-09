using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameConnection: GameBaseConnection<WWW>
{
	public GameConnection(string url): base(url)
	{
	}
	
	public GameConnection(string url, ConnectionAnswer callback): base(url, callback)
	{
	}
	
	public GameConnection(string url, ConnectionAnswerWithState callback_state): base(url, callback_state)
	{
	}
	
	public GameConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state): base(url, callback, callback_state)
	{
	}
	
	// Conecta ao servidor assincronamente
	public override IEnumerator startConnection(WWWForm form=null, object state=null)
	{
		WWW conn = setUpConnection(form);
		yield return conn;
		
		sendToCallback(conn.error, conn, state);
	}
	
	// Verifica se possui conexao com a Internet
	public static bool hasInternet()
	{
		//excerpt from code - http://forum.unity3d.com/threads/68938-How-can-you-tell-if-there-exists-a-network-connection-of-any-kind
		bool isConnectedToInternet = false;

#if UNITY_EDITOR || UNITY_WEBPLAYER
	    if (Network.player.ipAddress.ToString() != "127.0.0.1")
	    {
	        isConnectedToInternet = true;       
	    }
#elif UNITY_IPHONE
	    if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
	    {
	        isConnectedToInternet = true;
	    }
#elif UNITY_ANDROID
	    if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
	    {
	        isConnectedToInternet = true;
	    }
#endif	
		return isConnectedToInternet;
	}
	
	// Verifica se possui conexao com a Internet e mostra uma mensagem de erro
	public static bool hasInternetMessage()
	{
		bool connected = hasInternet();
		
		//if (!connected)
			//GameGUI.game_native.showMessage("Error", "There's is no Internet connection. Please, try again later");
		return connected;
	}
}
