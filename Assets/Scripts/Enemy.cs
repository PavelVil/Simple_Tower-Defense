using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[SerializeField] private Transform[] wayPoints;
	[SerializeField] private float navigationUpdate = 0.01f;
	[SerializeField] private int healthPoints;
	[SerializeField] private int rewardAmount;
	private float navigationTime = 0;
	private Transform exit;
	private int target = 0;
	private bool isDead = false;
	private Collider2D enemyCollider;
	private Animator anim;

	public bool Dead {
		get {return isDead;}
		set {isDead = value;}
	}


	void Start () {
		GameManager.Instance.RegisterEnemy(this);
		exit = GameObject.FindGameObjectWithTag("Exit").transform;
		enemyCollider = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
	}
	

	void Update () {
		MoveEnemy();
	}

	void MoveEnemy() {
		if (wayPoints != null && !isDead) {
		navigationTime += Time.deltaTime;
		if (navigationTime > navigationUpdate) {
			if (target < wayPoints.Length) {
				transform.position = Vector3.MoveTowards(transform.position, wayPoints[target].position, navigationTime);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, exit.position, navigationTime);
			}
		}
			navigationTime = 0;
		}
	}

	public void EnemyHit(int hitPoints) {
		if (healthPoints - hitPoints > 0) {
			healthPoints -= hitPoints;
			GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.Hit);
			anim.Play("Hurt");
		} else {
			anim.SetTrigger("didDie");
			Die();
		}
	}

	public void Die() {
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled++;
		GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.Death);
		GameManager.Instance.AddMoney(rewardAmount);
		GameManager.Instance.IsWaveOver();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "WayPoint") {
			target++;
		} 

		if (other.tag == "Exit") {
			GameManager.Instance.TotalEscaped++;
			GameManager.Instance.RoundEscaped++;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.IsWaveOver();
		}

		if (other.tag == "Projectiles") {
			Projectile projectile = other.gameObject.GetComponent<Projectile>();
			if (projectile != null) {
			EnemyHit(projectile.AttackStrength);
			Destroy(other.gameObject);
			}
		}
	}

}
