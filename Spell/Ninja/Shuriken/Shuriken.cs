using UnityEngine;
using System.Collections;

public class Shuriken : MonoBehaviour {
	
	
	public int caster_id;
    public float damage = 20;
    private Player owner;

	// Use this for initialization
	void Start ()
	{
	    owner = GameLogic.PlayerList.Find(x => x.ID == caster_id);
        this.damage += 5 * owner.TalentTree.Find(x => x is Talent.SnakeBite).GetRank();
        this.networkView.RPC("SendDamage", RPCMode.All, this.damage);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidbody.velocity = transform.TransformDirection(Vector3.forward * 180);
		transform.Rotate(0, 0, 200 * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider other)
	{
        Debug.Log("On trigger..");
		Player target = GameLogic.PlayerList.Find(x=> x.collider == other.collider);

		if(other.gameObject.tag == "Player" && target.ID != this.caster_id)
		{		
			target.ApplyDmg(this.damage,this.caster_id);
			target.BuffList.Add(new Buff.Debuff_Slow(3,3));

            Debug.Log("hit player");

            /*//Silence rune
		    var t = owner.TalentTree.Find(x => x is Talent.TongueSlicer);
                target.BuffList.Add(new Buff.Debuff_Silence(0.33f * t.GetRank()));*/

			this.damage = 0;
			Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
			Network.Destroy(this.gameObject);
            
		}
        if (other.gameObject.name == "Terrain")
        {
            Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
            Network.Destroy(this.gameObject);

        }
	}



	[RPC]
	void SendID(int i)
	{
		this.caster_id = i;
	}

    [RPC]
    void SendDamage(float d)
    {
        this.damage = d;
    }


}
