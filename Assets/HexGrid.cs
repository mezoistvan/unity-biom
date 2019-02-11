using System;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	HexCell[] cells;
	Canvas gridCanvas;
	HexMesh hexMesh;
	GameState gameState;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.blue;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
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
		position.x = (x + z * 0.5f - z / 2)  * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.innerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		if (gameState.levelStarter.attackStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.ATTACK;
		} else if (gameState.levelStarter.healthStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.HEALTH;
		} else if (gameState.levelStarter.reachStarter.ToString () == new HexCoordinates (x, z).ToString ()) {
			cell.playerCellStatus = PlayerCellStatus.REACH;
		} else if (gameState.levelStarter.playerStarter.Contains (new HexCoordinates (x, z))) {
			cell.playerCellStatus = PlayerCellStatus.PLAYER;
		} else {
			cell.playerCellStatus = PlayerCellStatus.NOTHING;
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
		label.text = "";

		if (cell.playerCellStatus == PlayerCellStatus.ATTACK) {
			label.text = "\n" + gameState.levelStarter.attack.ToString();
		}
		if (cell.playerCellStatus == PlayerCellStatus.HEALTH) {
			label.text = "\n" + gameState.levelStarter.health.ToString();
		}
		if (cell.playerCellStatus == PlayerCellStatus.REACH) {
			label.text = "\n" + gameState.levelStarter.reach.ToString();
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown(0) && !gameState.turnStarted) {
			if (CellClicked().playerCellStatus != PlayerCellStatus.NOTHING) {
				gameState.turnStarted = true;
				HighlightNeighbors();
			}
		}

		if (Input.GetMouseButton(0) && gameState.turnStarted) {
			HandleInput();
		}

		if (Input.GetMouseButtonUp(0) && gameState.turnStarted) {
			gameState.turnStarted = false;
			UnHighlightNeighbors();
			gameState.grownTilesThisTurn = 0;
			RearrangeOrgans();
		}
	}

	void HighlightNeighbors() {
		Action<HexCell> doHighlight = new Action<HexCell> (DoHighlight);
		Array.ForEach (cells, doHighlight);
		hexMesh.Triangulate(cells);
	}

	private static void DoHighlight(HexCell cell) {
		if (cell.playerCellStatus != PlayerCellStatus.NOTHING) {
			for (int i = 0; i < 6; i++) {
				HexCell neighbor = cell.GetNeighbor((HexDirection) 	i);

				if ((neighbor != null) && (neighbor.playerCellStatus == PlayerCellStatus.NOTHING)) {
					neighbor.color = Color.cyan;
				}
			}
		}
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
			cell.color = touchedColor;
			cell.playerCellStatus = PlayerCellStatus.PLAYER;
			gameState.grownTilesThisTurn += 1;
			HighlightNeighbors();
		}
		hexMesh.Triangulate(cells);
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
}
