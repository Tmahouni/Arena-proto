using UnityEngine;
using System.Collections;

//TODO:Destroy gameobjects


public class EffectManager {

	// Update is called once per frame
	public void PlayEffect(string effect,Vector3 position)
	{
	    GameObject g = (GameObject)Resources.Load(effect);


        Network.Instantiate(g, position,Quaternion.identity, 0);

	}

    public GameObject PlayEffect(string effect, Vector3 position,Quaternion rotation)
    {
        GameObject g = (GameObject)Resources.Load(effect);


       var g2 = (GameObject)Network.Instantiate(g, position, Quaternion.identity, 0);
       g2.transform.localRotation = rotation;

       return g2;
        
    }

    public GameObject PersistantEffect(string effect, GameObject target,float duration,Vector3 offset)
    {
        GameObject eff = (GameObject)Resources.Load(effect);
        

        GameObject g = Network.Instantiate(eff, target.transform.position + offset, eff.transform.rotation, 0) as GameObject;

        //Inject effect to target
        g.transform.parent = target.transform;
        //g.particleSystem.loop = true;

        /*foreach (var child_t in g.GetComponentsInChildren<ParticleSystem>() )
        {
            child_t.loop = true;
        }*/

       var destroyer = g.gameObject.AddComponent<Destroyer>();
        destroyer.duration = duration;
        destroyer.parent = g;

        return g;
    }

    public void PersistantEffect(GameObject effect, GameObject target, float duration, Vector3 offset)
    {

        GameObject g = Network.Instantiate(effect, target.transform.position + offset, effect.transform.rotation, 0) as GameObject;

        //Inject effect to target
        g.transform.parent = target.transform;
       // g.particleSystem.loop = true;

        /*foreach (var child_t in g.GetComponentsInChildren<ParticleSystem>())
        {
            child_t.loop = true;
        }*/

        var destroyer = g.gameObject.AddComponent<Destroyer>();
        destroyer.duration = duration;
        destroyer.parent = g;
    }


}
