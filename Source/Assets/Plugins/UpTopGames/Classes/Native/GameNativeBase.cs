using UnityEngine;
using System;
using System.Collections;

public abstract class GameNativeBase: MonoBehaviour
{
	// Mensagens padrao das telas de loading
	public static string LOADING_TITLE = "Loading";
	public static string LOADING_MESSAGE = "Please wait...";
	
	// Mensagens padrao dos botoes
	public string default_ok_button = "OK";
	public string default_cancel_button = "Cancel";
	
	// Tamanho da foto
	public const int PHOTO_WIDTH = 512;
	public const int PHOTO_HEIGHT = PHOTO_WIDTH;
	
	// Nome da foto
	protected string photo_name = null;
	
	// Filtra uma mensagem para exibicao na tela
	protected string filterMessage(string message)
	{
		return message.Replace('\n', ' ');
	}
	
	// Adiciona uma acao para os eventos de cancelamento de uma compra
	public abstract void addActionUserCancelledPurchase(Action<string> action);
	
	// Remove uma acao para os eventos de cancelamento de uma compra
	public abstract void removeActionUserCancelledPurchase(Action<string> action);
	
	// Adiciona uma acao para os eventos de clicar em um botao
	public abstract void addActionShowMessage(Action<string> action);
	
	// Remove uma acao para os eventos de clicar em um botao
	public abstract void removeActionShowMessage(Action<string> action);
	
	// Adiciona uma acao para os eventos de escolher uma foto
	public abstract void addActionPhotoChosen(Action<string, Texture2D> action);
	
	// Remove uma acao para os eventos de escolher uma foto
	public abstract void removeActionPhotoChosen(Action<string, Texture2D> action);
	
	// Mostra uma mensagem na tela com um botao
	public abstract void showMessage(GameObject messageOkDialog, string title, string message, string button);
	
	// Mostra uma mensagem na tela com dois botoes
	public abstract void showMessageOkCancel(string title, string message, string ok_button, string cancel_button);
	
	// Abre a tela para o usuario escolher uma foto
	public abstract void cameraRoll(float x, float y, string photo_name);
	
	// Inicia a tela de loading
	public abstract void loadingMessage(string title, string message);
	
	// Termina a tela de loading
	public abstract void stopLoading();
	
	// Abre uma URL com o navegador dentro do proprio aplicativo
	public abstract bool openUrlInline(string url);
}
