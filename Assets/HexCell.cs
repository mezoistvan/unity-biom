using UnityEngine;

public class HexCell : MonoBehaviour {
	public HexCoordinates coordinates;
	public Color color;

	[SerializeField]
	HexCell[] neighbors;

	public PlayerCellStatus playerCellStatus;

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
