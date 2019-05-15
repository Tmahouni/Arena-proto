using UnityEngine;
using System.Collections;



public class WowCharacterMotor : MonoBehaviour {

	public float jumpSpeed = 52.0f;

	public float gravity = 15.0f;

	public float runSpeed = 7.0f;

	private bool grounded = false;

	public Vector3 moveDirection = Vector3.zero;

	public Player player;
	
	public Transform LeftLeg,RightLeg;




void FixedUpdate ()
{

this.runSpeed = player.movement_speed;


if(player.HasState(CombatState.Stunned) || player.HasState(CombatState.Sapped) || player.HasState(CombatState.Immobilized) || player.IsDead())
		{ moveDirection.x = 0; moveDirection.z = 0;}

  	if(!networkView.isMine)
  	    return;


    if(grounded && !player.HasState(CombatState.Stunned) && !player.IsDead() && !player.HasState(CombatState.Sapped) && !player.HasState(CombatState.Immobilized)) 
    {

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0,Input.GetAxis("Vertical"));

        moveDirection = transform.TransformDirection(moveDirection);

        moveDirection *= runSpeed;
		
		//ANIMATION STUFF
		if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > .1 || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .1) 
		{
			player.transform.GetComponentInChildren<Animation>().animation.Stop("idle");
            if (!player.transform.GetComponentInChildren<Animation>().animation.isPlaying)
                player.Animator.PlayAnimation("run", 1.7f);
		}												
		else 
		{
			player.transform.GetComponentInChildren<Animation>().animation.Stop("run");
			if(!player.transform.GetComponentInChildren<Animation>().animation.isPlaying)
				player.transform.GetComponentInChildren<Animation>().animation.Play("idle");	
		}
		
        // Sideways movement
        if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0f) //|| (Input.GetAxis("Vertical") < 0f && Input.GetAxis("Horizontal") < 0f)) 
		{
            moveDirection *= 0.7f;
		}
	
        // Jump!
        if(Input.GetButton("Jump"))
        {
			player.Animator.PlayAnimation("jump",1f);
            moveDirection.y = jumpSpeed;
		}
    }

    if (moveDirection.x != 0 || moveDirection.z != 0)
        player.CombatFlags.Add(CombatFlag.OnMove);


    //Apply gravity
    moveDirection.y -= gravity * Time.deltaTime;

   

    //Move controller
    CharacterController controller = GetComponent<CharacterController>();

    var flags = controller.Move(moveDirection * Time.deltaTime);
    
		
	
	
    grounded = (flags & CollisionFlags.Below) != 0;

			
	if(player.Animator.prev_animation == "jump" && grounded && !player.transform.GetComponentInChildren<Animation>().animation.IsPlaying("jump") )
		{
		player.Animator.PlayAnimation("jump_landing", 1.6f);
		player.Animator.prev_animation = "";
			StartCoroutine(LandingDelay());
		}
			
}

 IEnumerator LandingDelay()
	{
		player.movement_speed -= 15;
		yield return new WaitForSeconds(0.05f);
		player.movement_speed += 15;
	}
	
void LateUpdate()
	{
        if (!networkView.isMine)
            return;
        
      if(!player.HasState(CombatState.Stunned))
       {
         LeftLeg.transform.localEulerAngles = new Vector3(Input.GetAxis("Horizontal") * 25.5f, LeftLeg.transform.localEulerAngles.y, LeftLeg.transform.localEulerAngles.z);
         RightLeg.transform.localEulerAngles = new Vector3(Input.GetAxis("Horizontal") * 25.5f, RightLeg.transform.localEulerAngles.y, RightLeg.transform.localEulerAngles.z);    
       }

	}
	

}

