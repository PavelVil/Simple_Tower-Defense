using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProType {
	ROCK,
	ARROW,
	FIREBALL
}

public class Projectile : MonoBehaviour {

	[SerializeField] private int attackStrength;
	[SerializeField] private ProType projectileType;

	public int AttackStrength {
		get {return attackStrength;}
	}

	public ProType ProjectileType {
		get {return projectileType;}
	}

}
