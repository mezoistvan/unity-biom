using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	public int width = 60;
	public int height = 60;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	HexCell[] cells;
	Canvas gridCanvas;
	HexMesh hexMesh;
	GameObject attackSprite;
	GameObject healthSprite;
	GameObject reachSprite;
	GameObject enemySprite;
	GameState gameState;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.blue;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
		attackSprite = GameObject.Find("Attack");
		healthSprite = GameObject.Find("Health");
		reachSprite = GameObject.Find("Reach");
		enemySprite = GameObject.Find("Enemy");
		gameState = new GameState();
		gameState.setup();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void Start () {
		hexMesh.Triangulate(cells);
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + (z * 0.5f) - (z / 2)) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * HexMetrics.outerRadius * 1.5f;

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		if (gameState.levelStarter.attackStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.ATTACK;
			cell.enemyCellStatus = EnemyCellStatus.NOTHING;
			attackSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
		} else if (gameState.levelStarter.healthStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.HEALTH;
			cell.enemyCellStatus = EnemyCellStatus.NOTHING;
			healthSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
		} else if (gameState.levelStarter.reachStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.REACH;
			cell.enemyCellStatus = EnemyCellStatus.NOTHING;
			reachSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
		} else if (gameState.levelStarter.playerStarter.Contains (new HexCoordinates (x, z))) {
			cell.playerCellStatus = PlayerCellStatus.PLAYER;
			cell.enemyCellStatus = EnemyCellStatus.NOTHING;
		} else if (gameState.levelStarter.enemyStarter.Contains (new HexCoordinates (x, z))) {
			cell.enemyCellStatus = EnemyCellStatus.ENEMY;
			cell.playerCellStatus = PlayerCellStatus.NOTHING;
			enemySprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
		} else {
			cell.playerCellStatus = PlayerCellStatus.NOTHING;
			cell.enemyCellStatus = EnemyCellStatus.NOTHING;
		}
		cell.color = GameState.ColorFromStatus(cell.playerCellStatus);

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		// label.text = cell.coordinates.ToStringOnSeparateLines();
	}

	void Update () {
		if (Input.GetMouseButtonDown(0) && !gameState.turnStarted) {
			if (CellClicked().playerCellStatus != PlayerCellStatus.NOTHING) {
				gameState.turnStarted = true;
				HighlightNeighbors();
				HighlightFurthest(CellClicked());
				hexMesh.Triangulate(cells);
			}
		}

		if (Input.GetMouseButton(0) && gameState.turnStarted) {
			HandleInput();
			HighlightFurthest(CellClicked());
			hexMesh.Triangulate(cells);

		}

		if (Input.GetMouseButtonUp(0) && gameState.turnStarted) {
			gameState.turnStarted = false;
			UnHighlightNeighbors();
			gameState.grownTilesThisTurn = 0;
			RearrangeOrgans();
			hexMesh.Triangulate(cells);
		}
	}

	void HighlightNeighbors() {
		Action<HexCell> doHighlight = new Action<HexCell> (DoHighlight);
		Array.ForEach (cells, doHighlight);
	}

	private static void DoHighlight(HexCell cell) {
		if (cell.playerCellStatus != PlayerCellStatus.NOTHING) {
			for (int i = 0; i < 6; i++) {
				HexCell neighbor = cell.GetNeighbor((HexDirection) i);

				if ((neighbor != null) && (neighbor.playerCellStatus == PlayerCellStatus.NOTHING)) {
					neighbor.color = Color.cyan;
				}
			}
		}
	}

	void HighlightFurthest(HexCell sourceCell) {
		if (gameState.grownTilesThisTurn >= gameState.levelStarter.reach) {
			return;
		}
		HexCell[] playerCells = Array.FindAll(cells, c => c.playerCellStatus != PlayerCellStatus.NOTHING);
		int furthestDistance = 0;
		HexCell furthestCell = null;
		for (int i = 0; i < playerCells.Length; i++) {
			playerCells[i].isFurthest = false;
			int distance = playerCells[i].coordinates.DistanceTo(sourceCell.coordinates);
			if (distance > furthestDistance) {
				furthestDistance = distance;
				furthestCell = playerCells[i];
			}
		}
		furthestCell.isFurthest = true;
		RemoveFurthestColor();
		furthestCell.color = Color.grey;
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}

	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);

		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		if ((gameState.grownTilesThisTurn < gameState.levelStarter.reach) && (cell.playerCellStatus == PlayerCellStatus.NOTHING)) {
			for (int i = 0; i < 6; i++) {
				HexCell neighbor = cell.GetNeighbor((HexDirection) i);

				if ((neighbor != null) && (neighbor.playerCellStatus != PlayerCellStatus.NOTHING)) {
					cell.color = touchedColor;
					cell.playerCellStatus = PlayerCellStatus.PLAYER;
					gameState.grownTilesThisTurn += 1;
					RemoveFurthestCell();
					UnHighlightNeighbors();
					HighlightNeighbors();
					break;
				}
			}
		}
	}

	void RemoveFurthestCell() {
		HexCell[] playerCells = Array.FindAll(cells, c => c.playerCellStatus != PlayerCellStatus.NOTHING);
		foreach (HexCell cell in playerCells) {
			if (cell.isFurthest) {
				cell.playerCellStatus = PlayerCellStatus.NOTHING;
				cell.color = GameState.ColorFromStatus(cell.playerCellStatus);
			}
		}
		RemoveFurthestColor();
		HighlightNeighbors();
		RearrangeOrgans();
	}

	void RemoveFurthestColor() {
		HexCell[] playerCells = Array.FindAll(cells, c => c.playerCellStatus != PlayerCellStatus.NOTHING);
		foreach (HexCell cell in playerCells) {
			cell.color = GameState.ColorFromStatus(cell.playerCellStatus);
		}
	}

	HexCell CellClicked() {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (inputRay, out hit)) {
			Vector3 position = transform.InverseTransformPoint(hit.point);
			HexCoordinates coordinates = HexCoordinates.FromPosition(position);

			int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
			return cells[index];
		}
		return null;
	}

	void RearrangeOrgans() {
		HexCell[] playerCells = Array.FindAll(cells, c => c.playerCellStatus != PlayerCellStatus.NOTHING);
		int tX = 0;
		int tZ = 0;
		for (int i = 0; i < playerCells.Length; i++) {
			tX += playerCells[i].coordinates.X;
			tZ += playerCells[i].coordinates.Z;
		}
		HexCoordinates centerCoordinates = new HexCoordinates((int)Math.Round((double)(tX / playerCells.Length), 0), (int)Math.Round((double)(tZ / playerCells.Length), 0));

		List<HexCell> orderedPlayerCells = new List<HexCell>(playerCells);
		orderedPlayerCells.Sort((a, b) => Utils.AbsDistanceTimes100(a.coordinates, centerCoordinates) - Utils.AbsDistanceTimes100(b.coordinates, centerCoordinates));

		foreach (HexCell cell in cells) {
			if (cell.playerCellStatus != PlayerCellStatus.NOTHING) {
				if (cell.coordinates.ToString() == orderedPlayerCells[0].coordinates.ToString()) {
					cell.playerCellStatus = PlayerCellStatus.ATTACK;
					attackSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
				} else if (cell.coordinates.ToString() == orderedPlayerCells[1].coordinates.ToString()) {
					cell.playerCellStatus = PlayerCellStatus.HEALTH;
					healthSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
				} else if (cell.coordinates.ToString() == orderedPlayerCells[2].coordinates.ToString()) {
					cell.playerCellStatus = PlayerCellStatus.REACH;
					reachSprite.transform.position = HexCoordinates.ToPosition(cell.coordinates);
				} else {
					cell.playerCellStatus = PlayerCellStatus.PLAYER;
				}
				cell.color = GameState.ColorFromStatus(cell.playerCellStatus);
			}
		}
	}

	void UnHighlightNeighbors() {
		Action<HexCell> undoHighlight = new Action<HexCell>(UndoHighlight);
		Array.ForEach(cells, undoHighlight);
		hexMesh.Triangulate(cells);
	}

	private static void UndoHighlight(HexCell cell) {
		if (cell.playerCellStatus == PlayerCellStatus.NOTHING) {
			cell.color = Color.white;
		}
	}

	public void FindDistancesTo (HexCell cell) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = cell.coordinates.DistanceTo(cells[i].coordinates);
		}
	}

}
