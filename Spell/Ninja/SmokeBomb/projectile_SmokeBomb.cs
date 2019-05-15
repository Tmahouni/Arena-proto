using UnityEngine;
using System.Collections;

public class projectile_SmokeBomb : MonoBehaviour {
	
	Vector3 speed = new Vector3(0,-4.5f,60);
    public Player owner;
    private bool exploded = false;
    public int owner_id;

    //Dragged in inspector
    public GameObject effect;

    int dr_multiplier;
    

	// Use this for initialization
	void Start () 
	{
        
	this.rigidbody.velocity = this.transform.TransformDirection(speed);
	StartCoroutine(DestroyOverTime());

    dr_multiplier = owner.TalentTree.Find(x => x is Talent.SmokeShield).GetRank();
	}
	
	// Update is called once per frame
	void Update() 
	{
        if (exploded)
        {
            effect.SetActive(true);

           var players = Service_Combat.GetPlayersInRange(this.transform.position, 13);

           foreach (Player p in players)
           {
               if (p == owner && !p.BuffList.Exists(x=> x is Spell.Spell_SmokeBomb.SmokeShield))
                   p.BuffList.Add(new Spell.Spell_SmokeBomb.SmokeShield(30 + 10 * dr_multiplier));
				
				
				if(!owner.IsEnemy(p))
					effect.GetComponent<ParticleSystem>().startColor = new Color(0,0,0,0.01f);
           }
        }
		
		if(networkView.isMine)
		{
			effect.GetComponent<ParticleSystem>().startColor = new Color(0,0,0,0.01f);
			//effect.GetComponent<ParticleSystem>().renderer.material.color = new Color(255,255,255,0.01f);
		}
	}
	
	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Terrain")
		{
            exploded = true;
			this.rigidbody.velocity = Vector3.zero;
		}
	}
	
	
	IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(15);
		Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
        Network.Destroy(GetComponent<NetworkView>().viewID);
    }


}
