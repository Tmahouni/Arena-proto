using UnityEngine;
using System.Diagnostics;
using System.Collections;

 

public class WowCamera : MonoBehaviour
	
{
 	public Texture2D crosshair_on;
	public Texture2D crosshair_off;
	
	private Texture2D current_crosshair;
	
    public Transform target;


    public float targetHeight = 1.7f;

    public float distance = 10.0f;

 

    public float maxDistance = 20;

    public float minDistance = .6f;

 

    public float xSpeed = 250.0f;

    public float ySpeed = 120.0f;

 

    public int yMinLimit = -80;

    public int yMaxLimit = 80;

 

    public int zoomRate = 40;

 

    public float rotationDampening = 3.0f;

    public float zoomDampening = 5.0f;

 

    private float x = 0.0f;

    private float y = 0.0f;

    private float currentDistance;

    private float desiredDistance;

    private float correctedDistance;

 	
	public bool Rotation_lock = false;

    void Start ()
    {
		
        Vector3 angles = transform.eulerAngles;

        x = angles.x;

        y = angles.y;

 

        currentDistance = distance;

        desiredDistance = distance;

        correctedDistance = distance;

 

        // Make the rigid body not change rotation

        if (rigidbody)
            rigidbody.freezeRotation = true;

    }

    

    /**

     * Camera logic on LateUpdate to only update after all character movement logic has been handled.

     */

    void LateUpdate ()
    {
		

	if(networkView.isMine)
    {


        // Don't do anything if target is not defined

        if (!target)
            return;

        if (!Rotation_lock)
            RotationController();



        y = ClampAngle(y, yMinLimit, yMaxLimit);



        // set camera rotation

        Quaternion rotation = Quaternion.Euler(y, x, 0);



        // calculate the desired distance

        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);

        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

        correctedDistance = desiredDistance;



        // calculate desired camera position

        Vector3 position = target.position - (Camera.mainCamera.transform.rotation * Vector3.forward * desiredDistance + new Vector3(0, -targetHeight, 0));



        // check for collision using the true target's desired registration point as set by user using height

        RaycastHit collisionHit;

        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);



        // if there was a collision, correct the camera position and calculate the corrected distance

        bool isCorrected = false;

        if (Physics.Linecast(trueTargetPosition, position, out collisionHit))
        {

            position = collisionHit.point;

            correctedDistance = Vector3.Distance(trueTargetPosition, position);

            isCorrected = true;

        }



        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance

        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;



        // recalculate position based on the new currentDistance

        position = target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -targetHeight, 0));



        Camera.mainCamera.transform.rotation = rotation;

        Camera.mainCamera.transform.position = position;

    }
    }

 
	

    private static float ClampAngle(float angle, float min, float max)
    {

        if (angle < -360)

            angle += 360;

        if (angle > 360)

            angle -= 360;

        return Mathf.Clamp(angle, min, max);

    }
	
	private void RotationController()
	{
        if (target.GetComponent<UI_Talent>().IsActivated() || target.transform.GetComponent<Player>().HasState(CombatState.Stunned) || target.transform.GetComponent<Player>().HasState(CombatState.Sapped) || target.transform.GetComponent<Player>().IsDead() )
            return;
		
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;

            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            
		target.transform.rotation = Quaternion.Euler(0, Camera.mainCamera.transform.eulerAngles.y, 0);
    }


    
    public void Rotate(Vector3 vec)
    {
        this.x = vec.x;
  
    }
	
	

	void OnGUI()
	{
        if (networkView.isMine)
        {
            if (target.GetComponent<Player>().GetTargetForward(200) != null)
                current_crosshair = crosshair_on;

            else current_crosshair = crosshair_off;


            //crosshair keskelle ruutua
            Rect position = new Rect((Screen.width - current_crosshair.width)/2, (Screen.height -
            current_crosshair.height)/2, current_crosshair.width, current_crosshair.height);

            
            //piirretään crosshair
            GUI.DrawTexture(position, current_crosshair);
        }
	}

}