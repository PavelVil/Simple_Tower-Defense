using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus {
	NEXT,
	PLAY,
	GAME_OVER,
	WIN
}

public class GameManager : Singleton<GameManager> {

	[SerializeField] private Enemy[] enemies;
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private int enemyPerSpawn;
	[SerializeField] private int totalEnemy = 3;
	[SerializeField] private float spawnDelay = 0.5f;
	[SerializeField] private int totalWaves = 1;
	[SerializeField] private Text totalMoneyLabel;
	[SerializeField] private Text curentWaveLabel;
	[SerializeField] private Text playButtonLabel;
	[SerializeField] private Button playButton;
	[SerializeField] private Text totalEscapedLabel;

	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private GameStatus currentState = GameStatus.PLAY;
	private AudioSource aSource;


	public List<Enemy> enemiesList = new List<Enemy>();

	private GameObject enemyParent;

	public int TotalMoney {
		get {
			return totalMoney;
			}
		
		set {
			totalMoney = value;
			totalMoneyLabel.text = totalMoney.ToString();
			}
	}

	public int TotalEscaped {
		get {return totalEscaped;}
		set {totalEscaped = value;}
	}

	public int RoundEscaped {
		get {return roundEscaped;}
		set {roundEscaped = value;}
	}

	public int TotalKilled {
		get {return totalKilled;}
		set {totalKilled = value;}
	}

	public AudioSource ASource {
		get {return aSource;}
	}

	public void RegisterEnemy(Enemy enemy) {
		enemiesList.Add(enemy);
	}

	public void UnregisterEnemy(Enemy enemy) {
		enemiesList.Remove(enemy);
		Destroy(enemy.gameObject);
	}

	public void DestroyAllEnemies() {
		foreach (Enemy enemy in enemiesList) {
			Destroy(enemy.gameObject);
		}

		enemiesList.Clear();
	}

	void Start () {
		aSource = GetComponent<AudioSource>();
		enemyParent = new GameObject("Enemies");
		playButton.gameObject.SetActive(false);
		ShowMenu();
	}
	

	void Update () {
		
	}

	IEnumerator Spawn() {
	if (enemyPerSpawn > 0 && enemiesList.Count < totalEnemy) {
		for (int i = 0; i < enemyPerSpawn; i++) {
			if (enemiesList.Count < totalEnemy) {
			Enemy enemy = Instantiate(enemies[Random.Range(0, enemies.Length)]);
			enemy.transform.position = spawnPoint.transform.position;
			enemy.transform.parent = enemyParent.transform;
			}
		}
		yield return new WaitForSeconds (spawnDelay);
		StartCoroutine(Spawn());
		}
	}

	public void AddMoney(int amount) {
		TotalMoney += amount;
	}

	public void SubtractMoney(int amount) {
		TotalMoney -= amount;
	}

	public void IsWaveOver() {
		totalEscapedLabel.text = "Escaped " + TotalEscaped + "/10";
		if ((RoundEscaped + TotalKilled) == totalEnemy) {
			SetCurrentGameState();
			ShowMenu();
		}
	}

	public void SetCurrentGameState() {
		if (totalEscaped >= 10) {
			currentState = GameStatus.GAME_OVER;
		} else if ((waveNumber == 0) && (totalKilled + roundEscaped) == 0) {
			currentState = GameStatus.PLAY;
		} else if (waveNumber >= totalWaves) {
			currentState = GameStatus.WIN;
		} else {
			currentState = GameStatus.NEXT;
		}
	}

	public void ShowMenu() {
		switch(currentState){
			case GameStatus.GAME_OVER:
				playButtonLabel.text = "Play Again!";
				aSource.PlayOneShot(SoundManager.Instance.Gameover);
				break;
			case GameStatus.NEXT:
				playButtonLabel.text = "Next Wave";
				break;
			case GameStatus.PLAY:
				playButtonLabel.text = "Play";
				break;
			case GameStatus.WIN:
				playButtonLabel.text = "You'r WIN! Game Again!";
				break;
		}
		playButton.gameObject.SetActive(true);
	}

	public void PlayButton() {
		switch(currentState) {
			case GameStatus.NEXT:
				waveNumber++;
				totalEnemy += waveNumber;
				break;
			default:
				totalEnemy = 3;
				TotalEscaped = 0;
				TotalMoney = 10;
				totalEscapedLabel.text = TotalMoney.ToString();
				totalEscapedLabel.text = "Escaped " + TotalEscaped + "/10";
				totalWaves = 0;
				TowerManager.Instance.DestroyAllTowers();
				TowerManager.Instance.RenameTagsBuildSites();
				aSource.PlayOneShot(SoundManager.Instance.NewGame);
				break;
		}
		DestroyAllEnemies();
		TotalKilled = 0;
		RoundEscaped = 0;
		curentWaveLabel.text = "Wave " + (waveNumber + 1);
		StartCoroutine(Spawn());
		playButton.gameObject.SetActive(false);
	}
}
