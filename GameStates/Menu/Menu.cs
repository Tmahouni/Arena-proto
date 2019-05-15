using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	private Texture2D Background;
	// Use this for initialization
	void Start () {
	Background = (Texture2D)Resources.Load("arenasplashart");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Background);
		
		//Go to CreateServer state
		if(UI.CreateButton(0.4f,0.4f,100,50,"Create server"))
		{
			GameLogic.gameState = GameState.CreateServer;
			Destroy(this);
		}
			
		//Go to JoinServer state
		if(UI.CreateButton(0.4f,0.5f,100,50,"Join server"))
		{
			GameLogic.gameState = GameState.JoinServer;
			Destroy(this);
		}
		
		//TODO
		UI.CreateButton(0.4f,0.6f,100,50,"Credits");
						
		//Quit
		if(UI.CreateButton(0.4f,0.7f,100,50,"Quit"))
			Application.Quit();
		
		
	}
}
