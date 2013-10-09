using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GameBaseConnection<T>
{
	// Timeout da conexao
	public const int CONNECTION_TIMEOUT = 60;
	
	// String de conexao
	public string url;
	
	// Metodo chamado quanto a consulta estiver disponivel
	public delegate void ConnectionAnswer(string error, T data);
	protected ConnectionAnswer callback;
	
	// Metodo chamado quanto a consulta estiver disponivel
	public delegate void ConnectionAnswerWithState(string error, T data, object state);
	protected ConnectionAnswerWithState callback_state;
	
	public GameBaseConnection(string url)
	{
		this.url = url;
		
		this.callback = null;
		this.callback_state = null;
	}
	
	public GameBaseConnection(string url, ConnectionAnswer callback): this(url)
	{
		this.callback = callback;
	}
	
	public GameBaseConnection(string url, ConnectionAnswerWithState callback_state): this(url)
	{
		this.callback_state = callback_state;
	}
	
	public GameBaseConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state): this(url)
	{
		this.callback = callback;
		this.callback_state = callback_state;
	}
	
	// Cria uma conexao padrao
	protected virtual WWW setUpConnection(WWWForm form)
	{
		WWW conn;
		
		if (form == null) conn = new WWW(url);
		else conn = new WWW(url, form);
		
		return conn;
	}
	
	// Chama o callback com os dados
	protected void sendToCallback(string error, T data, object state)
	{
		if (callback != null) callback(error, data);
		if (callback_state != null) callback_state(error, data, state);
	}
	
	// Conecta ao servidor assincronamente
	public abstract IEnumerator startConnection(WWWForm form=null, object state=null);
	
	// Conecta ao servidor
	public virtual string connect(WWWForm form=null, object state=null)
	{
		// Se necessário, inicializa Prefab GamePersistentConnection
		//Initializate.Prefab(); 
		Debug.Log("lalala");
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(startConnection(form, state));
		return null;
	}
}
