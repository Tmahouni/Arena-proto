using UnityEngine;
using System.Collections;

public class NetworkCamera : MonoBehaviour {

	void Awake() 
	{
    if(networkView.isMine)
        camera.enabled = true;
    else
        camera.enabled = false;
    
}
	
	// Update is called once per frame
	void Update () {
	
	
	}
}
