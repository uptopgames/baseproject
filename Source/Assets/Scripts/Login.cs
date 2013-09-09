using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class Login : MonoBehaviour 
{
	public UIPanelManager panelManager;
	
	private string authentication;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	void CheckLogin()
	{
		Debug.Log("vamos checar o login");
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			panelManager.BringIn("MultiplayerScenePanel");
		}
		else
		{
			panelManager.BringIn("LoginScenePanel");
		}
	}
	
	void StandaloneLogin()
	{
		
	}
	
	void FacebookLogin()
	{
#if UNITY_EDITOR || !UNITY_WEBPLAYER
			requestToken();
#elif UNITY_WEBPLAYER
			Application.ExternalEval("refreshCanvas()");
#endif
	}
	
	// Obtem o token com o servidor
	protected void requestToken()
	{
		if (!Info.HasConnection(true)) 
		{
			Debug.Log("sem conexao");
			return;
		}
		
		// Up Top Fix Me
		//screen_status = ScreenStatus.waiting;
		
		// Criar o codigo de autenticacao
		authentication = System.Guid.NewGuid().ToString();
		
		// Chama a pagina do Facebook no nosso servidor
		string fb_login_url = Flow.URL_BASE + "login/facebook/";
		fb_login_url += "?app_id=" + Info.appId;
		fb_login_url += "&authentication=" + authentication;
		fb_login_url += "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "");
		
		string device_push = PushNotifications.getPushDevice();
		if (device_push != null) fb_login_url += "&device_push=" + WWW.EscapeURL(device_push);
		// Info.GetAppType() = Free, Pay, Custom1, Custom2, Custom3
		fb_login_url += "&app_version=" + Info.version.ToString();
		fb_login_url += "&app_type=" + Info.appType.ToString();

#if UNITY_EDITOR
		fb_login_url += "&app_platform=UnityEditor";
#elif UNITY_WEBPLAYER
		fb_login_url += "&app_platform=Facebook";
#elif UNITY_ANDROID
		fb_login_url += "&app_platform=Android";
#elif UNITY_IPHONE
		fb_login_url += "&app_platform=iOS";
#endif
			
		//fb_login_url += "&app_platform=" + (Mobile.IsMobile() ? (Mobile.IsAndroid() ? "Android" : "iOS") : (Info.IsWeb() ? "Facebook" : (Info.IsEditor() ? "UnityEditor" : "Other")));
		
		// Up Top Fix Me
		game_native.openUrlInline(fb_login_url);
		
		// Obtem a resposta do servidor
		StartCoroutine(getFacebookInfo());
	}
	
	// Obtem as informacoes do Facebook no servidor
	private IEnumerator getFacebookInfo()
	{
		Debug.Log("pegando info face");
		// Numero maximo de tentativas
		int max_attempts = 5;
		
		WWW conn = null;
		
		WWWForm form = new WWWForm();
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-", ""));
		form.AddField("authentication", authentication);
		
		// Up Top Fix Me
		//game_native.stopLoading();
		//GameGUI.disableGui();
		
		while (max_attempts > 0)
		{
			conn = new WWW(Flow.URL_BASE + "login/facebook/fbinfo.php", form);
			yield return conn;
			
			if (conn.error != null || conn.text != "") break;
			
			max_attempts--;
			
			yield return new WaitForSeconds(1);
		}
		
		// Up Top Fix Me
		//GameGUI.enableGui();
		//screen_status = ScreenStatus.not_logged;
		
		if (max_attempts == 0 || conn.error != null)
		{
			Debug.LogError("Server error: " + conn.error);
			// Up Top Fix Me
			//game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject data = reader.ReadAsJSonObject(conn.text);
		
		if (data == null || data.Contains("error"))
		{
			Debug.LogError("Json error: " + conn.text);
			// Up Top Fix Me
			//game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		//Debug.Log(data);
		
		GameToken.save(data);
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), data["fbtoken"].ToString(), true);
		Save.Set(PlayerPrefsKeys.NAME.ToString(), data["username"].ToString(),true);
		Save.Set(PlayerPrefsKeys.ID.ToString(), data["user_id"].ToString(),true);

		// Up Top Fix Me
		//GameGUI.enableGui();
		
		// Atualiza token da FacebookAPI
		if (Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) {
			FacebookAPI facebook = new FacebookAPI();
			facebook.SetToken(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
		}
		
		// Up Top Fix Me
		//Scene.Load(next_scene);
	}

}
