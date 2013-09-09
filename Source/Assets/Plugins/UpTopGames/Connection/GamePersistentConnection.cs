using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using CodeTitans.JSon;
using System.Text.RegularExpressions;
using System.Threading;

public class GamePersistentConnection: MonoBehaviour
{
	private const string SAVE_CONNECTION_LIST = "connection:list";
	
	public class PConn
	{
		public string id;
		public string url;
		public byte[] data;
		public List<DictionaryEntry> headers;
		
		[XmlIgnore]
		private Semaphore semaphore = new Semaphore(1, 1);
		
		public PConn()
		{
		}
		
		public PConn(string id, string url, byte[] data, List<DictionaryEntry> headers)
		{
			this.id = id;
			this.url = url;
			this.data = data;
			this.headers = headers;
		}
		
		// Libera o semaforo
		public void release()
		{
			try
			{
				semaphore.Release();
			}
			catch (SemaphoreFullException) {}
		}
		
		// Envia a conexao ao servidor
		public void send(GameJsonAuthConnection.ConnectionAnswerWithState callback)
		{
			if (!semaphore.WaitOne(0))
			{
				//Debug.Log("Pero no.");
				return;
			}
			
			//Debug.Log("Yaarrrrr.");
			GameJsonAuthConnection conn = new GameJsonAuthConnection(url, callback);
			
			// Cria o header
			Hashtable table = new Hashtable(headers.Count);
			foreach (DictionaryEntry entry in headers) table.Add(entry.Key, entry.Value);
			
			// Decide se e uma conexao binaria
			string delimiter;
			if (!table.Contains("Content-Type")) delimiter = "&";
			else
			{
				Match match = Regex.Match(table["Content-Type"].ToString(), @"boundary=\""(.*)""");
				if (match.Success) delimiter = match.Groups[1].Value;
				else delimiter = "&";
			}
			
			byte[] fdata = (byte[]) data.Clone();
			
			string adata = null;
			
			if (delimiter == "&")
			{
				adata = "";
				if (data.Length != 0) adata += "&";
				adata += "device=" + WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier.Replace("-","")) + "&token=" + WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN.ToString()));
			}
			else
			{
				adata = @"
Content-Type: text/plain; charset=""utf-8""
Content-disposition: form-data; name=""device""

" + SystemInfo.deviceUniqueIdentifier.Replace("-","") + @"
--" + delimiter + @"
Content-Type: text/plain; charset=""utf-8""
Content-disposition: form-data; name=""token""

" + Save.GetString(PlayerPrefsKeys.TOKEN.ToString()) + @"
--" + delimiter + @"--
";
				
				System.Array.Resize<byte>(ref fdata, fdata.Length - 4);
			}
			
			byte[] bdata = System.Text.Encoding.ASCII.GetBytes(adata);
			int size = fdata.Length;
			System.Array.Resize<byte>(ref fdata, fdata.Length + bdata.Length);
			bdata.CopyTo(fdata, size);
			
			conn.connect(fdata, table, id);
		}
	}
	
	private static Dictionary<string, PConn> connections;
	private static HashSet<string> progress;
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		loadConnections();
		
		progress = new HashSet<string>();
		
		StartCoroutine(sendConnection());
	}
	
	private IEnumerator sendConnection()
	{
		while (true)
		{
			// Aguarda a conexao com a internet ser estabelecida
			while (!Info.HasConnection())
				yield return new WaitForSeconds(1);
			
			// Envia todas as conexoes pendentes
			lock (connections)
			{
				foreach (KeyValuePair<string, PConn> entry in connections)
				{
					Debug.Log("Trying: " + entry.Value.id);
					entry.Value.send(handleConnection);
				}
			}
			
			yield return new WaitForSeconds(2);
		}
	}
	
	// Verifica se a conexao foi bem sucedida
	private static void handleConnection(string error, IJSonObject data, System.Object state)
	{
		string id = state.ToString();
		
		// Obtem a conexao retornada
		PConn conn = null;
		lock(connections)
		{
			if (connections.ContainsKey(id)) conn = connections[id];
		}
		
		// Se nao ha conexao, termina
		if (conn == null) return;
		
		// Se houve erro, libera a conexao e termina
		if (error != null)
		{
			conn.release();
			return;
		}
		
		// Remove e libera a conexao
		removeConnection(id);
		conn.release();
	}
	
	// Obtem a lista de conexoes
	private static void loadConnections()
	{
		connections = new Dictionary<string, PConn>();
		
		// Obtem o XML da lista de conexoes
		string list_data = Save.GetString(SAVE_CONNECTION_LIST);
		if (list_data.IsEmpty()) return;
		
		// Obtem a lista a partir do XML
		XmlSerializer serializer = new XmlSerializer(typeof(List<PConn>));
		StringReader reader = new StringReader(list_data);
		List<PConn> list = (List<PConn>) serializer.Deserialize(reader);
		
		// Monta o dicionario
		lock (connections)
		{
			foreach (PConn conn in list)
				connections.Add(conn.id, conn);
		}
	}
	
	// Salva a lista de conexoes
	private static void saveConnections()
	{
		List<PConn> list;
		
		lock (connections)
		{
			// Cria a lista a partir do dicionario de conexoes
			list = new List<PConn>(connections.Count);
			foreach (KeyValuePair<string, PConn> entry in connections)
				list.Add(entry.Value);
		}
		
		// Serializa a lista e salva
		XmlSerializer serializer = new XmlSerializer(typeof(List<PConn>));
		using (StringWriter writer = new StringWriter())
		{
			serializer.Serialize(writer, list);
			Save.Set(SAVE_CONNECTION_LIST, writer.ToString(), true);
		}
	}
	
	// Adiciona uma conexao persistente a lista
	public static void addConnection(string id, string url, byte[] data, Hashtable headers)
	{
		// Remove da lista de conexoes em progresso
		finished(id);
		
		// Adiciona a lista de conexoes
		lock (connections)
		{
			if (connections.ContainsKey(id)) return;
			
			List<DictionaryEntry> list = new List<DictionaryEntry>();
			foreach (DictionaryEntry entry in headers) list.Add(entry);
			
			connections.Add(id, new PConn(id, url, data, list));
			saveConnections();
		}
	}
	
	// Remove uma conexao da lista
	public static void removeConnection(string id)
	{
		lock (connections)
		{
			connections.Remove(id);
			saveConnections();
		}
	}
	
	// Indica que uma conexao foi iniciada
	public static void started(string id)
	{
		lock (progress) progress.Add(id);
	}
	
	// Indica que uma conexao terminou
	public static void finished(string id)
	{
		lock (progress) progress.Remove(id);
	}
	
	// Verifica se a conexao foi enviada
	public static bool sent(string id)
	{
		if (id == null) return false;
		
		lock (progress) if (progress.Contains(id)) return false;
		lock (connections) return !connections.ContainsKey(id);
	}
	
	// Elimina todas as conexoes pendentes
	public static void clean()
	{
		Save.Delete(SAVE_CONNECTION_LIST);
		connections = new Dictionary<string, PConn>();
	}
}
