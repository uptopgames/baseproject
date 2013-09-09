// Classe utilizada para inicializar e manipular a KazzAPI a qualquer momento,
// garantindo estabilidade do objeto, forcando ser adicionado no exato momento em que é destruido (se for)

using UnityEngine;
using System.Collections;

public static class Initializate
{	
	// Inicializa a API, instancia Prefabs necessaria e carrega a proxima scene, caso solicitado
    public static void Start(bool load = true)
    {
        if (Info.CanDelete()) Save.DeleteAll();
		
        // Up Top Fix Me
		//if (load) Scene.First.Load();
		
		//LoadPrefabs();
    }
	
	// Retorna Prefab aonde sao salvos e instanciados todos scripts utilizados
	// No caso: #UpTopGamesAPI#
    public static GameObject Prefab()
    {
		if (Initializate.manager == null)
		{
	        GameObject apiObject = GameObject.Find(ConfigManager.API);
	        
            if(!apiObject) apiObject = new GameObject(ConfigManager.API);
			
			// Prefab aonde é salvo variaveis e classes em geral (funciona como uma variavel estatica)
            //if(!apiObject.HasComponent<CacheManager>()) apiObject.AddComponent<CacheManager>();
			
			// Manager de carregamento de scenes
			//if(!apiObject.HasComponent<SceneManager>()) apiObject.AddComponent<SceneManager>();
			
			// Manager de configuracoes salvas no servidor
			//if(!apiObject.HasComponent<ServerSettingsManager>()) apiObject.AddComponent<ServerSettingsManager>();
			
			// Component de Push Notifications / para as badges
			//if(!apiObject.HasComponent<PushWatcher>()) apiObject.AddComponent<PushWatcher>();
			
			// Component de Conexoes persistentes
			//if(!apiObject.HasComponent<GamePersistentConnection>()) apiObject.AddComponent<GamePersistentConnection>();
			
			//Debug.Log("oi, to criando prefabs");
			// Se for Web, adiciona objeto para o login pelo Canvas
			//if(Info.IsWeb()) 
			//{
			//	Debug.Log("taquei o treco de conectar na web...");
			//	apiObject.AddComponent<GameBrowserConnection>();
			//}
			
			// In-Game debug
			//if (Info.CanDebug() && !apiObject.HasComponent<BuildinUI>()) apiObject.AddComponent<BuildinUI>();
	        
			
			Initializate.manager = apiObject;
		}
		
		// Instancia prefabs
		//LoadPrefabs(true);
		
		return Initializate.manager;
	}
	
	
	// Instancia uma prefab necessaria para manipular métodos nativos
	// Ex: Initializate.AddPrefab("GameCenter", typeof(GameCenter));
	public static void AddPrefab(string name, System.Type type)
	{
		GameObject prefab = GameObject.Find(name);
		
		if (prefab != null && prefab.GetComponent(type))
			return;
		
		new GameObject(name, type);
	}
	
	// TODOS MÉTODOS ABAIXO SAO UTILIZADOS PARA USO INTERNO SOMENTE
	
	//public static string API = "#UpTopGamesAPI#";
	
	private static GameObject manager;
	
	// (uso interno) Carrega Prefabs default, necessarias a qualquer momento
	/*private static void LoadPrefabs(bool onlyRequired = false)
	{
		// Instancia Prefab de conexão persistente
		AddPrefab("GamePersistentConnection", typeof(GamePersistentConnection));
		
		if (onlyRequired)
			return;
		
		// Instancia PushWatcher (classe para alterar badges do PushWoosh)
		
		AddPrefab("PushWatcher", typeof(PushWatcher));
	}*/
}
