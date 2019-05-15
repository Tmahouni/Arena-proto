using UnityEngine;
using System.Collections;

public abstract class Talent : MonoBehaviour
{
    //attributes
    public Texture2D icon;
    public int tier;
    private int max_rank;
    private int rank = 0;
    private float pct_per_rank;

    public string tree_location;

    private Player owner; //Owner of this talent

    private void AddRank()
    {
		if(this.rank >= this.max_rank || this.owner.talent_point < 1) return;
		
        this.rank++;
        this.owner.talent_point -= 1;
		
		this.Apply();
    }

    public float GetPct()
    {
       
        //we want to decrease something
        if (pct_per_rank < 0)
            return (1 - (this.rank*Mathf.Abs(this.pct_per_rank))/100);

        else return (1 + (this.rank*this.pct_per_rank)/100);
    }

    public virtual void Apply()
    {
        ;
    }

    public int GetRank()
    {
        return this.rank;
    }
	
    //Entry point to talent from Talent_UI
	public void DrawIconInTree(float x,float y,int index,int tree_index)
	{
        Rect mouse_rect = new Rect(Input.mousePosition.x, Input.mousePosition.y, 10, 10);

		var style = new GUIStyle();
		style.normal.textColor = Color.red;
		
        //Rank
		UI.CreateTextLabel(x + 0.05f,y+0.05f,50,50,this.rank.ToString() + "/" + this.max_rank.ToString(),Color.red);
		
		if(UI.CreateButton(x, y, this.icon.width, this.icon.height, this.icon))
		{
			//Check requirements
			if(this.tier == 2)
			{
				var root_talent = owner.TalentTree[5 * (tree_index-1)];
				if(root_talent.GetRank() < 3) return;
			}
			
			if(this.tier == 3)
			{
				var t2_talent = owner.TalentTree[index-3];
				if(t2_talent.GetRank() < 2) return;
			}
			
			//If requirements are met add rank to talent
			this.AddRank();
		}
        
        if (new Rect(x, y, this.icon.width, this.icon.height).Contains(Event.current.mousePosition))
            Debug.Log("hovering over");
	}



    public abstract class StatTalent : Talent
    {
        public bool IsSelected()
        {
            if (this.max_rank == 1 && this.rank == 1)
                return true;
            else
            {
                return false;
            }
        }

    }

    public abstract class PassiveTalent : Talent
    {
        public abstract void Update();
    }

    //Tier 1 Shadow Three
    //Reduce mana per second of stealth and dark ethereal
    public class MasterOfShadows : StatTalent
    {
        public MasterOfShadows(Player p)
        {
            this.owner = p;
            this.icon = Resources.Load("Talent_ShadowMaster") as Texture2D;
            this.tier = 1;
            this.max_rank = 5;
            this.pct_per_rank = -15f;
            this.rank = 0;
        }
    }

	
    //Tier 1 Combat Tree:Critical Strike
    //Done
    public class CriticalStrike : StatTalent
    {
        public CriticalStrike(Player p)
        {
            this.owner = p;
            this.tier = 1;
            this.max_rank = 5;
            this.pct_per_rank = 500f;
            this.rank = 0;
            this.icon = Resources.Load("Talent_CriticalStrike") as Texture2D;
        }

        public bool Check()
        {
            if (Random.Range(1, 101) <= this.rank*5)
            {
                Debug.Log("Crit!");
                return true;
            }
            return false;
        }
    }

    //Tier 3 Combat Tree:Zen Energy
    //Done,not tested
    public class ZenEnergy : StatTalent
    {
        public ZenEnergy(Player p)
        {
            this.owner = p;

            this.icon = Resources.Load("Talent_ZenEnergy") as Texture2D;
            this.tier = 3;
            this.max_rank = 1;
            this.pct_per_rank = 30f;
            this.rank = 0;
        }

        public override void Apply()
        {
            owner.energy_regen *= 1.3f;
        }
    
    }

	//Implemented
	public class TongueSlicer : StatTalent
    {
        public TongueSlicer(Player p)
        {
			this.owner = p;
			this.icon = Resources.Load("Talent_TongueSlicer") as Texture2D;
            this.tier = 2;
            this.max_rank = 3;
            this.pct_per_rank = 33;
            this.rank = 0;
        }

       
    }
	
	//Implemented
	public class Neurotoxic : StatTalent
    {
        public Neurotoxic(Player p)
        {
			this.owner = p;
			this.icon = Resources.Load("Talent_Neurotoxic") as Texture2D;
            this.tier = 3;
            this.max_rank = 1;
            this.rank = 0;
        }  
    }

    //Implemented
    public class MultiShuriken : StatTalent
    {
        public MultiShuriken(Player p)
        {
			this.owner = p;
            this.icon = Resources.Load("Talent_MultiShuriken") as Texture2D;
            this.tier = 3;
            this.max_rank = 1;
            this.rank = 0;
        }
    }

    //Implemented
    //Double fan of knives
    public class EndlessKnives : StatTalent
    {
        public EndlessKnives(Player p)
        {
            this.owner = p;
            this.tier = 3;
            this.max_rank = 1;
            this.icon = Resources.Load("Talent_EndlessKnives") as Texture2D;
        }
    }

    //Implemented and tested
    public class ZenBlood : PassiveTalent
    {
		//used to calculate heal
        private float energy;

        public ZenBlood(Player p)
        {
            this.rank = 0;
            this.max_rank = 3;
            this.tier = 2;
            this.owner = p;
            this.energy = 0;
            this.icon = Resources.Load("Talent_ZenBlood") as Texture2D;
        }

        public override void Update()
        {

            if (owner.energy > energy)
            {
                energy = owner.energy;
            }

            if (owner.energy < energy)
            {
                float cutoff = energy - owner.energy;
                owner.HP += (int)cutoff/5 * this.GetRank();
                energy = owner.energy;
            }
        }
    }

    //Implemented and tested
    public class DoubleScimitar : StatTalent
    {
        public DoubleScimitar(Player p)
        {
			this.owner = p;
            this.tier = 3;
            this.rank = 0;
            this.max_rank = 1;
			this.icon = Resources.Load("Talent_DoubleScimitar") as Texture2D;
        }
    }
	
	//TODO:
	public class ShadowWalker : StatTalent
	{
		public ShadowWalker(Player p)
		{
            this.owner = p;
            this.icon = Resources.Load("Talent_ShadowWalker") as Texture2D;
            this.tier = 2;
            this.max_rank = 3;
		}
	}
	
	//FoK cooldown reduction
	public class DoubleTime : StatTalent
	{
		public DoubleTime(Player p)
		{
            this.icon = Resources.Load("Talent_DoubleTime") as Texture2D;
			this.owner = p;
			this.pct_per_rank = -33f;
			this.tier = 2;
            this.max_rank = 3;
		}

	}

    //Max energy
    public class ZenHatred : StatTalent
    {
        public ZenHatred(Player p)
        {
            this.icon = Resources.Load("Talent_ZenHatred") as Texture2D;
            this.owner = p;
            this.tier = 2;
            this.max_rank = 3;
        }

        public override void Apply()
        {
            this.owner.energy_max += 10;
        }

    }

    //Slow to shuriken
    public class SnakeBite : StatTalent
    {
        public SnakeBite(Player p)
        {
            this.icon = Resources.Load("Talent_SnakeBite") as Texture2D;
            this.owner = p;
            this.tier = 1;
            this.max_rank = 5;
        }


    }

    public class SmokeShield : StatTalent
    {
        public SmokeShield(Player p)
        {
            this.icon = Resources.Load("Talent_SmokeShield") as Texture2D;
            this.owner = p;
            this.tier = 2;
            this.max_rank = 3;
        }


    }

    public class DarkEthereal : StatTalent
    {
        public DarkEthereal(Player p)
        {
            this.icon = Resources.Load("Talent_DarkEthereal") as Texture2D;
            this.owner = p;
            this.tier = 3;
            this.max_rank = 1;
        }


    }


    //Angel TalentSet
    ////////////////

    //Tree:Heal 

    public class HealingSwords : StatTalent
    {
        public HealingSwords(Player owner)
        {
            this.owner = owner;
            this.max_rank = 5;
            this.tier = 1;

        }
        public override void Apply()
        {
            var Spell = owner.Spellbook.Find(x => x is Spell.Spell_AngelSword) as Spell.Spell_AngelSword;
            Spell.heal_amount += 5;   
        }
    }

    public class RegeneratingHealing : PassiveTalent
    {
        private Spell Spell;

        public RegeneratingHealing(Player owner)
        {
            this.icon = Resources.Load("Talent_HealingMaster") as Texture2D;
            this.owner = owner;
            this.tier = 1;
            this.max_rank = 5;
        }

        public override void Update()
        {
           Spell = owner.Spellbook.Find(x => x is Spell.Spell_HealBurst) as Spell.Spell_HealBurst;
 
            Spell.charges += 1f * (float)this.rank * Time.deltaTime;
        }
        
    }

    public class JavelinMastery : StatTalent
    {
        public JavelinMastery(Player owner)
        {
            this.owner = owner;
            this.max_rank = 5;
            this.tier = 1;

        }
        public override void Apply()
        {
            var Spell = owner.Spellbook.Find(x => x is Spell.JavelinOfLight) as Spell.JavelinOfLight;
            Spell.mana_cost -= 5;
        }
    }

    public class HealingPowers : StatTalent
    {
        public HealingPowers(Player owner)
        {
            this.owner = owner;
            this.max_rank = 5;
            this.tier = 1;

        }
        public override void Apply()
        {
          /* var allies = owner.GetAlliesInRange(30f);
           allies.ForEach(x => x.Attributes[Attribute.HealingAmplifier] += 30);

           owner.Attributes[Attribute.HealingAmplifier] += 30;*/
            ;
            
        }
    }

    public class HealingAura : PassiveTalent
    {
        public HealingAura(Player owner)
        {
            this.tier = 1;
            this.max_rank = 5;
            this.owner = owner;
        }

        public override void Update()
        {
            owner.GetAlliesInRange(30f).ForEach(x=> x.HP += 0.5f * this.rank);
        }
    }

    public class AngelPunishment : PassiveTalent
    {
        public AngelPunishment(Player owner)
        {
            this.tier = 2;
            this.max_rank = 3;
            this.owner = owner;
        }

        public override void Update()
        {
            var enemies = owner.GetEnemiesInRange(1000f);

            
            foreach (var e in enemies)
            {
                //if(e.BuffList.Exists(x=> x is Buff.Debuff_Immobilize));
               // e.Attributes[Attribute.PhysicalDamageAmplifier] +=  10f * (float)this.rank;
            }

            
        }
    }


    //Wizard
    public class PiercingIce : StatTalent
    {
        public PiercingIce()
        {
            this.max_rank = 3;
            this.tier = 2;
        }

    }
}