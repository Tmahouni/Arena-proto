using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class LightBeam : MonoBehaviour {
	
	public int caster_id;
	public float damage = 80;
	private GameObject target;
	private GameObject caster;
	private Stopwatch timer = new Stopwatch();
	// Use this for initialization
	void Start () {
		
		StartCoroutine(DestroyOverTime());
		timer.Start();

        this.networkView.RPC("SendDamage", RPCMode.All, this.damage);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		    rigidbody.velocity = transform.TransformDirection(new Vector3(0,0,800));	
	}
	
	void OnTriggerEnter(Collider other)
	{
		Player p = GameLogic.PlayerList.Find(x=> x.collider == other.collider);
		if(other.gameObject.tag == "Player" && p.ID != this.caster_id)
		{
			
			p.ApplyDmg(this.damage,this.caster_id);
			this.damage = 0;
			Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
			Network.Destroy(GetComponent<NetworkView>().viewID);
		}
		
	}
	
	[RPC]
	void SendID(int i)
	{
		this.caster_id = i;
	}

    [RPC]
    void SendDamage(float i)
    {
        this.damage = i;
    }

	IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(7);
        Network.Destroy(GetComponent<NetworkView>().viewID);
    }
	
	
}
