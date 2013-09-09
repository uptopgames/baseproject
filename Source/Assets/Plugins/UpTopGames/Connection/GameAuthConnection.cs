using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public abstract class GameAuthConnection<T>: GameBaseConnection<T>
{
	// Indica se a conexao deve ser persistente no envia
	public bool persistent;
	
	public GameAuthConnection(string url): base(url)
	{
	}
	
	public GameAuthConnection(string url, ConnectionAnswer callback): base(url, callback)
	{
	}
	
	public GameAuthConnection(string url, ConnectionAnswerWithState callback_state): base(url, callback_state)
	{
	}
	
	public GameAuthConnection(string url, ConnectionAnswer callback, ConnectionAnswerWithState callback_state): base(url, callback, callback_state)
	{
	}
	
	// Chama o callback com os dados
	protected void sendToCallbackPersistent(string error, T data, object state, string id, string url, byte[] form_data, Hashtable headers)
	{
		// Verifica se e necessario repetir a conexao
		if (persistent)
		{
			if (error != null) GamePersistentConnection.addConnection(id, url, form_data, headers);
			else GamePersistentConnection.finished(id);
		}
		
		// Envia ao callback
		sendToCallback(error, data, state);
	}
	
	// Cria uma conexao padrao
	protected WWW setUpConnection(WWWForm form, bool with_login)
	{
		if (form == null) form = new WWWForm();
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-",""));
		
		if (Save.HasKey(PlayerPrefsKeys.TOKEN.ToString())) form.AddField("token", Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		
		if (with_login && Save.HasKey(PlayerPrefsKeys.APP_ID.ToString()))
		{
			if (Save.HasKey(PlayerPrefsKeys.EMAIL.ToString()) && Save.HasKey(PlayerPrefsKeys.PASSWORD.ToString()))
			{
				form.AddField("app_id", Save.GetInt(PlayerPrefsKeys.APP_ID.ToString()));
				form.AddField("email", Save.GetString(PlayerPrefsKeys.EMAIL.ToString()));
				form.AddField("password", Save.GetString(PlayerPrefsKeys.PASSWORD.ToString()));
			}
			
			else if (Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()))
			{
				form.AddField("app_id", Save.GetInt(PlayerPrefsKeys.APP_ID.ToString()));
				form.AddField("fbtoken", Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
			}
		}
		
		return setUpConnection(form);
	}
	
	public abstract IEnumerator startConnectionBytes(string id, byte[] data, Hashtable headers, object state);
	
	public abstract IEnumerator startConnection(string id, WWWForm form, object state, bool redo);
	public override IEnumerator startConnection(WWWForm form=null, object state=null)
	{
		return startConnection(uniqueId(), form, state, true);
	}
	public IEnumerator startConnection(string id, WWWForm form=null, object state=null)
	{
		return startConnection(id, form, state, true);
	}
	
	// Conecta ao servidor
	public override string connect(WWWForm form=null, object state=null)
	{
		string id = uniqueId();
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(startConnection(id, form, state));
		return id;
	}
	
	// Conecta ao servidor com os bytes do formulario
	public string connect(byte[] data, Hashtable headers, object state=null)
	{
		string id = uniqueId();
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(startConnectionBytes(id, data, headers, state));
		return id;
	}
	
	// Gera um identificador unico para a conexao
	public string uniqueId()
	{
		return System.Guid.NewGuid().ToString();
	}
}
