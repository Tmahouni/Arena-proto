using UnityEngine;
using System.Collections;

	
	public enum CombatFlag : int
	{
		OnAttack, //When doing damage to other player directly
		OnDamaged, //When player takes damage
		OnDeath, //Not used yet
        OnMove, //When player moves this flag is raised,can be forced (KnockBack,falling) or Input moving
	}

	

