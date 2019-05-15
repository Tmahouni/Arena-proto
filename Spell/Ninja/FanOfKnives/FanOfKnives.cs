using UnityEngine;
using System.Collections;

public class FanOfKnives : MonoBehaviour {
	
	
	public Vector3 direction;
	public Vector3 spawn;
	public int caster_id;
	
	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
		for(float y = -3; y < 3; y++)
				{
					for(float x = -3; x < 3; x++)
					{
						for(float z = -3; z < 3; z++)
						{
							spawn = new Vector3(x * 0.03f,y * 0.03f,z * 0.03f);
							direction = (spawn - this.transform.position).normalized;
							var ray = new Ray(spawn,direction);
							RaycastHit hit;
							
							
							if(Physics.Raycast(ray,out hit))
							{
								if(hit.transform.gameObject.tag == "Player")
								{
									Player enemy = hit.transform.gameObject.GetComponent<Player>();
									enemy.ApplyDmg(20,caster_id);
								}
							}
							
							
							
						}
					}
				}
	
	}
	
	

	
	
}
