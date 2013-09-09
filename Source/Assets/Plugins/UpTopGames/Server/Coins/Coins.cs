using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Coins {
	
    // Callback 
	public delegate void CoinsCallback(
        int userId, int count
    );
	
	private static CoinsManager manager
	{
		get
		{
			if (Coins._manager != null)
				return Coins._manager;
			
			GameObject _coinsObj = Flow.config;
	
			CoinsManager manager = _coinsObj.GetComponent<CoinsManager>();
			if (manager == null) manager = _coinsObj.AddComponent<CoinsManager>();
			
			Coins._manager = manager;
			
			return Coins._manager;
		}
	}
	private static CoinsManager _manager;

    // Retorna a quantidade de coins de um determinado usuario no callback
    public static void GetBalance(int userId, CoinsCallback callback)
    {
        manager.GetTotal(userId, callback);
    }

    // Retorna a quantidade de coins do proprio usuario no callback
    public static void GetBalance(CoinsCallback callback)
    {
        manager.GetTotal(0, callback);
    }
}
