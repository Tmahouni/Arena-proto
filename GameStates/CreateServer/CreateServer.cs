using UnityEngine;
using System.Collections;

public class CreateServer : MonoBehaviour {
	
	private Texture2D Background;

    

	//server variables
	private string servername = "Untitled";
	
	// Use this for initialization
	void Start () {
		Background = (Texture2D)Resources.Load("arenasplashart");
	}
	
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Background);
		
		//Start Server
		UI.CreateTextLabel(0.6f,0.3f,100,100,"Server name:",Color.white);
		servername = UI.CreateTextBox(0.7f,0.3f,200,50,servername);
		
		if(UI.CreateButton(0.2f,0.3f,100,100,"Start Server"))
		{
            StartServer();
            GameLogic.gameState = GameState.Prematch;
			Destroy(this);
		}
		
		if(UI.CreateButton(0.2f,0.8f,100,100,"Back to Menu"))
		{
			GameLogic.gameState = GameState.Menu;
			Destroy(this);
		}
		
	}
	
	
	void StartServer()
	{
	Network.InitializeServer(32,25000,!Network.HavePublicAddress() );
	MasterServer.RegisterHost("Arena_battle",servername,"ZB network testi");
	}
}
