using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

public abstract class Spell : MonoBehaviour
{

    public int mana_cost;
    public abstract void Cast(Player p);
    public Texture2D icon;
    public float cooldown_time; //time left in cooldown
	private float max_cooldown_time; //max cd time

    private string tooltip = "Tooltip not defined yet";
    public int damage;
    public Buff buff; //Buff which spell applies
    public Talent reference_talent;

    public float charges = -1; //Combo this with HasCharges() always
    

        public bool HasCharges()
        {
            if (this.charges > -1)
                return true;

            return false;
        }       


    public bool HasMana(Player player)
    {
        if (player.energy >= this.mana_cost)
            return true;
        else
        {
            return false;
        }
    }


    public bool IsOnCooldown()
    {
        if (this.cooldown_time == 0)
            return false;

        else return true;

    }


    public void ReduceCooldown()
    {
        if (this.cooldown_time <= 0)
            this.cooldown_time = 0;

        else
            this.cooldown_time -= 60 * Time.deltaTime;

    }

    public string GetTooltip()
    {
        return this.tooltip;
    }
    private void Teleport(Player p, float distance)
    {

        RaycastHit hit;

        if (Physics.Raycast(p.transform.position, p.transform.forward, out hit, distance))
        {
            distance = hit.distance - 4;

          p.transform.Translate(p.transform.up*4, Space.World);
        }

        p.transform.Translate(p.transform.forward*distance, Space.World);

    }

   /* private void ApplyBuff(Player target,Buff b)
    {
        if (target.BuffList.Contains(b) && b.Stackable)
        {
            var old_b = target.BuffList.Find(x=> x.GetType() == typeof(b) );
            old_b.DeApply();

            if(old_b.current_stack < old_b.max_stack)
                old_b.current_stack++;

            old_b.Apply(target);
        }
    }*/


    //Random stuff
    //Wizard
    public class Fireball : Spell
    {
        public Fireball()
        {
            this.icon = (Texture2D) Resources.Load("testi");
            this.mana_cost = 15;
        }


        public override void Cast(Player player)
        {
            if (HasMana(player))
            {
                GameObject fireball = (GameObject) Resources.Load("Fireball");

                GameObject g =
                    (GameObject)
                    Network.Instantiate(fireball, player.gameObject.transform.position,
                                        Camera.mainCamera.transform.rotation, 0);
                g.GetComponent<Fireball>();
                g.networkView.RPC("SendID", RPCMode.All, player.ID);
                player.energy -= this.mana_cost;
            }

        }

    }

    public class FrostNova : Spell
    {
        public FrostNova()
        {
            this.icon = Resources.Load("FrostNova") as Texture2D;
        }

        public override void Cast(Player p)
        {
            if (!HasMana(p) && IsOnCooldown()) return;

            p.energy -= 40;

            var enemies = p.GetEnemiesInRange(15f);

                foreach(var e in enemies)
                {
                    Buff b = new FrostNova_Debuff();
                    b.Add(e);
                }
        }

        public class FrostNova_Debuff : Buff
        {
            public FrostNova_Debuff()
            {
                this.duration = 5f * 60;
                this.state = CombatState.Immobilized;
            }

            public override void Apply(Player player)
            {
                EffectManager eff = new EffectManager();
                eff.PersistantEffect("Effect_FrostNova", player.gameObject, 5f, Vector3.down * 2.5f);
            }

            public override void DeApply(Player player)
            {
                ;
            }
        }

    }
    
    public class Blink : Spell
    {
        public Blink()
        {

            this.mana_cost = 25;
            this.icon = Resources.Load("tex_Blink") as Texture2D;
        }


        public override void Cast(Player player)
        {
            if (HasMana(player) || !IsOnCooldown())
            {
                player.energy -= this.mana_cost;
                this.cooldown_time = 8 * 60f;
                EffectManager eff = new EffectManager();
                eff.PlayEffect("Effect_Blink", player.transform.position);
                player.gameObject.transform.Translate(player.transform.forward * 30, Space.World);
                eff.PlayEffect("Effect_Blink", player.transform.position);
            }

        }

    }

    public class JavelinOfLight : Spell
    {
        public JavelinOfLight()
        {
            this.icon = (Texture2D) Resources.Load("tex_Javelin");
            this.mana_cost = 30;
        }


        public override void Cast(Player player)
        {
            if (HasMana(player) && !IsOnCooldown())
            {
				player.Animator.PlayAnimation("throw",1.5f);
                GameObject javelin = (GameObject) Resources.Load("Javelin");

                GameObject g =
                   (GameObject)
                    Network.Instantiate(javelin, player.gameObject.transform.position,
                                        Camera.mainCamera.transform.rotation, 0);
                g.GetComponent<Javelin>();
                g.networkView.RPC("SendID", RPCMode.All, player.ID);
                player.energy -= this.mana_cost;
                this.cooldown_time = 60*2.5f;
            }

        }

    }


    /////////////////////////////////////
    //NINJA//////////////////////////////
    /////////////////////////////////////

    //Talents:
    //MultiStrike:100%
    //Silence:100%,Handled on projectile (Gameobject:Shuriken)
    //Stackable:0%
    public class Shuriken : Spell
    {
        GameObject shuriken = (GameObject)Resources.Load("Shuriken");

        public Shuriken(Player p)
        {
            this.icon = (Texture2D)Resources.Load("tex_Shuriken");
            this.mana_cost = 20;
            this.reference_talent = p.TalentTree.Find(x => x.GetType() == typeof(Talent.MultiShuriken) );
            this.damage = 20;
			
			this.tooltip = "Throw shuriken,which damages and slows victim";
        }


        public override void Cast(Player player)
        {

            if (HasMana(player) && !IsOnCooldown())
            {
                player.Animator.PlayAnimation("throw",2.0f);
                player.energy -= this.mana_cost;
                var rune = reference_talent as Talent.StatTalent;

                //MultiShuriken
                if (rune.IsSelected())
                {
                   MultiStrike(player);
                    return;
                }

                //Normal
                GameObject g4 =
                    (GameObject)
                    Network.Instantiate(shuriken, player.gameObject.transform.position,
                                        Camera.mainCamera.transform.rotation, 0);
                g4.GetComponent<Shuriken>();
                g4.networkView.RPC("SendID", RPCMode.All, player.ID);

            }
        }

        

        //MultiStrike
        private void MultiStrike(Player player)
        {
                Vector3 original_pos = player.gameObject.transform.position;

                GameObject g1 =
                    (GameObject)
                    Network.Instantiate(shuriken, player.gameObject.transform.position,
                                        Camera.mainCamera.transform.rotation, 0);
                g1.GetComponent<Shuriken>();
                g1.networkView.RPC("SendID", RPCMode.All, player.ID);

                original_pos.z -= 3.5f;

                GameObject g2 =
                    (GameObject)
                    Network.Instantiate(shuriken, original_pos,
                                        Camera.mainCamera.transform.rotation, 0);
                g2.GetComponent<Shuriken>();
                g2.networkView.RPC("SendID", RPCMode.All, player.ID);

                original_pos.z += 6.5f;

                GameObject g3 =
                    (GameObject)
                    Network.Instantiate(shuriken, original_pos,
                                        Camera.mainCamera.transform.rotation, 0);
                g3.GetComponent<Shuriken>();
                g3.networkView.RPC("SendID", RPCMode.All, player.ID);
                return;
            }

        }
       

    public class ShadowWalk : Spell
    {
        private GameObject effect = (GameObject) Resources.Load("ShadowWalk_effect");

        public ShadowWalk()
        {
            this.icon = (Texture2D)Resources.Load("tex_ShadowWalk");
            this.mana_cost = 15;
            
			this.tooltip = "Conceals you in shadows.Drains energy while active.Can be toggled on and off at will.";
        }


        public override void Cast(Player player)
        {
            this.reference_talent = player.TalentTree.Find(x => x is Talent.MasterOfShadows);

            //When cancelling stealth
            if (player.HasState(CombatState.Stealth))
            {

                Buff b = player.BuffList.Find(x => x.state == CombatState.Stealth);
                b.DeApply(player);
                player.BuffList.Remove(b);
                return;
            }


            if (!HasMana(player) || IsOnCooldown()) return;

            //Apply talent
            var final_buff = new Buff_ShadowWalk();
            final_buff.energy_regen_decrease *= reference_talent.GetPct();
            //final_buff.Add(player);
            
            

            Network.Instantiate(effect, player.transform.position, Quaternion.identity, 0);
           player.BuffList.Add(final_buff);
            player.energy -= this.mana_cost;
            this.cooldown_time = 60*6;
        }

        //Buff implementation
        public class Buff_ShadowWalk : Buff
        {
            public float energy_regen_decrease = 1; //Talent needs to access these
            public int movement_speed = 6;

            private float orig_energy;
            
            private Color original_color;

            public Buff_ShadowWalk()
            {
                this.duration = 0xFFF;
                this.state = CombatState.Stealth;

                this.DeleteFlags.Add(CombatFlag.OnAttack); //We want to remove stealth when player damages someone
                this.DeleteFlags.Add(CombatFlag.OnDamaged); //And when he takes damage
            }


            public override void Apply(Player player)
            {
                this.orig_energy = player.energy_regen;

                var t = player.transform.GetComponentInChildren<SkinnedMeshRenderer>();

                //Transparency effect
                original_color = t.renderer.material.color;
                t.renderer.material.color = new Color(0f, 0f, 0f, 0.10f);
                player.networkView.RPC("Fade", RPCMode.All, original_color.r, original_color.g, original_color.b, 0.10f);

                player.energy_regen -= 2;
                player.energy_regen *= energy_regen_decrease;
                player.movement_speed += movement_speed;

            }

            public override void DeApply(Player player)
            {
                //Transparency effect
               var  t = player.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                t.renderer.material.color = original_color;
                player.networkView.RPC("Fade", RPCMode.All, original_color.r, original_color.g, original_color.b, 255f);

                player.energy_regen = orig_energy;
                player.movement_speed -= movement_speed;
            }

        }
    }


    public class Stab : Spell
    {
        public Stab(Player p)
        {
            this.icon = (Texture2D) Resources.Load("tex_Stab");
            this.mana_cost = 10;
            this.reference_talent = p.TalentTree.Find(x => x.GetType() == typeof(Talent.CriticalStrike));
            this.damage = 40;
			this.tooltip = "Basic melee attack. If done while stealthed stuns your target";
        }


        public override void Cast(Player player)
        {
            //Nullify damage to base
            this.damage = 40;

            if (HasMana(player) && !IsOnCooldown())
            {
                var effect = new EffectManager();
                player.Animator.PlayAnimation("attack",1f);

                
                Player target = player.GetTargetForward(10f);

                if (target == null)
                {
                    this.cooldown_time = 60 * 0.3f;
                    return;
                }

                this.cooldown_time = 60 * 1.0f;
                player.energy -= this.mana_cost;
                //RuneA:Crit
                var runeA = this.reference_talent as Talent.CriticalStrike;

                //Critted
                if (runeA.Check())
                {
                    effect.PlayEffect("Effect_Crit", target.transform.position);
                    damage *= 2;
                }
                    
                    //RuneB:DoubleScimitar TODO:Bleed Proc debuff
                    var runeB = player.TalentTree.Find(x => x is Talent.DoubleScimitar) as Talent.StatTalent;
               
                
                if (runeB.IsSelected())
                {
                    this.damage *= 2;
                }

                target.ApplyDmg(damage,player.ID);

  
                //Effect
                effect.PlayEffect("Effect_Blood",target.transform.position);
               

                    //Stealth procs here
                    if (player.HasState(CombatState.Stealth))
                    {
                        //Talent ShadowWalker
                        var talent_shadowWalker = player.TalentTree.Find(x => x is Talent.ShadowWalker);
                        float plus = 0.5f * talent_shadowWalker.GetRank();
                        target.networkView.RPC("Stun", RPCMode.All, 2f + plus);

                        if (runeB.IsSelected())
                        {
                            target.networkView.RPC("Bleed", RPCMode.All,player.ID);
                        }

                    }

                }

            }


        public class Debuff_Bleed : Buff.DotBuff
        {
           public Debuff_Bleed(Player owner)
            {
                this.duration = 60 * 8;
                this.owner = owner;
                this.Tick_interval = 15000000f;
                this.TickTimer = new Stopwatch();
            }

            public override void Apply(Player player)
            {
                TickTimer.Start();
                EffectManager effect = new EffectManager();
                effect.PersistantEffect("Effect_Bleed", player.gameObject, 8f, Vector3.down * 0.5f);
            }

            public override void OnTick(Player p)
            {
                if (TickTimer.Elapsed.Ticks >= Tick_interval)
                {
                    TickTimer.Reset();
                    UnityEngine.Debug.Log("Ticking..");
                    p.ApplyDmg(10, owner.ID);
                    TickTimer.Start();
                }
            }

            public override void DeApply(Player player)
            {
                ;
            }
        }


        }

    /// <summary>
    /// Talents:
    /// DoubleTime:100%
    /// ReduceCD:100%
    /// </summary>
    public class Spell_FanOfKnives : Spell
    {

        public Spell_FanOfKnives(Player p)
        {
            this.icon = (Texture2D) Resources.Load("tex_FanOfKnives");
            this.mana_cost = 50;
            this.reference_talent = p.TalentTree.Find(x => x is Talent.EndlessKnives);
			
        }


        public override void Cast(Player player)
        {

            if (HasMana(player) && !IsOnCooldown())
            {
                Teleport(player, 45);

                var eff = new EffectManager();
                eff.PlayEffect("Effect_FanOfKnives",player.transform.position);

                List<Player> Enemies = player.GetEnemiesInRange(35);


                foreach (Player target in Enemies)
                {
                    float distance = (player.transform.position - target.transform.position).magnitude;

                    target.ApplyDmg((int) (3*(50 - distance)), player.ID);

                }

                //EndlessKnives
                var rune_A = reference_talent as Talent.StatTalent;
                if (rune_A.IsSelected() && Rune_A())
                {
                    return;
                }

                player.energy -= this.mana_cost;
                this.cooldown_time = 60* 8 * player.TalentTree.Find(x=> x is Talent.DoubleTime).GetPct();
            }

        }

        private bool RuneA_cooldown = false;
        private bool Rune_A()
        {
            if (!RuneA_cooldown)
            {
                RuneA_cooldown = !RuneA_cooldown;
                return true;
            }

            else
            {
                RuneA_cooldown = !RuneA_cooldown;
                return false;
            }

        }


    }

    //TODO: Limit bombs to 3 per player
    public class Spell_SmokeBomb : Spell
    {
        List<GameObject> Bombs = new List<GameObject>();
		
        public Spell_SmokeBomb(Player p)
        {
            this.icon = (Texture2D)Resources.Load("tex_SmokeBomb");
            this.mana_cost = 10;
            this.reference_talent = p.TalentTree.Find(x=> x.GetType() == typeof(Talent.Neurotoxic) );
        }


        public override void Cast(Player player)
        {
            GameObject smokebomb = (GameObject)Resources.Load("SmokeBomb");

            if (HasMana(player) && !IsOnCooldown())
            {
                player.energy -= this.mana_cost;
                this.cooldown_time = 7 * 60;

                var rune = this.reference_talent as Talent.StatTalent;

                if (rune.IsSelected())
                {
                    Rune_A(player);
                    return;
                }
                
                var g = (GameObject)Network.Instantiate(smokebomb, player.transform.position, Camera.mainCamera.transform.rotation, 0);
                g.GetComponent<projectile_SmokeBomb>().owner = player;

            }

        }

        //Neurotoxic
        private void Rune_A(Player player)
        {
            GameObject poisonbomb = (GameObject)Resources.Load("PoisonBomb");
            GameObject g2 = (GameObject)Network.Instantiate(poisonbomb, player.transform.position, Camera.mainCamera.transform.rotation, 0);

            g2.GetComponent<PoisonBomb>().owner = player;
        }

        //Used in normal smokebomb
        public class SmokeShield : Buff
        {
            private int power;
            public SmokeShield(int power)
            {
                this.duration = 60 * 1.0f;
                this.power = power;
            }

            public override void Apply(Player player)
            {
                player.DR += power;
            }

            public override void DeApply(Player player)
            {
                player.DR -= power;
            }
        }

    }

    //Done and tested
    public class Spell_Chloroform : Spell
    {
       

        public Spell_Chloroform()
        {
            this.mana_cost = 30;
            this.icon = (Texture2D)Resources.Load("tex_Chloroform");
        }

        public override void Cast(Player p)
        {

            if (!p.HasState(CombatState.Stealth))
                return;

            if (HasMana(p) && !IsOnCooldown())
            {

                var target = p.GetTargetForward(10f);
                if (target == null) return;
 
                target.networkView.RPC("Chloroform",RPCMode.All,null);

                p.energy -= this.mana_cost;
                this.cooldown_time = 60*20;
            }
        }

        //Fix sap state handling at some point
       public class Debuff_Chloroform : Buff
       {
           GameObject effect;

           public Debuff_Chloroform()
           {
               this.state = CombatState.Sapped; //Doing sap effect on Player.cs,big hack fix!
               this.duration = 7 * 60;
               this.DeleteFlags.Add(CombatFlag.OnDamaged);
           }

           
           public override void Apply(Player player)
           {
               var EffectManager = new EffectManager();
               effect = EffectManager.PersistantEffect("Effect_Chloroform", player.gameObject, 7f, Vector3.zero);
               this.applied = true;
           }

           public override void DeApply(Player player)
           {
               Network.Destroy(effect);
           }


       }
    }


    public class Spell_ShadowStep : Spell
    {
        private Vector3 return_point; // Used in talent

        public Spell_ShadowStep(Player p)
        {
            this.tooltip = "Teleport behind target";
            this.icon = Resources.Load("tex_ShadowStep") as Texture2D;
            this.mana_cost = 10;
            this.reference_talent = p.TalentTree.Find(x => x is Talent.DarkEthereal);
			this.tooltip = "Teleport behind target";
        }

        public override void Cast(Player p)
        {
            var talent = reference_talent as Talent.StatTalent;
            EffectManager eff = new EffectManager();
            
            //handle talent
            if (this.IsOnCooldown() && talent.IsSelected() && this.return_point != Vector3.zero)
            {
                p.transform.position = return_point;
                return_point = Vector3.zero;
                eff.PlayEffect("Effect_ShadowStep2", p.gameObject.transform.position + Vector3.down);
            }

            if(HasMana(p) && !IsOnCooldown())
            {
                var target = p.GetTargetForward(105f);
                if (target == null) return;

                Buff b = new Debuff_Shadowstep();
                b.Add(target);

            eff.PlayEffect("Effect_ShadowStep", p.gameObject.transform.position + Vector3.down * 2f, Camera.mainCamera.transform.rotation);

                //Save spawnpoint if talent is chosen
            if (talent.IsSelected())
                return_point = p.transform.position;

            
            p.GetComponent<WowCamera>().Rotate(target.transform.rotation.eulerAngles);
            p.transform.position = target.transform.position - target.transform.forward * 1f;
            p.GetComponent<WowCamera>().Rotate(target.transform.rotation.eulerAngles);

            eff.PlayEffect("Effect_ShadowStep2", p.gameObject.transform.position + Vector3.down);

            p.energy -= this.mana_cost;
            this.cooldown_time = 60 * 15;
            }
        }

        public class Debuff_Shadowstep : Buff
        {
            public Debuff_Shadowstep()
            {
                this.duration = 1f * 60;
				
            }

            public override void Apply(Player player)
            {
                player.movement_speed -= 10f;
            }
            public override void DeApply(Player player)
            {
                player.movement_speed += 10f;
            }
        }

        
    }


    /////////////////////////////////////
    //Angel//////////////////////////////       Damage/Support
    /////////////////////////////////////
    
    //Implemented,not tested
    public class TestOfFate : Spell
    {

        public TestOfFate()
        {
            this.icon = Resources.Load("tex_SaviorsReach") as Texture2D;
            this.mana_cost = 30;
			this.tooltip = "Pull friendly unit to your position";
        }

        public override void Cast(Player p)
        {
            if (!HasMana(p) || IsOnCooldown())
                return;

            var target = p.GetTargetForward(80f);
            if (p.IsEnemy(target))
             return;
            
            target.transform.position = p.transform.position;

            this.cooldown_time = 60*25;
            p.energy -= this.mana_cost;
        }
    }

    public class BlindingLight : Spell
    {
        public BlindingLight()
        {
            this.icon = Resources.Load("tex_Flash") as Texture2D;
            this.mana_cost = 40;
			this.tooltip = "Blind all enemies near you";
        }

        public override void Cast(Player p)
        {
            if (!HasMana(p) || IsOnCooldown()) return;

            var enemy = p.GetEnemiesInRange(25f);

            foreach (var player in enemy)
            {
                Buff b = new Debuff_BlindingLight();
                b.Add(player);
            }

            var effect = new EffectManager();
            effect.PlayEffect("Effect_BlindingLight",p.transform.position);



            p.energy -= this.mana_cost;
            this.cooldown_time = 60*15;
        }


        public class Debuff_BlindingLight : Buff
        {
            public Debuff_BlindingLight()
            {
                this.duration = 60 * 3;
                this.state = CombatState.Blinded;
            }

            public override void Apply(Player player)
            {
                this.applied = true;
            }

            public override void DeApply(Player player)
            {
                ;
            }
        }
    }

    //Done
    public class Spell_LightBeam : ChannelingSpell
    {
        public Spell_LightBeam(Player owner)
        {
            this.icon = Resources.Load("tex_GodBeam") as Texture2D;
            this.owner = owner;
            this.mana_cost = 35;
            this.cast_time = 2f * 60;
			this.tooltip = "Channel beam of light,when released launches projectile at speed of light, which does damage based on channeling time";
        }


        public override void Cast(Player p)
        {
            if(HasMana(p) && !IsOnCooldown())
                this.StartCast();
            
        }

        public override void FinishCast()
        {
           /* UnityEngine.Debug.Log("Finishing cast");
             GameObject g = (GameObject)Network.Instantiate(Resources.Load("LightBeam") as GameObject, owner.transform.position,
                                                   Camera.mainCamera.transform.rotation, 0);

                g.GetComponent<LightBeam>();
                g.networkView.RPC("SendID", RPCMode.All, owner.ID);
                owner.energy -= this.mana_cost;*/

 	        End();
        }

        public override void End()
        {
            owner.Animator.PlayAnimation("cast_end", 1.0f);
            UnityEngine.Debug.Log("Finishing cast");
             GameObject g = (GameObject)Network.Instantiate(Resources.Load("LightBeam") as GameObject, owner.transform.position,
                                                   Camera.mainCamera.transform.rotation, 0);

               var lightbeam = g.GetComponent<LightBeam>();
               lightbeam.networkView.RPC("SendID", RPCMode.All, owner.ID);
			   lightbeam.damage = 80 * (1 - (this.GetTime() / 2f));
                owner.energy -= this.mana_cost;
                base.End();
        }
    }

    public class Spell_AngelSword : Spell
    {
        public int heal_amount;
        public Spell_AngelSword()
        {
            this.icon = Resources.Load("tex_AngelSword") as Texture2D;
            this.mana_cost = 15;
            this.heal_amount = 30;
        }

        public override void Cast(Player p)
        {
            var enemy = p.GetTargetForward(9f);
            if (!HasMana(p) || IsOnCooldown())
                return;
			
			p.Animator.PlayAnimation("attack",1.2f);
            if (enemy == null)
            {
                this.cooldown_time = 60 * 0.2f; 
                return;
            }
            p.energy -= this.mana_cost;
            var effect = new EffectManager();
			
            Spell heal_charge = p.Spellbook.Find(x => x is Spell_HealBurst);

                heal_charge.charges += 50;
                var sword_effect = effect.PlayEffect("Effect_AngelSword", enemy.transform.position, Camera.mainCamera.transform.rotation);
                p.HP += heal_amount;
                p.networkView.RPC("SendHP", RPCMode.All, p.HP);
                enemy.ApplyDmg(50, p.ID);
                this.cooldown_time = 60*2;
        }
    }

    public class Spell_HealBurst : Spell
    {
        

        public Spell_HealBurst()
        {
            this.charges = 50;
            this.icon = Resources.Load("tex_HealingCircle") as Texture2D;
			this.tooltip = "Heal all units around you";
            this.mana_cost = 25;
        }

        public override void Cast(Player player)
        {
            if (!HasMana(player) || IsOnCooldown())
                return;
            var friends = player.GetAlliesInRange(20f);
            friends.Add(player); // We want to heal self too

            foreach (var p in friends)
            {
                p.Heal(this.charges);
            }
			
			    var effect = new EffectManager();
                effect.PersistantEffect("Effect_HealingCircle",player.gameObject,2f,Vector3.zero);
			
            this.charges = 0;
           // Debug.Log(this.damage);
            player.energy -= this.mana_cost;
            this.cooldown_time = 60*15;
        }
    }

    //TODO
    public class Spell_AngelCharge : Spell 
    {
        public Spell_AngelCharge()
        {
            this.mana_cost = 30;
            this.icon = (Texture2D)Resources.Load("tex_AngelCharge");
			this.tooltip = "Charge forward for 0.5 seconds,if colliding with enemy jump backwards. Applies immobilize to enemy";
        }


        public override void Cast(Player p)
        {
            if (!HasMana(p) || IsOnCooldown()) return;

            var eff = new EffectManager();
            eff.PersistantEffect("Effect_AngelCharge", p.gameObject, 1.5f, Vector3.zero);
            this.cooldown_time = 14*60;
            p.gameObject.AddComponent<AngelCharge>().SetValues(p, Camera.mainCamera.transform.forward, 80f, 0.5f);
            p.energy -= this.mana_cost;
        }


    }

    //FrostMage PROTO
    public class Spell_FrostBolt : ChannelingSpell
    {
        public Spell_FrostBolt(Player owner)
        {
            this.owner = owner;
            this.cast_time = 2.0f * 60;
            this.mana_cost = 35;
            this.icon = Resources.Load("tex_FrostBolt") as Texture2D;
        }

        public override void Cast(Player p)
        {
            StartCast();
        }

        public override void FinishCast()
        {
            GameObject g = (GameObject)Network.Instantiate(Resources.Load("FrostBolt") as GameObject, owner.transform.position,
                                                   Camera.mainCamera.transform.rotation, 0);
            owner.Animator.PlayAnimation("cast_end", 1.0f);
            base.End();
        }
     }

    public class Spell_Blizzard : Spell
    {
        public Spell_Blizzard()
        {
            this.icon = Resources.Load("tex_FrostBolt") as Texture2D;
        }

        public override void Cast(Player p)
        {
            

            var bliz = (GameObject)Network.Instantiate(Resources.Load("Blizzard") as GameObject, p.transform.position, Camera.mainCamera.transform.rotation, 0);
           bliz.GetComponent<Blizzard>().owner_id = p.ID;
        }

        public class Debuff_Blizzard_Slow : Buff
        {
            public Debuff_Blizzard_Slow()
            {
                this.duration = 10f*60;
                this.state = CombatState.Slowed;
            }

            public override void Apply(Player player)
            {
                player.movement_speed -= 0.1f;
            }
            public override void DeApply(Player player)
            {
                player.movement_speed += 0.1f;
            }
        }
    }

    /// <summary>
    ///Channeling Spell
    /// </summary>

   public abstract class ChannelingSpell : Spell
    {
       public Player owner;
       public abstract void FinishCast();
        private bool casting = false;
        public float cast_time;
        public float current_cast_time;
        private GameObject effect;

        public virtual void End()
        {
            casting = false;
            this.current_cast_time = cast_time;
           
			Network.Destroy(effect);
				
        }
		
        public void StartCast()
        {
			//Spawn casting effect
			var eff = new EffectManager();
			effect = eff.PersistantEffect("Effect_cast",owner.gameObject,this.cast_time/60,Vector3.zero);
			
            this.current_cast_time = cast_time;
            this.casting = true;
        }

       public bool IsCasting()
       {
           if (casting)
           {
               return true;
           }
           return false;
       }

       public bool IsReady()
       {
           this.current_cast_time -= 60 * Time.deltaTime;
           if (this.current_cast_time < 0)
               return true;

           return false;
       }

       public float GetTime()
       {
           return this.current_cast_time / 60;
       }
    }
}

		
	
	
	
	
	

