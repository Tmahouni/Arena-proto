using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


public class Player : MonoBehaviour {

    public List<float> Attributes;

	public AnimationHandler Animator;
    public CombatLogger CombatLogger;
    //Character
	public int ID;
    public string p_Name;
    public float movement_speed;
    public string team;
    public string @class;
	
	
	//HP JA ENERGY
	public float HP_max = 250;
	public float HP = 150;

	public float energy = 120;
	public float energy_max = 120;
	public float energy_regen = 1.2f;
	
    //STATS
    //TODO
    public float DR = 0;
    public float bonus_damage;

	public List<Buff> BuffList = new List<Buff>();
	public List<Spell> Spellbook = new List<Spell>();
	public List<CombatFlag> CombatFlags = new List<CombatFlag>();

    //Talents
    public List<Talent> TalentTree  = new List<Talent>();
    public int talent_point;

	//Exp system
	public int Level = 1;
	public float XP_current = 0;
	public float XP_needed = 100;
	
	
	// K/D
	public int deaths = 0;
	public int kills = 0;
	
	
	Stopwatch regen_timer = new Stopwatch();

	
	void Awake()
	{
		GameLogic.PlayerList.Add(this);
		Animator = new AnimationHandler(this);
		this.ID = GameLogic.GetLastID();
        //this.Spellbook = Skills.GetAllActiveSkillsByClass(this, this.@class);
        CombatLogger = new CombatLogger(this);

	}

    bool model = false;
    GameObject Mode = null;
	// Use this for initialization
	void Start () 
	{	
		Screen.showCursor = false;
		
		//Talent ini
        //ShadowTree
        TalentTree.Add(new Talent.MasterOfShadows(this));
        TalentTree.Add(new Talent.ShadowWalker(this));
		TalentTree.Add(new Talent.SmokeShield(this));
		TalentTree.Add(new Talent.DarkEthereal(this));
		TalentTree.Add(new Talent.Neurotoxic(this));

        //Throwing Master tree
        TalentTree.Add(new Talent.SnakeBite(this));
        TalentTree.Add(new Talent.DoubleTime(this));
        TalentTree.Add(new Talent.TongueSlicer(this));
        TalentTree.Add(new Talent.EndlessKnives(this));
        TalentTree.Add(new Talent.MultiShuriken(this));

        //Combat tree
        TalentTree.Add(new Talent.CriticalStrike(this));
        TalentTree.Add(new Talent.ZenHatred(this));
        TalentTree.Add(new Talent.ZenBlood(this));
        TalentTree.Add(new Talent.DoubleScimitar(this));
        TalentTree.Add(new Talent.ZenEnergy(this));

        if(networkView.isMine)
        	this.Spellbook = Skills.GetAllActiveSkillsByClass(this, this.@class);

     

    

		this.talent_point = 11;
	    this.HP = 500;
	    this.HP_max = 500;
		regen_timer.Start();

        networkView.RPC("SendID", RPCMode.All, this.ID);
        networkView.RPC("SendName", RPCMode.All, this.p_Name);
        networkView.RPC("SendClass", RPCMode.All, this.@class);
        networkView.RPC("SendTeam", RPCMode.All, this.team);

        

      
        this.movement_speed = 11;

        var Model = new Models();
        Model.GetModel(this, @class);

	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
        networkView.RPC("SendClass", RPCMode.All, this.@class);
        networkView.RPC("SendTeam", RPCMode.All, this.team);
        networkView.RPC("SendName", RPCMode.All, this.p_Name);


        /*if (!model)
        {
            var Model = new Models();
           Mode = Model.GetModel(this, @class);
            model = true;

        }*/
        //this.networkView.RPC("SendModel", RPCMode.All);

        //Set up outlining
       /* if (networkView.isMine)
        {
            foreach (Player player in GameLogic.PlayerList)
            {
				if(player == this)
					continue;
				
				var transform = player.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                if (IsEnemy(player))
                {
					
					transform.renderer.material.shader = Shader.Find("Outlined/Diffuse");
                    transform.renderer.material.SetColor("_OutlineColor", Color.red);
					player.networkView.RPC("SendMaterial",RPCMode.All);
                    //var width = (player.transform.position - this.transform.position).magnitude;
                    //transform.renderer.material.SetFloat("_Outline", width);
					continue;
                }
					transform.renderer.material.shader = Shader.Find("Outlined/Diffuse");
                    transform.renderer.material.SetColor("_OutlineColor", Color.green);
					player.networkView.RPC("SendMaterial",RPCMode.All);
				
            }
        }*/
		
		//this.networkView.RPC("SendMaterial",RPCMode.All,this.transform.GetComponentInChildren<SkinnedMeshRenderer>().transform.renderer.material.shader.ToString());
		
        foreach (var dotter in this.BuffList)
        {
            if (dotter is Buff.DotBuff)
            {
                var dot = dotter as Buff.DotBuff;
                dot.OnTick(this);
            }
        }

        

		Regen();
		ExperienceHandler();

        DeathHandler();

		if(networkView.isMine && !this.HasState(CombatState.Stunned) && !this.IsDead() && !this.HasState(CombatState.Sapped))
		{
            if (!this.transform.GetComponent<UI_Talent>().IsActivated())
			    ActionInput(); //Spell input
		}

        //Update talents
        foreach (var talent in TalentTree)
        {
            if (talent is Talent.PassiveTalent)
            {
                var t = talent as Talent.PassiveTalent;
                t.Update();
            }
        }

        HandleChannelingSpells();

        List<Buff> temp = new List<Buff>();

            foreach (Buff b in this.BuffList)
            {
                foreach (CombatFlag c in b.DeleteFlags)
                {
                    if (this.CombatFlags.Contains(c))
                    {
                        temp.Add(b);
                    }
                }

            }
            this.CombatFlags.Clear();
        
        //Process
            foreach (var b in temp)
            {
                b.DeApply(this);
                this.BuffList.Remove(b);
            }
	}
	
	
	
	////////////////////////////// 
	//FUNKTIOT////////////////////
	//////////////////////////////  

   


    public void Heal(float amount)
    {
        this.networkView.RPC("CombatTextMessage", RPCMode.All, (int)amount, 1);
        this.CombatLogger.Add(0, amount);
        this.HP += amount;
        this.networkView.RPC("SendHP", RPCMode.All,this.HP);
    }

	//Regenoi energyä,myöhemmin ehkä HP?
	private void Regen()
	{
        if (this.HP > HP_max)
            HP = HP_max;

		if(regen_timer.Elapsed.TotalSeconds >= 0.1)
		{
			energy += energy_regen;
			regen_timer.Reset();
			regen_timer.Start();
		}
		
		if(energy >= energy_max)
			energy = energy_max;
	}
	
	//Kun pelaaja ottaa damagea käytä tätä!
	public void ApplyDmg(float damage,int caster_id)
	{
		Player caster = GameLogic.PlayerList.Find(x=> x.ID == caster_id);
        caster.CombatLogger.Add(damage, 0);

        caster.networkView.RPC("SendID",RPCMode.All, caster_id);

        if (this.death_timer.IsRunning || !this.IsEnemy(caster))
            return;

        this.networkView.RPC("CombatTextMessage", RPCMode.All, (int)damage,0);

        this.HP -= damage;
		
		if(this.HP <= 0)
			{
				this.deaths++;
				caster.kills += 1;
				caster.networkView.RPC("SendKills",RPCMode.All,caster.kills);
				this.networkView.RPC("SendDeaths",RPCMode.All,this.deaths);
                death_timer.Start();
			}
		  networkView.RPC("SendHP",RPCMode.All,this.HP);

          this.networkView.RPC("CombatFlagOnDamage", RPCMode.All);
          caster.networkView.RPC("CombatFlagOnAttack", RPCMode.All);
	}

    Stopwatch death_timer = new Stopwatch();
    private void DeathHandler()
    {
        if (this.HP <= 0 && !death_timer.IsRunning)
        {
            //Clear buffs
            BuffList.ForEach(x => x.DeApply(this));
            BuffList.Clear();

            this.Animator.PlayAnimation("death",1f);
            death_timer.Start();
        }

        /*if (death_timer.Elapsed.Seconds > 5)
        {
            this.HP = this.HP_max;
            this.transform.position = GameLogic.SpawnPoints[Random.Range(0,3)];
            death_timer.Reset();
            death_timer.Stop();
            networkView.RPC("SendHP", RPCMode.All, this.HP);
        }*/
    }

    public void ReSpawn()
    {
        this.HP = this.HP_max;
        death_timer.Reset();
        death_timer.Stop();
        networkView.RPC("SendHP", RPCMode.All, this.HP);
		
		if(this.team == "Team1")
			this.transform.position = GameLogic.SpawnPoints[0];
		
		else this.transform.position = GameLogic.SpawnPoints[1];
    }

    public bool IsDead()
    {
        if (death_timer.IsRunning)
            return true;
        return false;
    }
    
	
	public bool HasState(CombatState State)
        {
            if (this.BuffList.Exists(b => b.state == State))
                {
                    return true;
                }
            
            else return false;
        }

    private bool HasFlag(CombatFlag Flag)
    {
        if(this.CombatFlags.Contains(Flag))
            return true;

        return false;
    }
	
	
	//Taikojen käyttö
	private void ActionInput()
	{

		if(Input.GetButtonDown("Spell1"))
			Spellbook[0].Cast(this);


		if(Input.GetButtonDown("Spell2"))
			Spellbook[1].Cast(this);
					
		
		if(Input.GetButtonDown("Spell3"))
			Spellbook[2].Cast(this);

		
		if(Input.GetButtonDown("Spell4"))
			Spellbook[3].Cast(this);	
		
		
		if(Input.GetButtonDown("Spell5"))
			Spellbook[4].Cast(this);
		
		if(Input.GetButtonDown("Spell6"))
			Spellbook[5].Cast(this);

        if (Input.GetButtonDown("Spell7"))
            Spellbook[6].Cast(this);
		
		if(Input.GetKeyDown(KeyCode.F3))
			UnityEngine.Debug.Log(this.transform.position);

	}
	
	//Not used on anything
	public bool BehindTarget(Player target)
	{
		Vector3 directionToTarget = this.transform.position - target.transform.position;
		float dot = Vector3.Dot(directionToTarget,target.transform.forward);
		UnityEngine.Debug.Log(dot);
		if (dot < -0.7f)
		{
    		UnityEngine.Debug.Log("im behind target");
			return true;
		}
		else
		{
			UnityEngine.Debug.Log("fuck");
			return false;
		}
		
	}
	
	
	public Player GetTargetForward(float distance)
		{
			//OLD SHIT
			//var ray = Camera.mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
		/*var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));*/
		//UnityEngine.Debug.DrawRay (Camera.mainCamera.transform.position, ray.direction * 40, Color.green);

        var direction = Camera.mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;
        var ray = new Ray(this.transform.position,direction);

        UnityEngine.Debug.DrawRay(this.transform.position, direction * 100, Color.green);

			RaycastHit hit;

		if (Physics.Raycast (ray,out hit)) 
		{
			if(hit.distance > distance)
				return null;
			
  			if(hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != this.gameObject)
			{
				Player enemy = hit.transform.gameObject.GetComponent<Player>();
				    return enemy;
   			}
		}
		return null;
		
	}

    public Player GetTargetForward(float distance,out Player enemy,out Player ally)
    {
        enemy = null;
        ally = null;
        //OLD SHIT
        //var ray = Camera.mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        /*var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));*/
        //UnityEngine.Debug.DrawRay (Camera.mainCamera.transform.position, ray.direction * 40, Color.green);

        var direction = Camera.mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).direction;
        var ray = new Ray(this.transform.position, direction);

        UnityEngine.Debug.DrawRay(this.transform.position, direction * 100, Color.green);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance > distance)
                return null;

            if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != this.gameObject)
            {
                Player target = hit.transform.gameObject.GetComponent<Player>();
                if (IsEnemy(target))
                    enemy = target;
                if (!IsEnemy(target))
                    ally = target;
            }
        }
        return null;

    }

	//Returns all enemies in range as a list
	public List<Player> GetEnemiesInRange(float range)
	{
		List<Player> Enemies = new List<Player>();
		
		foreach(Player p in GameLogic.PlayerList)
		{
			float distance = (p.transform.position - this.transform.position).magnitude;
			if(distance <= range &&  p != this && this.IsEnemy(p))
				Enemies.Add(p);
		}
		
		return Enemies;
	}

    //Returns all Allies in range as a list
    public List<Player> GetAlliesInRange(float range)
    {
        List<Player> Allies = new List<Player>();

        foreach (Player p in GameLogic.PlayerList)
        {
            float distance = (p.transform.position - this.transform.position).magnitude;
            if (distance <= range && p != this && !this.IsEnemy(p))
                Allies.Add(p);
        }

        return Allies;
    }
	
    public bool IsEnemy(Player target)
    {
        if (this.team == target.team)
            return false;

        return true;
    }
	
	private void ExperienceHandler()
	{
		int[] XP_chart = new int[12] {100,200,300,400,500,600,700,800,900,1000,1100,1200};


		//Generate XP
		this.XP_current += 2.5f * Time.deltaTime;
		
		//If enough xp for level
		if(this.XP_current >= XP_needed)
		{		
			this.Level++;
			this.talent_point++;
			XP_current = 0;
			XP_needed = XP_chart[Level - 1];
		}


	}

    void OnGUI()
    {
		
        //FIX at some point
        if (this.HasState(CombatState.Sapped) && this.networkView.isMine)
        {
            var effect = Resources.Load("sap") as Texture2D;
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), effect);
        }

        if (this.HasState(CombatState.Blinded) && this.networkView.isMine)
        {
            var effect = Resources.Load("blinded") as Texture2D;
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), effect);
        }

    }

    //Done
    private void HandleChannelingSpells()
    {
        foreach (var spell in this.Spellbook)
        {
            if (!this.Spellbook.Exists(x => x is Spell.ChannelingSpell))
                return;

            if (spell is Spell.ChannelingSpell)
            {

                var s = spell as Spell.ChannelingSpell;
                if(s.IsCasting())
                {
                   
                  var others = this.Spellbook.FindAll(x=> x != s && x is Spell.ChannelingSpell);
                  foreach(var o in others)
                  {
                      var test = o as Spell.ChannelingSpell;
                      test.End();
                  }
                    //Spell is ready to be casted
                  if (s.IsReady())
                  {
                      s.FinishCast();
                  }
                }
                //End casting when move flag is set
                if (this.HasFlag(CombatFlag.OnMove) && s.IsCasting())
                    s.End();

            }
        }
    }
	
	
//Network messages
[RPC]
	void SendName(string s)
	{
		this.p_Name = s;
	}
	
[RPC]
	
	public void SendHP(float i)
	{
		this.HP = i;
	}
	
[RPC]
	
	void SendID(int i)
	{
		this.ID = i;
	}
	
[RPC]
	
	void SendKills(int i)
	{
		this.kills = i;
	}
	
[RPC]
public	void SendDeaths(int i)
	{
		this.deaths = i;
	}
	
[RPC]
	//USED on Ninja stealth spell only
	public void Fade(float r,float g,float b,float fade_value)
	{
		Shader shader = Shader.Find("Transparent/Diffuse");
        
        //Looks like we are reverting
        if (fade_value == 255f)
            shader = Shader.Find("Diffuse");

			var t = this.transform.GetComponentInChildren<SkinnedMeshRenderer>();
			t.renderer.material.shader = shader;
			t.renderer.material.color = new Color(r,g,b,fade_value);
			
			var asd = this.transform.GetComponentsInChildren<MeshRenderer>();
			
		foreach(var test in asd)
		{
			test.renderer.material.shader = shader;
			test.renderer.material.color = new Color(r, g, b, fade_value);
		}
		
			
           /*foreach (Transform child_t in this.transform.FindChild(@class + "_Model").transform)
            {
                var mesh_component = child_t.GetComponent<MeshRenderer>();
                if (mesh_component != null)
                {
                    child_t.renderer.material.shader = shader;
                    child_t.renderer.material.color = new Color(r, g, b, fade_value);
                }
            }*/
	}
	
[RPC]
	public void Stun(float duration)
	{
		this.BuffList.Add(new Buff.Debuff_Stun(duration));
	}

[RPC]
public void Chloroform()
{
    this.BuffList.Add(new Spell.Spell_Chloroform.Debuff_Chloroform());
}

[RPC]
public void Bleed(int owner_id)
{
    this.BuffList.Add(new Spell.Stab.Debuff_Bleed(GameLogic.PlayerList.Find(x=> x.ID == owner_id)));
}

/*[RPC]
public void Bleed(string s)
{
    Buff b = (Buff)System.Activator.CreateInstance(System.Type.GetType(s));

    this.BuffList.Add(b);
}*/


[RPC]
public void BlindingLight()
{
        this.BuffList.Add(new Spell.BlindingLight.Debuff_BlindingLight());
}


[RPC]
public void CombatFlagOnDamage()
{
    this.CombatFlags.Add(CombatFlag.OnDamaged);
}


[RPC]
public void CombatFlagOnAttack()
{
    this.CombatFlags.Add(CombatFlag.OnAttack);
}

//TODO
[RPC]
public void SendBuff(string type)
{
    Buff b = (Buff)System.Activator.CreateInstance(System.Type.GetType(type));

    this.BuffList.Add(b);
}

[RPC]
public void SendClass(string s)
{
    this.@class = s;

}
[RPC]
public void SendTeam(string s)
{
    this.team = s;

}

[RPC]
public void SendLogger(float damage,float heal)
{
    this.CombatLogger.healing = heal;
    this.CombatLogger.damage = damage;
}

[RPC]
public void SendMaterial()
{
    var transform = this.transform.GetComponentInChildren<SkinnedMeshRenderer>();
		transform.renderer.material.shader = Shader.Find("Outlined/Diffuse");
         
}

[RPC]
public void CombatTextMessage(int amount,int mode)
{
    if (amount < 1)
        return;

    if(mode == 0)
        this.GetComponent<CombatText>().Report(amount, Color.red);
    else this.GetComponent<CombatText>().Report(amount, Color.green);
}



	
}
