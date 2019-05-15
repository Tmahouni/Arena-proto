
using UnityEngine;
using System.Collections;

//TODO:Team selection,Polish class selection

public class Prematch : MonoBehaviour
{
    private Texture2D background = Resources.Load("arenasplashart") as Texture2D;
    private string @class = null;
    private string @team = null;
    private string @name = "Unnamed";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Screen.showCursor = true;
	}

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height),background);

        
        if (UI.CreateButton(0.3f, 0.7f, 100, 100, "SPECTATE"))
        {
            var Spectator = Resources.Load("Spectator");

            Network.Instantiate(Spectator,GameLogic.SpawnPoints[0],Quaternion.identity,0);
            GameLogic.gameState = GameState.InGame;
            Destroy(this);
        }

         //Class selection
         SelectClass();
         SelectTeam();
        @name = SelectName();
        if(@class == null || @team == null) 
            return;

        if (UI.CreateButton(0.3f, 0.5f, 100, 100, "PLAY"))
        {
            var network = GameObject.Find("NetworkManager").GetComponent<NetworkHandler>();
            network.PlayerFactory(@class,@team,@name);
            GameLogic.gameState = GameState.InGame;
            Debug.Log(@class);
            Destroy(this);
        }

 
    }

    private void SelectClass()
    {
        if (UI.CreateButton(0.5f, 0.5f, 128, 128, (Texture2D)Resources.Load("Ninja")))
            @class = "Ninja";
        if (UI.CreateButton(0.62f, 0.5f, 128, 128, (Texture2D)Resources.Load("Angel")))
            @class = "Angel";
    }

    private void SelectTeam()
    {
        if (UI.CreateButton(0.5f, 0.3f, 100, 100,"Team 1"))
            @team = "Team1";
        if (UI.CreateButton(0.6f, 0.3f, 100, 100, "Team 2"))
            @team = "Team2";
    }

    private string SelectName()
    {
       return UI.CreateTextBox(0.7f, 0.7f, 120, 20, @name);
    }
}
