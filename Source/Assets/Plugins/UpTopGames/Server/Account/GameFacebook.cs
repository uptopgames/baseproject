using UnityEngine;
using System;
using System.Collections;
using CodeTitans.JSon;

public class GameFacebook
{

	public GameBaseConnection<IJSonObject>.ConnectionAnswer callback;
	
	public GameFacebook(GameBaseConnection<IJSonObject>.ConnectionAnswer callback=null)
	{
		this.callback = callback;
	}
	
	private void sendToCallback(string error, IJSonObject data)
	{
		if (callback != null) callback(error, data);
	}
	
	// Obtem o resultado do vinculo com o Facebook
	private IEnumerator handleLink()
	{
#if UNITY_IPHONE
		EtceteraBinding.hideActivityView();
#elif UNITY_ANDROID
		EtceteraAndroid.hideProgressDialog();
#endif
		//GameGUI.game_native.stopLoading();
		
		// Numero maximo de tentativas
		int max_attempts = 5;
		
		WWW conn = null;
		
		WWWForm form = new WWWForm();
		form.AddField("token", Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-",""));
		
		while (max_attempts > 0)
		{
			conn = new WWW(Flow.URL_BASE + "login/facebook/fblresult.php", form);
			yield return conn;
			
			if (conn.error != null || conn.text != "") break;
			
			max_attempts--;
			yield return new WaitForSeconds(1);
		}
		
		if (max_attempts == 0 || conn.error != null)
		{
			Debug.LogError("Server error: " + conn.error);
			sendToCallback(GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE, null);
			
			yield break;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject data = reader.ReadAsJSonObject(conn.text);
		
		// Salva o token
		if (data.Contains("token")) GameToken.save(data);
		
		// Verifica se houve erro
		if (data == null || data.Contains("error"))
		{
			Debug.LogError("Json error: " + conn.text);
			
			string message;
			
			switch(data["error"].ToString())
			{
				case "access_denied":
					message = "You have to authorize our app on Facebook.";
					break;
					
				case "facebook_already_used":
					message = "Your Facebook is already in use on another account.";
					break;
					
				case "different_account":
					message = "Your account already has another Facebook linked.";
					break;
				
				default:
					message = GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE;
					break;
			}
			
			sendToCallback(message, null);
			yield break;
		}
		
		data = data["result"];
		
		Save.Set(PlayerPrefsKeys.TOKEN.ToString() , data["fbtoken"].ToString(), true);
		sendToCallback(null, data);
	}
	
	// Abre a tela para vinculo do Facebook ao usuario logado
	public bool link(GameBaseConnection<IJSonObject>.ConnectionAnswer callback=null)
	{
		if (callback != null) this.callback = callback;
		
		// Chama a pagina do Facebook no nosso servidor
		string fb_link_url = Flow.URL_BASE + "login/facebook/fblink.php";
		fb_link_url += "?token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		fb_link_url += "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "");
		
		// Up Top Fix (fazer uma dialog)
		//if (!GameGUI.game_native.openUrlInline(fb_link_url)) return false;
		
		// Obtem a resposta do servidor
		//GameGUI.components.StartCoroutine(handleLink());
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(handleLink());
		return true;
	}
	
	public delegate void LoginCallback(object state);
	
	// Abre a janela de login do usuario com o Facebook
	public void login(object state, LoginCallback callback)
	{
		if (!Info.HasConnection(true))
			return;
		
		if (Info.IsWeb())
		{
			Application.ExternalEval("getUserInfo()");	
			return;
		}
		
		// Criar o codigo de autenticacao
		string auth = System.Guid.NewGuid().ToString();
		
		// Chama a pagina do Facebook no nosso servidor
		string fb_login_url = Flow.URL_BASE + "login/facebook/";
		fb_login_url += "?app_id=" + Info.appId;
		fb_login_url += "&authentication=" + auth;
		fb_login_url += "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "");
	
		if (Save.HasKey(PlayerPrefsKeys.TOKEN.ToString())) fb_login_url += "&token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
		
		string device_push = PushNotifications.getPushDevice();
		if (device_push != null) fb_login_url += "&device_push=" + WWW.EscapeURL(device_push);
		
		// Up Top Fix Me (dialog)
		//GameGUI.game_native.openUrlInline(fb_login_url);
		
		// Obtem a resposta do servidor
		//GameGUI.components.StartCoroutine(getLoginResult(auth, state, callback));
		Flow.config.GetComponent<ConfigManager>().StartCoroutine(getLoginResult(auth, state, callback));
	}
	public void login() { login(0, null); }
	
	// Obtem as informacoes do Facebook no servidor
	private IEnumerator getLoginResult(string auth, object state, LoginCallback callback)
	{
		// Numero maximo de tentativas
		int max_attempts = 5;
		
		WWW conn = null;
		
		WWWForm form = new WWWForm();
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-", ""));
		form.AddField("authentication", auth);
		
		while (max_attempts > 0)
		{
			conn = new WWW(Flow.URL_BASE + "login/facebook/fbinfo.php", form);
			yield return conn;
			
			if (conn.error != null || conn.text != "") break;
			
			max_attempts--;
			
			yield return new WaitForSeconds(1);
		}
		
		if (max_attempts == 0 || conn.error != null)
		{
			Debug.LogError("Server error: " + conn.error);
			yield break;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject data = reader.ReadAsJSonObject(conn.text);
		
		if (data == null || data.Contains("error"))
		{
			Debug.LogError("Json error: " + conn.text);
			yield break;
		}
		
		GameToken.save(data);
		
		// Verifica se houve erro
		if (data.Contains("fb_error_reason") && data["fb_error_reason"].ToString() == "user_denied")
		{
			// Up Top Fix Me
			//GameGUI.game_native.showMessage("Error", "You need to authorize our app on Facebook.");
			
			// Redireciona para o login caso necessario
			// Up Top Fix Me
			//if (Scene.GetCurrent() != GameGUI.components.login_scene)
				//Scene.Login.Load(Info.firstScene);
			
			yield break;
		}
		
		// Salva o token do Facebook
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), data["fbtoken"].ToString(), true);		
		
		// Atualiza token da FacebookAPI
		FacebookAPI facebook = new FacebookAPI();
		facebook.UpdateToken();
		if (callback != null) callback(state);
	}
}
