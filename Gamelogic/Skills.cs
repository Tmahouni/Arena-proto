
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
// TODO: Might be able to make all toon-class classes into enums instead of having a bunch of const fields and
// lists, with maybe an implicit cast operator for each one


public class Skills : MonoBehaviour
{
	/*public GameObject model_ninja;
	
	
    public static void GetModel(Player Customer,string s)
	{
		//string model_name = s + "_Model";
      //GameObject model = Resources.Load(model_name) as GameObject;
		var g = GameObject.Instantiate(model_ninja) as GameObject;
		
		g.transform.parent = Customer.gameObject.transform;
		g.transform.localPosition = new Vector3(0,-26.60f,1.76f);
		g.transform.localScale = new Vector3(14.63f,14.64f,14.64f);
	}*/
	
    public static List<Spell> GetAllActiveSkillsByClass(Player Customer, string @class)
    {
		
        switch (@class)
        {
            case "Ninja":
                return Ninja.GetSpellList(Customer);
            case "Angel":
                return Angel.GetSpellList(Customer);
            default:
                return null;
        }
    }


    public class Ninja
    {
        public static  List<Spell> AllActiveSkillsList = new List<Spell>();

        public static List<Spell> GetSpellList(Player Customer)
        {
            AllActiveSkillsList.Add(new Spell.Stab(Customer));
            AllActiveSkillsList.Add(new Spell.Spell_FanOfKnives(Customer));
            AllActiveSkillsList.Add(new Spell.Shuriken(Customer));
            AllActiveSkillsList.Add(new Spell.Spell_SmokeBomb(Customer));
            AllActiveSkillsList.Add(new Spell.ShadowWalk());
            AllActiveSkillsList.Add(new Spell.Spell_Chloroform());
            AllActiveSkillsList.Add(new Spell.Spell_ShadowStep(Customer));

            return AllActiveSkillsList;
        }
        public static List<Talent> TalentTree = new List<Talent>();
        public static List<Talent> GetTalentList(Player Customer)
        {
            TalentTree.Add(new Talent.MasterOfShadows(Customer));
            TalentTree.Add(new Talent.ShadowWalker(Customer));
            TalentTree.Add(new Talent.SmokeShield(Customer));
            TalentTree.Add(new Talent.DarkEthereal(Customer));
            TalentTree.Add(new Talent.Neurotoxic(Customer));

            //Throwing Master tree
            TalentTree.Add(new Talent.SnakeBite(Customer));
            TalentTree.Add(new Talent.DoubleTime(Customer));
            TalentTree.Add(new Talent.TongueSlicer(Customer));
            TalentTree.Add(new Talent.EndlessKnives(Customer));
            TalentTree.Add(new Talent.MultiShuriken(Customer));

            //Combat tree
            TalentTree.Add(new Talent.CriticalStrike(Customer));
            TalentTree.Add(new Talent.ZenHatred(Customer));
            TalentTree.Add(new Talent.ZenBlood(Customer));
            TalentTree.Add(new Talent.DoubleScimitar(Customer));
            TalentTree.Add(new Talent.ZenEnergy(Customer));

            return TalentTree;
        }


        
   }

    public class Angel
    {
        public static List<Spell> AllActiveSkillsList = new List<Spell>();

        public static List<Spell> GetSpellList(Player Customer)
        {
            AllActiveSkillsList.Add(new Spell.Spell_AngelSword());
            AllActiveSkillsList.Add(new Spell.JavelinOfLight());
            AllActiveSkillsList.Add(new Spell.Spell_LightBeam(Customer));
            AllActiveSkillsList.Add(new Spell.BlindingLight());
            AllActiveSkillsList.Add(new Spell.Spell_AngelCharge());
            AllActiveSkillsList.Add(new Spell.Spell_HealBurst());
            AllActiveSkillsList.Add(new Spell.TestOfFate());

            return AllActiveSkillsList;
        }
    }




}


  