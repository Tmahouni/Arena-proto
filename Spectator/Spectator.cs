using UnityEngine;
using System.Collections;

public class Spectator : MonoBehaviour {

    public float lookSpeed = 2.5f;
    public float moveSpeed = 2.5f;

    public float rotationX = 0.0f;
    public float rotationY = 0.0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!this.networkView.isMine)
            return;

    rotationX += Input.GetAxis("Mouse X")*lookSpeed;
    rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
    rotationY = Mathf.Clamp (rotationY, -90, 90);
    
    transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
    transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    
    transform.position += transform.forward*1*Input.GetAxis("Vertical");
    transform.position += transform.right*1*Input.GetAxis("Horizontal");

	
	}
}
