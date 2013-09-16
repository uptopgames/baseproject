using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class GameRawAuthConnection: GameAuthConnection<WWW>
{
	// Mensagem padrao de erro
	public const string DEFAULT_ERROR_MESSAGE = "Connection error. Please, try again later.";
	public const string DEFAULT_FB_TOKEN_ERROR_MESSAGE = DEFAULT_ERROR_MESSAGE;
	
	public GameRawAuthConnection(string url): base(url)
	{
	}
	
	public GameRawAuthConnection(string url, ConnectionAnswer callback): base(url, callback)
	{
	}
	
	public GameRawAuthConnection(string url, ConnectionAnswerWithState callback_state): base(url, callback_state)
	{
	}
	
	public GameRawAuthConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state): base(url, callback, callback_state)
	{
	}
	
	// Cria uma conexao autenticada com o servidor
	public override IEnumerator startConnectionBytes(string id, byte[] data, Hashtable headers, object state)
	{
		WWW conn = new WWW(url, data, headers);
		yield return conn;
		
		handleConnection(conn, id, state, null, null);
	}
	
	// Conecta ao servidor assincronamente
	public override IEnumerator startConnection(string id, WWWForm form=null, object state=null, bool redo=true)
	{
		// Se for uma conexao permanente, marca o seu inicio
		if (persistent) GamePersistentConnection.started(id);
		
		// Salva o formulario passado caso necessario
		byte[] form_data;
		Hashtable headers;
		
		if (persistent && form != null)
		{
			form_data = form.data;
			headers = form.headers;
		}
		else
		{
			form_data = null;
			headers = null;
		}

		// Cria a conexao
		float start = Time.realtimeSinceStartup;
		//Debug.Log("Time now: " + start);
		WWW conn = setUpConnection(form, !redo);
		
		yield return conn;
		
		if (persistent) yield return conn;
		else
		{
			while (!conn.isDone)
			{
				if (Time.realtimeSinceStartup - start > CONNECTION_TIMEOUT)
				{
					string url = conn.url;
					conn.Dispose();
					Debug.Log("Timeout after: " + (Time.realtimeSinceStartup - start));
					sendToCallbackPersistent(DEFAULT_ERROR_MESSAGE, null, state, id, url, null, null);
					yield break;
				}

				yield return new WaitForEndOfFrame();
			}
		}

		//Debug.Log("Time after: " + Time.realtimeSinceStartup);
		//Debug.Log("Time passed: " + (Time.realtimeSinceStartup - start));
		handleConnection(conn, id, state, form_data, headers);
	}
	
	// Obtem o resultado da conexao
	private void handleConnection(WWW conn, string id, object state, byte[] form_data, Hashtable headers)
	{
		// Verifica se houve erro
		if (conn.error != null)
		{
			if (url.Contains(Flow.URL_BASE) && url.EndsWith(".picture.php"))
				return;
			
			Debug.Log("Internal server error to url (" + url + "): " + conn.error);
			
			sendToCallbackPersistent(conn.error, conn, state, id, conn.url, form_data, headers);
			
			return;
		}
		
		// Verifica se pediu senha
		try
		{
			JSonReader reader = new JSonReader();
			IJSonObject data = reader.ReadAsJSonObject(conn.text);
			
			if (data != null && data.Contains("ask"))
			{
				// Caso peca senha, avisa o erro
				if (data["ask"].ToString() == "password")
					sendToCallbackPersistent(DEFAULT_ERROR_MESSAGE + "r", null, state, id, conn.url, form_data, headers);
				
				// Obtem o token do Facebook, caso necessario
				else if (data["ask"].ToString() == "fb_token")
				{
					GameFacebook facebook = new GameFacebook();
					facebook.login();
					sendToCallbackPersistent(DEFAULT_FB_TOKEN_ERROR_MESSAGE, null, state, id, conn.url, form_data, headers);
				}
				
				// Refaz a conexao caso necessario e possivel
				//if (redo && GameGUI.components != null) GameGUI.components.StartCoroutine(startConnection(form, state, false));
				//else Application.LoadLevel(GameGUI.components.login_scene);
				
				return;
			}
		}
		catch (JSonReaderException)
		{
		}
		
		sendToCallbackPersistent(conn.error, conn, state, id, conn.url, form_data, headers);
	}
}
