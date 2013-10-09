using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class GameJsonConnection: GameBaseConnection<IJSonObject>
{
	public GameJsonConnection(string url): base(url)
	{
	}
	
	public GameJsonConnection(string url, ConnectionAnswer callback): base(url, callback)
	{
	}
	
	public GameJsonConnection(string url, ConnectionAnswerWithState callback_state): base(url, callback_state)
	{
	}
	
	public GameJsonConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state): base(url, callback, callback_state)
	{
	}
	
	// Conecta ao servidor assincronamente
	public override IEnumerator startConnection(WWWForm form=null, object state=null)
	{
		WWW conn = setUpConnection(form);
		Debug.Log("before");
		
		if(!Application.isPlaying)
		{
			ContinuationManager.Add(() => conn.isDone, () =>
			{
				// Verifica se houve erro
			    if (!string.IsNullOrEmpty(conn.error)) 
				{
					//Debug.Log("WWW failed: " + conn.error);
					sendToCallback(conn.error, ExtensionJSon.Empty(), state);
				}
				
				JSonReader reader = new JSonReader();
				IJSonObject data = null;
				
				bool er = false;
				// Faz o parse da resposta
				try
				{
					data = reader.ReadAsJSonObject(conn.text);
				}
				catch (JSonReaderException)
				{
					Debug.LogError("Error parsing Json: " + conn.text);
					sendToCallback("Error parsing json", null, state);
					er = true;
				}
				
				if(!er) sendToCallback(null, data, state);
				
				
			   // Debug.Log("WWW result : " + conn.text);
			});
		}
		else
		{
			yield return conn;
		
			Debug.Log("after");
			
			// Verifica se houve erro
			if (conn.error != null)
			{
				sendToCallback(conn.error, ExtensionJSon.Empty(), state);
				
				yield break;
			}
			
			JSonReader reader = new JSonReader();
			IJSonObject data = null;
			
			// Faz o parse da resposta
			try
			{
				data = reader.ReadAsJSonObject(conn.text);
			}
			catch (JSonReaderException)
			{
				Debug.LogError("Error parsing Json: " + conn.text);
				sendToCallback("Error parsing json", null, state);
				
				yield break;
			}	
			
			sendToCallback(null, data, state);
		}
	}
}
