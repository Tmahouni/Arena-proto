using UnityEngine;
using System.Collections;

public class CombatLogger : MonoBehaviour {

    private Player owner;
    public float damage;
    public float healing;

    public CombatLogger(Player p)
    {
        this.owner = p;
        this.damage = 0;
        this.healing = 0;
    }

    public void Reset()
    {
        this.damage = 0;
        this.healing = 0;
        owner.networkView.RPC("SendLogger", RPCMode.All, damage, healing);
    }

    public void Add(float amount,float heal)
    {
        this.damage += amount;
        this.healing += heal; 
        owner.networkView.RPC("SendLogger", RPCMode.Others, damage, healing);
    }

}
