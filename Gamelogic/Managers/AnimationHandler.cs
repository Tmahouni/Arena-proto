using UnityEngine;
using System.Collections;

public class AnimationHandler
{
	
	public string prev_animation;
	private string current_animation;
	
	private Player player;
	
	public AnimationHandler(Player p)
	{
		this.player = p;
	}
	
	
	public void PlayAnimation(string name,float speed)
	{
        //player.networkView.RPC("PlayAnimation", RPCMode.All, name, speed);
		
		if(player.transform.GetComponentInChildren<Animation>().animation.IsPlaying("run") && name == "attack")
		{
			player.transform.GetComponentInChildren<Animation>().animation.Stop();
			player.transform.GetComponentInChildren<Animation>().animation.Play("run_attack");
			return;
		}
		
		if(player.transform.GetComponentInChildren<Animation>().animation.IsPlaying("run") && name == "throw")
		{
			player.transform.GetComponentInChildren<Animation>().animation.Stop();
			player.transform.GetComponentInChildren<Animation>().animation.Play("run_throw");
			return;
		}
		
		if(name == "jump")
			player.transform.GetComponentInChildren<Animation>().animation["jump"].speed = 1.2f;

        player.transform.GetComponentInChildren<Animation>().animation[name].speed = speed;
        player.transform.GetComponentInChildren<Animation>().animation.Play(name);
		
		this.prev_animation = name;
		
	}
	
	

}
