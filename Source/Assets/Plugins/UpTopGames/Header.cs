// Classe global para utilizacao de Headers em geral
// 
// Exemplo de uso:
// 
// Ao setar no Config o tipo de Component a ser utilizado, a classe Header automaticamente instancia o Component
// Ex: MapIQHeader.cs (no Config)
// 
// A classe Header ira instanciar a classe MapIQHeader em uma prefab (nao temporaria) interna
// Para utilizar metodos da sua per-app class (no caso: MapIQHeader), basta utilizar um cast
// 
// Ex: ((MapIQHeader)Header.manager).metodoDaClasseMapIQHeader(parametro);
// 
// Dessa forma, NÃO é necessario ficar o tempo todo procurando o objeto e o Component no root
// Segue abaixo um exemplo de como deveria ser utilizado o header SEM utilizar a classe Header global
//
// Ex errado:
// 		GameObject myHeader = GameObject.Find("MapIQHeader");
// 		if (myHeader != null)
// 		{
// 			MapIQHeader myComp = myHeader.GetComponent<MapIQHeader>();
// 			if (myComp != null)
// 				myComp.metodoDaClasseMapIQHeader(parametro);
// 		}
// 
// Para habilitar/desabilitar o Header (incluindo a per-app class):
// 		Header.Enable(); ou Header.Disable();
// 
// Para desabilitar somente a GUI do Header (ou seja, fazer apenas com que o Header nao desenhe as Textures):
// Header.GUI = false;
//
// O método Header.Enable() automaticamente seta Header.GUI como true, portanto, não é necessario setar duas vezes
//
// Se o Header for utilizado com alguma Texture (como na Unity3D não existe a possibilidade de setar como default uma Texture em um Component),
// é necessario criar uma Prefab, para as Textures serem instanciadas simultaneamente
//
// Passos:
// 		1- Criar uma Prefab com o nome MapIQHeader.prefab
//		2- Adicionar o Component a ser utilizado (no caso, MapIQHeader.cs)
//		3- Adicionar o Component DontDestroyOnLoad (para ser destruido ao mudar de scene)
//		4- Inserir a Prefab na pasta: Assets/Resources/KazzAPI
//
// No script per-app, é necessario colocar uma checagem no método nativo OnGUI():
//
// Ex:
//		void OnGUI()
//		{
//			if (!Header.GUI)
//				return;
//
//			<seu código aqui>
//		}

using UnityEngine;
using System.Collections;

public class Header : MonoBehaviour
{
	private int _coins;
	public int coins
	{
		get
		{
			if(_coins == null) _coins = 0;
			return _coins;
		}
		set
		{
			Debug.Log("seta para "+value);
			_coins = value;
			shopButton.Text = _coins.ToString();
			
		}
	}
	public UIButton shopButton;
	
	/*public void Awake()
	{
		// Checa para nao criar outro quando entrar em uma cena que tenha o prefab de config
		GameObject[] configs = GameObject.FindGameObjectsWithTag("Header");
		foreach(GameObject g in configs)
		{
			if(gameObject != g)
			{
				GameObject.Destroy(gameObject);
				return;
			}
		}
		
		// Seta a prefab #Config# para nao ser destruida na troca de cenas
		DontDestroyOnLoad(gameObject);
		Debug.Log("setou o header");
	}*/
	
	public void Start()
	{
		
	}
	
	public void CompraTeste()
	{
		ShopItem item1 = Flow.shopManager.GetShopItem("consumable_1");
		ShopItem item2 = Flow.shopManager.GetShopItem("consumable_2");
		
		item1.count = 1;
		item2.count = 1;
		
		Flow.shopManager.BuyItem(testecallback, item1, item2);
	}
	
	public void testecallback(ShopResultStatus status, string[] ids)
	{
		Debug.Log(status.ToString());
		foreach (string id in ids)
		{
			Debug.Log(id);
		}
	}

}

/*public class Header : MonoBehaviour
{
	// Atualiza os coins do usuario logado (se estiver logado)
	// Pode ser utilizada de qualquer lugar
	//
	// Ex: Header.UpdateCoins()
	public static void UpdateCoins()
	{
		if (!Save.GetString(PlayerPrefsKeys.TOKEN.ToString()).IsEmpty()) Coins.GetBalance(thisManager.OnReceiveCoins);
	}
	
	// Retorna instancia (per-app) do Header, podendo ser utilizado qualquer tipo de Component, desde que seja setado no Config
	public static object manager
	{
		get
		{
			thisManager.Force();
			
			if (Header._myManager != null) return Header._myManager;
			
			GameObject obj = Flow.config;
			
			string component = Info.headerComponent;
			
			if (component.IsEmpty())
			{
				//Debug.LogWarning("Header Component cannot be null!");
				return default(object);
			}
			
			GameObject prefab = (GameObject)Resources.Load("KazzAPI/" + component, typeof(GameObject));
		
			if (prefab != null)
			{
				GameObject instance = (GameObject)Instantiate(prefab) as GameObject;
				instance.name = component;
				
				DontDestroyOnLoad(instance);
				
				obj = instance;
			}
			
			Header._myManager = (object)(obj.GetComponent(component) ? obj.GetComponent(component) : obj.AddComponent(component));
			
			Header.GUI = true;
			
			return Header._myManager;
		}
	}
	
	// Habilita Header, instancia classe per-app e atualiza quantidade de coins
	public static void Enable()
	{
		Header._myManager = manager;
		
		if (!Header.firstLoad)
			return;
		
		HandleUpdateCoins();
		
		Header.GUI = true;
	}
	
	// Desabilita Header e destroi classe per-app (se existir)
	public static void Disable()
	{
		GameObject obj = Flow.config;
		
		Component component = obj.GetComponent(Info.headerComponent);
		
		if (component != null)
			Destroy(component);
		
		GameObject prefab = GameObject.Find(Info.headerComponent);
		
		if (prefab != null)
			Destroy(prefab);
		
		Header._myManager = null;
		Header.GUI = false;
	}
	
	// TODOS MÉTODOS ABAIXO SAO UTILIZADOS PARA USO INTERNO SOMENTE
	public static int coins, depth = -10;
	
	public static bool
		GUI = false,
		firstLoad = true,
		mini = false;
	
	private static Header _thisManager;
	private static object _myManager;
	
	// (uso interno) Hack para forçar instanciar a classe
	private void Force()
	{}
	
	// (uso interno) Retorna instancia do Header
	private static Header thisManager
	{
		get
		{
			if (Header._thisManager != null)
				return Header._thisManager;
			
			GameObject obj = Flow.config;
	
			Header._thisManager = (!obj.GetComponent<Header>())
				? obj.AddComponent<Header>()
				: obj.GetComponent<Header>();
			
			return Header._thisManager;
		}
	}
	
	// (uso interno) Callback aonde retorna a quantidade de coins do usuario
	private void OnReceiveCoins(int userId, int coins)
	{
		Header.coins = coins;
		Header.firstLoad = false;
	}
	
	// (uso interno) Solicita atualizacao da quantidade de coins do usuario (se estiver logado)
	private static void HandleUpdateCoins()
	{
		if (!Save.GetString(PlayerPrefsKeys.TOKEN.ToString()).IsEmpty()) Header.UpdateCoins();
		else new KTimer(1f, HandleUpdateCoins);
	}
}*/
