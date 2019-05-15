using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour  {

	private Texture2D Logo;
	private AudioClip sound;
	
	void Start()
	{
		Logo = (Texture2D)Resources.Load("Logo");
		sound = (AudioClip)Resources.Load("intro2");
		AudioSource.PlayClipAtPoint(sound,Camera.main.transform.position);
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Logo);
		
		
		if(Input.anyKeyDown)
		{
			GameLogic.gameState = GameState.Menu;
			Destroy(this);
		}
	}
}
