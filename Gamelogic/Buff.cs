using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public abstract class Buff : MonoBehaviour{

	public CombatState state; //Used to determine what kind of effect buff provides, Stun,Slow,Stealth etc.
	public bool applied = false;
	public float duration;

	public List<CombatFlag> DeleteFlags = new List<CombatFlag>(); //Which flags remove buff
	
	private Texture2D icon; //Kuvake 

    //Stacking buff
    public bool Stackable = false;
    public int current_stack = 0;
    public int max_stack;

	public abstract void Apply(Player player); 
	public abstract void DeApply(Player player);
	
	
	public void ReduceDuration()
       {
           if (this.duration > 0)
               this.duration -= 60 * Time.deltaTime;
          
           else
               this.duration = 0;
       }


    public void Add(Player target)
    {
        UnityEngine.Debug.Log("Apying buff:" +this.GetType().ToString());
        target.networkView.RPC("SendBuff", RPCMode.All, this.GetType().ToString());
    }

	public class Debuff_Stun : Buff
	{
	    private float dur;

       public Debuff_Stun(float seconds)
       {
		this.duration = (int)(60 * seconds);
		this.applied = false;
        this.state = CombatState.Stunned;
        dur = seconds;

       }

       public override void Apply(Player player)
       {
            var effect = new EffectManager();
            effect.PersistantEffect("Effect_Stun",player.gameObject,dur,Vector3.zero);
			this.applied = true;

       }
		
		public override void DeApply(Player player)
		{
			;
		}

     
   }
	
	public class Debuff_Silence : Buff
	{
	    private float dur;

	     public Debuff_Silence(float seconds)
	     {
             this.duration = (int)(60 * seconds);
             this.applied = false;
             this.state = CombatState.Silenced;
	         dur = seconds;
	     }

	    public override void Apply(Player player)
	    {
	        var effect = new EffectManager();
            effect.PersistantEffect("Effect_Silence",player.gameObject,dur,Vector3.up);
            

	        this.applied = true;
	    }

	    public override void DeApply(Player player)
	    {
            ;
	    }

	}
	
	public class Debuff_Slow : Buff
    {
        float Power;
		
       public Debuff_Slow(float seconds,int power)
       {
		this.Power = power;
		this.duration = (int)(60 * seconds);
		this.applied = false;
        this.state = CombatState.Slowed;
        this.max_stack = 5;
			
       }

       public override void Apply(Player player)
       {
 
			player.movement_speed -= Power;

       }
		
		public override void DeApply(Player player)
		{
			player.movement_speed += Power;
			
		}

     
   }

    public class Debuff_Immobilize : Buff
    {
        public Debuff_Immobilize()
        {
            this.duration = 4f * 60;
            this.state = CombatState.Immobilized;
        }

        public override void Apply(Player player)
        {
            var effect = new EffectManager();
            effect.PersistantEffect("Effect_AngelPrison", player.gameObject, 4f, Vector3.down);
        }
        public override void DeApply(Player player)
        {
            ;
        }
    }
	
    public abstract class DotBuff : Buff
    {
        public Player owner;
        public Stopwatch TickTimer;
        public float Tick_interval;
        public abstract void OnTick(Player p);
    }



}
