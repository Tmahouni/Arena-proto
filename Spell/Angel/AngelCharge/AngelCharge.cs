using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class AngelCharge : MonoBehaviour
{
    private Player owner;
    private CharacterController target;
    private Vector3 direction;
    private float speed;
    private float duration;
    private Mover Chain;

    private List<Player> hit_enemies;

    int phase = 0;

    float asd;
    Stopwatch phase_0 = new Stopwatch();
    Stopwatch phase_1 = new Stopwatch();

    // Use this for initialization
    void Start()
    {
        target = owner.transform.GetComponent<CharacterController>();
        phase_0.Start();
        asd = Mathf.Lerp(0.5f, -0.5f, 1);
        hit_enemies = new List<Player>();
    }

    // Update is called once per frame
    void Update()
    {
       
        target.Move(direction * speed * Time.deltaTime);

        if (phase_0.Elapsed.Seconds > 0.7f)
            Destroy(this);
          

        if (phase == 0)
        {
            var enemy = Service_Combat.GetPlayersInRange(owner.transform.position, 5f);
            enemy.Remove(owner);
            if (enemy.Count >= 1 && !hit_enemies.Contains(enemy[0]))
            {
                hit_enemies.Add(enemy[0]);
                phase_0.Reset();
                phase_0.Stop();
                phase++;
                Buff b = new Buff.Debuff_Immobilize();
                b.Add(enemy[0]);
                //var eff = new EffectManager();
                //eff.PersistantEffect("Effect_AngelPrison", enemy[0].gameObject, 5f, Vector3.down * 2.5f);
                direction = target.transform.up * 0.5f + target.transform.forward * -1;
                enemy[0].ApplyDmg(40f, owner.ID);
                this.speed = 40;

                var next_enemy = Service_Combat.GetPlayersInRange(owner.transform.position, 40f);
                next_enemy.Remove(owner);
                next_enemy.Remove(enemy[0]);
                if (next_enemy.Count >= 1)
                {
                    direction = (next_enemy[0].transform.position - owner.transform.position).normalized;
                    phase = 0;
                    phase_0.Start();
                }

            }
        }
        if (phase == 1)
        {

            phase_1.Start();

            if (phase_1.Elapsed.Seconds > 1.5f)
                Destroy(this);
        }
    }



    public void SetValues(Player Target, Vector3 Direction, float Speed, float Duration)
    {
        this.owner = Target;
        this.target = Target.GetComponent<CharacterController>();
        this.direction = Direction;
        this.speed = Speed;
        this.duration = Duration;
    }

    
}

    

