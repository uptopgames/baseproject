using UnityEngine;
using System.Collections;
using CodeTitans.JSon;

public class CoinsManager : MonoBehaviour
{
	protected class CoinsConnection
	{
		public Coins.CoinsCallback callback;
		public int userId;
		
		public CoinsConnection(int userId, Coins.CoinsCallback callback)
		{
			this.callback = callback;
			this.userId = userId;
		}
	}
	
	private void OnReceiveTotal(string error, IJSonObject data, object state)
	{
		CoinsConnection connection = (CoinsConnection)state;
		if (connection.callback != null)
			connection.callback(connection.userId, (!data.IsNull() && !data.IsError()) ? data.ToString().ToInt32() : 0);
	}
	
	public void GetTotal(int userId, Coins.CoinsCallback callback) {
		GameJsonAuthConnection request = new GameJsonAuthConnection(
			Flow.URL_BASE + "login/items/get_user_coins.php", OnReceiveTotal);

			request.connect(
				(userId > 0)
					? new WWWForm().Add("user_id", userId)
					: new WWWForm().Add("_", "empty"),
				new CoinsConnection(userId, callback)
			);
	}
}
