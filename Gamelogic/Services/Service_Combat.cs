using System.Collections.Generic;
using UnityEngine;
using System.Collections;


static class Service_Combat{

    //Returns all enemies in range as a list
    public static List<Player> GetPlayersInRange(Vector3 position,float range)
    {
        List<Player> Players = new List<Player>();

        foreach (Player target in GameLogic.PlayerList)
        {
            float distance = (target.transform.position - position).magnitude;
            if (distance <= range)
                Players.Add(target);
        }

        return Players;
    }

    //Add mover component to Player,use to manipulate motions
    public static Mover AddMover(Player target,Vector3 direction,float speed,float duration)
    {
        var mover = target.gameObject.AddComponent<Mover>();

        //Apply values
        mover.SetValues(target,direction,speed,duration);

        return mover;
    }
}
