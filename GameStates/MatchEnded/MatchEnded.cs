using UnityEngine;
using System.Collections;

public class MatchEnded : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        
        UI.CreateTextLabel(0.5f,0.5f,300,300,"Winner is " + GameLogic.GameMode.winners.p_Name,Color.red);
    }
}
