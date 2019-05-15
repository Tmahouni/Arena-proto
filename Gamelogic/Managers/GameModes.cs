using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public abstract class GameMode : MonoBehaviour
{

    private bool running;
    public Player winners;
    public abstract void WinCondition();
    

    public bool Run()
    {
        if (running)
        {
            this.WinCondition();
            return true;
        }
        UnityEngine.Debug.Log("Going to MatchEnded State");
        return false;
    }

    private void End()
    {
        UnityEngine.Debug.Log("End()");
        foreach (var player in GameLogic.PlayerList)
        {
            player.BuffList.Add(new Buff.Debuff_Stun(5f));
        }
    }

    //Deadmatch
    public class DeathMatch : GameMode
    {
        private int killcount;

        public DeathMatch(int Killcount)
        {
            this.running = true;
            this.killcount = Killcount;
        }

        public override void WinCondition()
        {
            foreach (var player in GameLogic.PlayerList)
            {
                //End
                if (player.kills >= this.killcount)
                {
                    winners = player;
                    End();
                    this.running = false;
                    return;
                }

            }
        }

    }
    //TODO: ALL
    public class TeamCompetitive : GameMode
    {
        public enum MatchPhase { WARMUP,RUNNING,ROUND_END,MATCH_ENDED }
        Stopwatch round_timer = new Stopwatch();
        private List<Player> Team1;
        private List<Player> Team2;
        private MatchPhase Phase;

        public TeamCompetitive()
        {
            Team1 = new List<Player>();
            Team2 = new List<Player>();
            this.running = true;
            this.Phase = MatchPhase.RUNNING;
        }

        public override void WinCondition()
        {
            Team1 = (from player in GameLogic.PlayerList where player.team == "Team1" select player).ToList();
            Team2 = (from player in GameLogic.PlayerList where player.team == "Team2" select player).ToList();

           

            if (Phase == MatchPhase.RUNNING)
            {
                UnityEngine.Debug.Log("Running..");
                int i = 0;
                foreach (Player p in Team1)
                {
					p.networkView.RPC("SendTeam",RPCMode.All,p.team);
					
                    if (p.IsDead())
                        i++;

                    if (i >= Team1.Count)
                    {
                        UnityEngine.Debug.Log("Team 1 dead");
                        Phase++;
                        round_timer.Start();
                        End();
                        return;
                    }
                }

                foreach (Player p in Team2)
                {
					p.networkView.RPC("SendTeam",RPCMode.All,p.team);
					
                    if (p.IsDead())
                        i++;

                    if (i >= Team2.Count)
                    {
                        UnityEngine.Debug.Log("Team 2 dead");
                        Phase++;
                        round_timer.Start();
                        End();
                        return;
                    }
                }

            }
            
            if (Phase == MatchPhase.ROUND_END)
            {
                UnityEngine.Debug.Log("Round ended");
                if (round_timer.Elapsed.Seconds >= 5)
                {
                    round_timer.Reset();
                    round_timer.Stop();
                    foreach (Player p in GameLogic.PlayerList)
                    {
                        p.ReSpawn();
                        p.ReSpawn();
                    }
                    Phase--;
                }
            }

            

        }
    }


}
