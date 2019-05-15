using UnityEngine;
using System.Collections;

public class PoisonBomb : MonoBehaviour
{

    Vector3 speed = new Vector3(0, -2.5f, 70);
    public Player owner;
    private bool exploded = false;

    public GameObject effect;

    // Use this for initialization
    void Start()
    {
        this.rigidbody.velocity = this.transform.TransformDirection(speed);
        StartCoroutine(DestroyOverTime());

    }

    // Update is called once per frame
    void Update()
    {

        var enemy = Service_Combat.GetPlayersInRange(this.transform.position, 13f);

        if(enemy == null) return;


    if(exploded)
    {    
        effect.SetActive(true);

        foreach (var player in enemy)
        {
            if(player != owner)
                player.ApplyDmg(1,owner.ID);
        }
    }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Terrain")
        {

            this.exploded = true;
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