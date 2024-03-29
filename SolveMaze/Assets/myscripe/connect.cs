using UnityEngine;
using System.Collections;

public class connect : MonoBehaviour {

	
		
	private float myFloat = 0.05f;
	//DontDestroyOnLoad(this);
	public string remoteIP= "127.0.0.1";
	public int remotePort= 25000;
	public int listenPort= 25000;
	public string remoteGUID= "";
	public bool useNat= false;
	public bool GameStart= false;
	private string connectionInfo= "";
	void  Awake (){
	}
	
	void  OnGUI (){
		GUILayout.BeginHorizontal();
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			useNat = GUILayout.Toggle(useNat, "Use NAT punchthrough");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
	
			GUILayout.BeginVertical();
			if (GUILayout.Button ("Connect"))
			{
				if (useNat)
				{
					if (remoteGUID == null)
						Debug.LogWarning("Invalid GUID given, must be a valid one as reported by Network.player.guid or returned in a HostData struture from the master server");
					else
						Network.Connect(remoteGUID);
				}
				else
				{
					Network.Connect(remoteIP, remotePort);
				}
				
			}
			if (GUILayout.Button ("Start Server"))
			{
				Network.InitializeServer(32, listenPort, useNat);
				foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
					go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
			}
			GUILayout.EndVertical();
			if (useNat)
			{
				remoteGUID = GUILayout.TextField(remoteGUID, GUILayout.MinWidth(145));
			}
			else
			{
				remoteIP = GUILayout.TextField(remoteIP, GUILayout.MinWidth(100));
				remotePort = int.Parse(GUILayout.TextField(remotePort.ToString()));
			}
		}
		else
		{
			if (useNat)
				GUILayout.Label("GUID: " + Network.player.guid + " - ");
			GUILayout.Label("Local IP/port: " + Network.player.ipAddress + "/" + Network.player.port);
			GUILayout.Label(" - External IP/port: " + Network.player.externalIP + "/" + Network.player.externalPort);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("Disconnect"))
				Network.Disconnect(200);
	
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	void  OnServerInitialized (){
		if (useNat)
			Debug.Log("==> GUID is " + Network.player.guid + ". Use this on clients to connect with NAT punchthrough.");
		Debug.Log("==> Local IP/port is " + Network.player.ipAddress + "/" + Network.player.port + ". Use this on clients to connect directly.");
	}
	
	void  OnConnectedToServer (){
		// Notify our objects that the level and the network is ready
		foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);		
	}
	
	void  OnDisconnectedFromServer (){
		if (this.enabled != false)
			Application.LoadLevel(Application.loadedLevel);		
	}

}
