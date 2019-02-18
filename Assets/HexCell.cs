using UnityEngine;

public class HexCell : MonoBehaviour {
	public HexCoordinates coordinates;
	public Color color;
	int distance;
	public bool isFurthest = false;

	[SerializeField]
	HexCell[] neighbors;

	public PlayerCellStatus playerCellStatus;
	public EnemyCellStatus enemyCellStatus;

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
		}
	}


	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexCell[] GetNeighbors() {
		return neighbors;
	}
}
