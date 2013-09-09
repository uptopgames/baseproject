// Classe do Flurry utilizada somente para Logs
// ps: Para Ads em geral, ignorar essa classe

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Flurry
{
	// Inicializa a classe Flurry
	// Essa classe só é instanciada automaticamente pela Advertisement (se o Flurry for solicitado)
	// Necessario enviar a Key do Flurry ao instanciar
	public static void Start(string key)
	{
		// Se não for mobile (ex: Web), retornar
#if UNITY_WEBPLAYER
			return;
#endif
		
		// Instancia prefab correta para as diferentes plataformas
		#if UNITY_IPHONE
			Initializate.AddPrefab("FlurryManager", typeof(FlurryManager));
		
			if (!Info.IsEditor())
				FlurryBinding.startSession(key);
		
		#elif UNITY_ANDROID
			Initializate.AddPrefab("FlurryAndroidManager", typeof(FlurryAndroidManager));
		
			if (!Info.IsEditor())
				FlurryAndroid.onStartSession(key, true, true);
		#endif
	}
	
	// Cria um evento de Log nativo
	// Ex: Flurry.Log("Iniciou o jogo!");
	public static void Log(string eventType)
	{
		#if UNITY_IPHONE
			FlurryBinding.logEvent(eventType, false);
		#elif UNITY_ANDROID
			FlurryAndroid.logEvent(eventType);
		#endif
	}
	
	// Cria um evento de Log nativo com parametros
	//
	// Ex: Flurry.Log("Iniciou o jogo!", new Dictionary<string, string>()
	//		{
	//			{"name", "Fulano"},
	//			{"platform", "Android"},
	//			{"country", "Brazil"}
	//		}
	//	);
	public static void Log(string eventType, Dictionary<string, string> parameters)
	{
		#if UNITY_IPHONE
			FlurryBinding.logEventWithParameters(eventType, parameters, false);
		#elif UNITY_ANDROID
			FlurryAndroid.logEvent(eventType, parameters);
		#endif
	}
	
}
