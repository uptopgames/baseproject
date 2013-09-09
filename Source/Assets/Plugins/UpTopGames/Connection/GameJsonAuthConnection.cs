using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class GameJsonAuthConnection: GameAuthConnection<IJSonObject>
{
	// Mensagem padrao de erro
	public const string DEFAULT_ERROR_MESSAGE = "Connection error. Please, try again later.";
	public const string DEFAULT_FB_TOKEN_ERROR_MESSAGE = DEFAULT_ERROR_MESSAGE;
	
	// Mensagens de erro
	protected Dictionary<string, string> messages;
	
	public GameJsonAuthConnection(string url, Dictionary<string, string> messages=null): base(url)
	{
		setErrorMessages(messages);
	}
	
	public GameJsonAuthConnection(string url, ConnectionAnswer callback, Dictionary<string, string> messages=null): base(url, callback)
	{
		setErrorMessages(messages);
	}
	
	public GameJsonAuthConnection(string url, ConnectionAnswerWithState callback_state, Dictionary<string, string> messages=null): base(url, callback_state)
	{
		setErrorMessages(messages);
	}
	
	public GameJsonAuthConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state, Dictionary<string, string> messages=null): base(url, callback, callback_state)
	{
		setErrorMessages(messages);
	}
	
	// Atribui as mensagens de erro
	public void setErrorMessages(Dictionary<string, string> messages)
	{
		this.messages = messages != null? messages: new Dictionary<string, string>();
	}
	
	// Cria uma conexao autenticada com o servidor
	public override IEnumerator startConnectionBytes(string id, byte[] data, Hashtable headers, object state)
	{
		WWW conn = new WWW(url, data, headers);
		yield return conn;
		
		handleConnection(conn, id, state, null, null);
	}
	
	// Cria uma conexao autenticada com o servidor
	public override IEnumerator startConnection(string id, WWWForm form, object state, bool redo)
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
			Debug.LogError("Internal server error (" + url + "): " + conn.error);
			
			string error = "";
			if (!messages.TryGetValue("server_error", out error)) error = DEFAULT_ERROR_MESSAGE;
			sendToCallbackPersistent(error, null, state, id, conn.url, form_data, headers);
			
			return;
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
			string fbError = conn.text;
			
			if (fbError.Contains("ask") && fbError.Contains("fb_token"))
			{
				new GameFacebook().login();
				sendToCallbackPersistent(DEFAULT_FB_TOKEN_ERROR_MESSAGE, null, state, id, conn.url, form_data, headers);
				return;
			}
			
			Debug.LogError("Error parsing Json: " + conn.text);
			
			string error = "";
			if (!messages.TryGetValue("json_error", out error)) error = DEFAULT_ERROR_MESSAGE;
			sendToCallbackPersistent(error, null, state, id, conn.url, form_data, headers);
			
			return;
		}
		// Verifica se nao houve resultado
		if (data == null)
		{
			
			sendToCallbackPersistent(null, null, state, id, conn.url, form_data, headers);
			return;
		}
		
		if (data.Contains("ask"))
		{
			// Caso peca senha,s avisa o erro
			if (data["ask"].ToString() == "password")
			{
				sendToCallbackPersistent(DEFAULT_ERROR_MESSAGE + "j", null, state, id, conn.url, form_data, headers);
				
				Save.Delete(PlayerPrefsKeys.TOKEN.ToString());
				// Invalid token
				//Scene.Load("Login", Info.firstScene);
			}
			// Obtem o token do Facebook, caso necessario
			else if (data["ask"].ToString() == "fb_token")
			{
				new GameFacebook().login();
				sendToCallbackPersistent(DEFAULT_FB_TOKEN_ERROR_MESSAGE, null, state, id, conn.url, form_data, headers);
			}
			
			// Refaz a conexao caso necessario e possivel
			//if (redo && GameGUI.components != null) GameGUI.components.StartCoroutine(startConnection(form, state, false));
			//else Application.LoadLevel(GameGUI.components.login_scene);
			
			return;
		}
		
		// Salva o token enviado pelo caso necessario
		if (data.Contains("token"))
		{
			if (!Save.HasKey(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString()))
			{
				Save.Set(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString(), data["expiration"].ToString(), true);
				Save.Set(PlayerPrefsKeys.TOKEN.ToString(), data["token"].ToString(), true);
			}
			
			else
			{
				System.DateTime old_date = System.DateTime.Parse(Save.GetString(PlayerPrefsKeys.TOKEN_EXPIRATION.ToString()));
				System.DateTime new_date = System.DateTime.Parse(data["expiration"].ToString());
				
				if (new_date > old_date)
					Save.Set(PlayerPrefsKeys.TOKEN.ToString(), data["token"].ToString(), true);
			}
		}
		
		// Trata a mensagem de erro
		if (data.Contains("error"))
		{
			Debug.LogError("Server error: " + data["error"].ToString());
			
			string error = "";
			if (!messages.TryGetValue(data["error"].ToString(), out error)) error = DEFAULT_ERROR_MESSAGE;
			
			sendToCallbackPersistent(error, ExtensionJSon.Empty(), state, id, conn.url, form_data, headers);
			
			return;
		}
		
		// Chama o callback com o resultado
		/*if (Scene.GetCurrent() != GameGUI.components.login_scene) */data = data["result", (IJSonObject) null];
		sendToCallbackPersistent(null, data, state, id, conn.url, form_data, headers);
	}
}
