using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {


	[SerializeField] private float timeBetweenAttacks;
	[SerializeField] private float attackRadius;
	[SerializeField] private Projectile projectile;
	private Enemy targetEnemy = null;
	private float attackCounter;
	private bool isAttacking = false;

	void Start()
	{
		
	}

	void Update()
	{
		attackCounter -= Time.deltaTime;
		if (targetEnemy == null || targetEnemy.Dead) {
			Enemy nearestEnemy = GetNearestEnemyInRange();
			if (nearestEnemy != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius) {
			targetEnemy = nearestEnemy;
			}
		} else {
			if (attackCounter <= 0) {
				isAttacking = true;
				attackCounter = timeBetweenAttacks;
			} else {
				isAttacking = false;
			}
				if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius) {
					targetEnemy = null;
			}
		}
	}

	void FixedUpdate()
	{
		if (isAttacking) {
			Attack();
		}	
	}

	public void Attack() {
		isAttacking = false;
		Projectile newProjectile = Instantiate(projectile) as Projectile;
		newProjectile.transform.position = transform.position;
		PlayAttackSound(newProjectile);

		if (targetEnemy == null) {
			Destroy(newProjectile);
		} else {
			StartCoroutine(MoveProjectile(newProjectile));
		}
	}

	private IEnumerator MoveProjectile(Projectile projectile) {
		while(GetTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null ) {
			var diraction = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDiraction = Mathf.Atan2(diraction.y, diraction.x) * Mathf.Rad2Deg;
			projectile.transform.rotation = Quaternion.AngleAxis(angleDiraction, Vector3.forward);
			projectile.transform.localPosition =  Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
			yield return null;
		}
		if (projectile != null || targetEnemy == null) {
			Destroy(projectile);
		}
	}

	private float GetTargetDistance(Enemy enemy) {
		if (enemy == null) {
			enemy = GetNearestEnemyInRange();
			if (enemy == null) {
				return 0;
			}
		}
		
		return Mathf.Abs(Vector2.Distance(transform.localPosition, enemy.transform.localPosition));
	}

	private List<Enemy> GetEnemiesInRange() {
		List<Enemy> enemyInRange = new List<Enemy>();

		foreach(Enemy enemy in GameManager.Instance.enemiesList) {
			if (enemy != null && Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius) {
				enemyInRange.Add(enemy);
			}
		}
		return enemyInRange;
	}

	private Enemy GetNearestEnemyInRange() {
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;
		
		foreach(Enemy enemy in GetEnemiesInRange()) {
			float enemyDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
			if (enemyDistance < smallestDistance) {
				smallestDistance = enemyDistance;
				nearestEnemy = enemy;
			}
		}
		return nearestEnemy;
	}

	private void PlayAttackSound(Projectile projectile) {
		if (projectile.ProjectileType == ProType.ARROW) {
			GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.Arrow);
		} else if (projectile.ProjectileType == ProType.ROCK) {
			GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.Rock);
		} else if (projectile.ProjectileType == ProType.FIREBALL) {
			GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.Fireball);
		}
	}

}
