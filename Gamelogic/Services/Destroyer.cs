using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{
    public GameObject parent;
    public float duration;

	// Use this for initialization
	void Start () {
        StartCoroutine(DestroyOverTime());
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(duration);
        Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
        Network.Destroy(GetComponent<NetworkView>().viewID);
    }

}
