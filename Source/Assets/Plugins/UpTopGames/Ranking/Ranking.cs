// Classe utilizada para setar Ranking
// Ao criar novas plataformas de Ranking (OpenFeint/Gree, UpTopGames Ranking), solicitado alterar nessa classe
//
// Ao iniciar o jogo, a classe automaticamente tenta autenticar o usuário com a Apple
//
// Exemplo de uso:
//		void Start()
//		{
//			Ranking.AuthenticateUser();
//		}
//		
//		void OnGUI()
//		{
//			if (GameGUI.regularButton(new Rect(0f, 0f, 100f, 100f), GUIStyle))
//				Ranking.ShowLeaderboard(GameCenterLeaderboardTimeScope.Week, "com.freegamesandtopapps.baseranking", true);
//		}

using UnityEngine;
using System.Collections;

public static class Ranking
{
	// Mensagens default de erro a serem exibidas ao usuario (opcional)
	private const string
		
		MESSAGE_GAMECENTER_UNAVAIBLE	=	"GameCenter is currently unavaible on your device.",
		MESSAGE_USER_NOT_AUTHENTICATED	=	"User is currently not authenticated. Try again later!";
	
	// Se o GameCenter esta ativo na plataforma
	// ps: Se o método for executado em qualquer plataforma que não seja iOS, retornara false
	public static bool GameCenterAvailable()
	{
		#if UNITY_IPHONE
			return GameCenterBinding.isGameCenterAvailable();
		#else
			return false;
		#endif
	}
	
	// Se o usuario esta autenticado
	// Apos ser a classe ser isntanciada, é necessario autenticar o usuario com a Apple para fazer as checagens
	public static bool UserIsAuthenticated()
	{
		#if UNITY_IPHONE
			return GameCenterBinding.isPlayerAuthenticated();
		#else
			return false;
		#endif
	}
	
	// Tenta autenticar o usuario com a Apple e instancia a Prefab GameCenterManager (caso necessario)
	public static void AuthenticateUser()
	{
		Setup();
		
		if (!GameCenterAvailable() || UserIsAuthenticated())
			return;
		
		#if UNITY_IPHONE
			GameCenterBinding.authenticateLocalPlayer();
		#endif
	}
	
	// Seta score ao usuario
	// Caso o usuario nao estiver autenticado, tenta autenticar
	// Se apos todas tentativas o usuario nao for autenticado, ignorar
	//
	// Ex: SetScore(250, "com.freegamesandtopapps.baseranking");
	// Adiciona 250 de score no usuario, na leaderboard "com.freegamesandtopapps.baseranking"
	public static void SetScore(long score, string leaderboardId)
	{
		AuthenticateUser();
			
#if UNITY_IPHONE
			GameCenterBinding.reportScore(score, leaderboardId);
#endif
	}
	
	public static void SetScore(long score, ulong context, string leaderboardId)
	{
		AuthenticateUser();
		
#if UNITY_IPHONE
		GameCenterBinding.reportScore(score, context, leaderboardId);
#endif
	}
	
	// Tenta mostrar o ranking de determinada leaderboard
	// 
	// Ex1: ShowLeaderboard(GameCenterLeaderboardTimeScope.Today, "com.freegamesandtopapps.baseranking");
	// Irá mostrar a leaderboard com os melhores jogadores do dia
	//
	// Se o GameCenter estiver indisponivel ou o usuario nao estiver autenticado, retorna
	//
	// Ex2: ShowLeaderboard(GameCenterLeaderboardTimeScope.All, "com.freegamesandtopapps.baseranking", true);
	// Irá mostrar a leaderboard com os melhores jogadores no total
	//
	// Se o GameCenter estiver indisponivel ou o usuario nao estiver autenticado, mostra popup informando
	public static void ShowLeaderboard(GameCenterLeaderboardTimeScope timeScope, string leaderboardId, bool popup = false)
	{
		AuthenticateUser();
		
		if (!GameCenterAvailable())
		{
			//if (popup) GameGUI.game_native.showMessage(Info.name, MESSAGE_GAMECENTER_UNAVAIBLE);
			
			return;
		}
		
		if (!UserIsAuthenticated())
		{
			//if (popup) GameGUI.game_native.showMessage(Info.name, MESSAGE_USER_NOT_AUTHENTICATED);
			
			return;
		}
			
		#if UNITY_IPHONE
			GameCenterBinding.showLeaderboardWithTimeScopeAndLeaderboard(timeScope, leaderboardId);
		#endif
	}
	
	public static void ShowLeaderboard(GameCenterLeaderboardTimeScope timeScope, bool popup = false)
	{
		ShowLeaderboard(timeScope, "", popup);
	}
	
	// TODOS MÉTODOS ABAIXO SAO UTILIZADOS PARA USO INTERNO SOMENTE
	
	// (uso interno) Instancia prefab do GameCenter, caso necessario, foi tirado pois o prime faz isso automaticamente agora...
	private static void Setup()
	{
		#if UNITY_IPHONE
			//Initializate.AddPrefab("GameCenterManager", typeof(GameCenterManager));
		#endif
	}
}
