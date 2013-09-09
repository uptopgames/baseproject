using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class PushWatcher: PushNotifications
{
#if UNITY_IPHONE
	// Conexao com o servidor
	private GameJsonAuthConnection conn;
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/push/reset.php", handleBadgeReset);
	}
	
	
	// Reseta o contado de badges
	private void badgeReset()
	{
        // Caso nao possui token, nao realiza a conexao
        if (Save.GetString(PlayerPrefsKeys.TOKEN.ToString()).IsEmpty()) return;
		
		// Informa ao servidor que recebeu notificacoes
		StartCoroutine(conn.startConnection());
	}
	
	private void handleBadgeReset(string error, IJSonObject data)
    {
        if (error != null) return;

        EtceteraBinding.setBadgeCount(0);
	}
	
	// Ao despausar o app, reseta o contador
	void OnApplicationPause(bool pause)
	{
        if (pause) return;
		
		badgeReset();
	}
	
#endif
    public override void onRegisteredForPushNotifications(string token)
	{
	}

	public override void onFailedToRegisteredForPushNotifications(string error)
	{
	}
	
	// Ao receber uma badge, reseta o contador
	public override void onPushNotificationsReceived(string payload)
	{
#if UNITY_IPHONE
        badgeReset();
#endif
	}
	
}
