using UnityEngine;
using System.Collections;

public class DialogCancel : MonoBehaviour 
{

	// Use this for initialization
	void BaseCancel()
	{
		Debug.Log("cancelado pelo metodo padrao");
		gameObject.SetActive(false);
	}
}
