using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	private TowerButton towerBtnPressed;
	private SpriteRenderer spriteRenderer;
	private List<Tower> towerList = new List<Tower>();
	private List<Collider2D> buildList = new List<Collider2D>();
	private Collider2D buildTile;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

			if (hit.collider.tag == "BuildSite" && towerBtnPressed != null) {
				buildTile = hit.collider;
				hit.collider.tag = "BuildSiteFull";
				RegisterBuildSite(buildTile);
				PlaceTower(hit);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			GameManager.Instance.AddMoney(towerBtnPressed.TowerPrice);
			DisableDragSprite();
		}

		if (spriteRenderer.enabled) {
				FollowMouse();
		}
	}

	public void RegisterBuildSite(Collider2D buildTag) {
		buildList.Add(buildTag);
	}

	public void RegisterTower(Tower tower) {
		towerList.Add(tower);
	}

	public void RenameTagsBuildSites() {
		foreach (Collider2D buildTag in buildList) {
			buildTag.tag = "BuildSite";
		}
		buildList.Clear();
	}

	public void DestroyAllTowers() {
		foreach(Tower tower in towerList) {
			Destroy(tower.gameObject);
		}
		towerList.Clear();
	}

	public void BuyTower (int price) {
		GameManager.Instance.SubtractMoney(price);
	}

	public void SelectedTower(TowerButton towerSelected) {
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney) {
			BuyTower(towerSelected.TowerPrice);
			towerBtnPressed = towerSelected;
			EnabledDragSprite(towerBtnPressed.DragSprite);
		}
	}

	public void PlaceTower(RaycastHit2D hit) {
		if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null) {
			Tower newTower = Instantiate(towerBtnPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			GameManager.Instance.ASource.PlayOneShot(SoundManager.Instance.TowerBuild);
			RegisterTower(newTower);
			DisableDragSprite();
		}
	}

	public void FollowMouse() {
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void EnabledDragSprite(Sprite sprite) {
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void DisableDragSprite() {
		spriteRenderer.enabled = false;
		towerBtnPressed = null;
	}

}
