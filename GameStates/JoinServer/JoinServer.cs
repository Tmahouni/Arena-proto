using UnityEngine;
using System.Collections.Generic;

public class JoinServer : MonoBehaviour
{

    private Texture2D Background;
    private bool refreshing;
    private int server_count;
    //private HostData[] hostData;
    private List<HostData> ListData = new List<HostData>();
	
    // Use this for initialization
    void Start()
    {
		refreshing = false;
        Background = (Texture2D)Resources.Load("arenasplashart");
    }


    void Update()
    {
        if (refreshing)
        {
            if (MasterServer.PollHostList().Length > 0)
            {
                Debug.Log(MasterServer.PollHostList().Length.ToString());
                server_count = MasterServer.PollHostList().Length;
                HostData[] hostData = MasterServer.PollHostList();
				
				ListData = new List<HostData>(hostData);
				refreshing = false;
            }
        }

        

    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background);

        if (UI.CreateButton(0.2f, 0.2f, 150, 50, "Refresh serverlist"))
        {
            RefreshHosts();
        }
		Debug.Log(MasterServer.PollHostList().Length.ToString());
        if (UI.CreateButton(0.2f, 0.7f, 100, 50, "Back to menu"))
        {
            GameLogic.gameState = GameState.Menu;
            Destroy(this);
        }

        if (ListData.Count > 0)
		{
        	for (int i = 0; i < ListData.Count; i++)
        	{
            	if (UI.CreateButton(0.5f, 0.1f * i, 150, 50, ListData[i].gameName))
            	{
					Network.Connect(ListData[i]);
                	GameLogic.gameState = GameState.Prematch;
					Destroy(this);
            	}
        	}
		}


        if (UI.CreateButton(0.2f, 0.1f , 150, 50, "Local"))
        {
            Network.Connect("127.0.0.1",25000);
            GameLogic.gameState = GameState.Prematch;
            Destroy(this);
        }

    }

    void RefreshHosts()
    {
        MasterServer.RequestHostList("Arena_battle");
        refreshing = true;
    }
}
