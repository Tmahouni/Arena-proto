using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    private Player owner;
    private CharacterController target;
    private Vector3 direction;
    private float speed;
    private float duration;
    private Mover Chain;

	// Use this for initialization
	void Start ()
	{
        Destroy(this,duration);
	    target = this.transform.parent.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    target.Move(direction * speed * Time.deltaTime);

	}

    void OnControllerColliderHit(ControllerColliderHit other)
    {
       
            Service_Combat.AddMover(owner,Vector3.back,3f,3f);
            Destroy(this);
    }

    public void SetValues(Player Target, Vector3 Direction, float Speed, float Duration)
    {
        this.owner = Target;
        this.target = Target.GetComponent<CharacterController>();
        this.direction = Direction;
        this.speed = Speed;
        this.duration = Duration;
    }


}
