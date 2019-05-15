using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameLogic : MonoBehaviour
{


	public static List<Player> PlayerList = new List<Player>();
	public static int lastPlayerId = 0;
	public static GameState gameState;

    public static GameMode GameMode;
	
    //Move to map at some point
    public static List<Vector3> SpawnPoints = new List<Vector3>(); 

	//Retrieves new ID to player when he logs in
	public static int GetLastID()
	{
		lastPlayerId++;
		return lastPlayerId;
	}
	
	void Awake()
	{
		gameState = GameState.Intro;
        GameMode = new GameMode.TeamCompetitive();

        SpawnPoints.Add(new Vector3(986f,55f,926.7f));
        SpawnPoints.Add(new Vector3(983f, 54.6f, 1046.2f));
        
	}

    void Update()
    {
        Application.runInBackground = true;
        //Run WinCondition checks
        var b = GameMode.Run();

        if (b == false)
        {
            Debug.Log(("Ending match"));
            gameState = GameState.MatchEnded;
        }

    ChangeState(gameState);
		
		
			//Buffit
			HandleBuffs();
			BuffFixes();	
		
			//Spell
			HandleSpell();
			
			//HandleFlags();
			
	}
	
	
	
	 private void HandleBuffs()
     {

         /*foreach (Player p in PlayerList)
         {
             foreach (var dotter in p.BuffList)
             {
                 if (dotter is Buff.DotBuff)
                 {
                     var dot = dotter as Buff.DotBuff;
                     dot.OnTick(p);
                 }
             }
         }*/


       //Reduce duration and apply buffs
       for (int i = 0; i < PlayerList.Count; i++)
       {
			
           for (int j = 0; j < PlayerList[i].BuffList.Count; j++)
           {
               if(PlayerList[i].BuffList[j].duration != 0xFFF)
                PlayerList[i].BuffList[j].ReduceDuration();

               if (!PlayerList[i].BuffList[j].applied)
               {
                   PlayerList[i].BuffList[j].Apply(PlayerList[i]);
                   PlayerList[i].BuffList[j].applied = true;
               }

               if (PlayerList[i].BuffList[j].duration <= 0)
               {
                   PlayerList[i].BuffList[j].DeApply(PlayerList[i]);
                   PlayerList[i].BuffList.RemoveAt(j);
               }

             
           }
       }

		 
      }
	
	private void HandleSpell()
       {
           foreach (Player p in PlayerList)
           {
               foreach (Spell s in p.Spellbook)
               {

                   s.ReduceCooldown();
               }
           }
       }
	
	
	
	private void HandleFlags()
	{
		foreach (Player p in PlayerList)
           {
			
               foreach (Buff b in p.BuffList)
               {
					
					foreach(CombatFlag c in b.DeleteFlags)
					{
						
						if(p.CombatFlags.Contains(c))
						{
						b.DeApply(p);
						p.BuffList.Remove(b);
						}
					}
				
				}
				p.CombatFlags.Clear();
            }
      }
	
	
	private void BuffFixes()
	{
		
		//ShadowWalk,end when players mana reaches 0
           foreach (Player p in PlayerList)
           {
			List<Buff> stealth = new List<Buff>();
               foreach (Buff b in p.BuffList)
               {
					if(b is Spell.ShadowWalk.Buff_ShadowWalk && p.energy <= 1)
					{
						 stealth.Add(b);
						Debug.Log("out of energy..");
					}
               }
			
			if(stealth.Count >= 1)
			{
				stealth[0].DeApply(p);
				p.BuffList.Remove(stealth[0]);
			}
			
           }
	}
	
	private void ChangeState(GameState state)
	{
		

		switch ((int)state)
		{
		case 0: 
			if(this.gameObject.GetComponent<Intro>() == null)
				this.gameObject.AddComponent<Intro>();
			break;
			
		case 1: 
			if(this.gameObject.GetComponent<Menu>() == null)
				this.gameObject.AddComponent<Menu>();
			break;
			
		case 2: 
			if(this.gameObject.GetComponent<CreateServer>() == null)
				this.gameObject.AddComponent<CreateServer>();
			break;
			
		case 3: 
			if(this.gameObject.GetComponent<JoinServer>() == null)
				this.gameObject.AddComponent<JoinServer>();
			break;
			
		case 4: 
			if(this.gameObject.GetComponent<Prematch>() == null)
				this.gameObject.AddComponent<Prematch>();
			break;
		}
	}


	
	
	
	
	
	
}
