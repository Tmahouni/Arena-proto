using UnityEngine;
using System.Collections;
using System.Diagnostics;


public class Javelin : MonoBehaviour {
	
	public int caster_id;	
	public float stun_len; //Stunnin pituus
	private Stopwatch timer = new Stopwatch();
	
	void Start () {
		timer.Start();
        StartCoroutine(DestroyOverTime());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		rigidbody.velocity = transform.TransformDirection(new Vector3(0,0,130));

        this.stun_len += 0.03f;
	}
	
	
	
	void OnTriggerEnter(Collider other)
	{
		Player p = GameLogic.PlayerList.Find(x=> x.collider == other.collider);
		if(other.gameObject.tag == "Player" && p.ID != this.caster_id)
		{
            p.networkView.RPC("Stun", RPCMode.All, stun_len);
            p.ApplyDmg(30f, caster_id);
			Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
			Network.Destroy(GetComponent<NetworkView>().viewID);
		}
        if (other.gameObject.name == "Terrain")
        {
			Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
            Network.Destroy(GetComponent<NetworkView>().viewID);
        }
		
	}
	
	
	[RPC]
	void SendID(int i)
	{
		this.caster_id = i;
	}
	
    IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(5);
		Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
        Network.Destroy(GetComponent<NetworkView>().viewID);
    }
	
	IEnumerator asd()
    {
        yield return new WaitForSeconds(0.5f);
		this.stun_len += 0.5f;
    }
}
