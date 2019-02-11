using System.Collections.Generic;
using UnityEngine;

public struct StarterState {
	public List<HexCoordinates> playerStarter;
	public HexCoordinates healthStarter;
	public HexCoordinates attackStarter;
	public HexCoordinates reachStarter;

	public int health;
	public int attack;
	public int reach;
}

public class GameState {
	public StarterState levelStarter;
	public bool turnStarted = false;
	public int grownTilesThisTurn = 0;

	public void setup() {
		levelStarter = new StarterState();
		levelStarter.playerStarter = new List<HexCoordinates>();
		levelStarter.playerStarter.Add(new HexCoordinates(1, 1));
		levelStarter.playerStarter.Add(new HexCoordinates(2, 1));
		levelStarter.playerStarter.Add(new HexCoordinates(2, 2));

		levelStarter.healthStarter = new HexCoordinates(1, 1);
		levelStarter.attackStarter = new HexCoordinates(2, 2);
		levelStarter.reachStarter = new HexCoordinates(2, 1);

		levelStarter.health = 9;
		levelStarter.attack = 7;
		levelStarter.reach = 3;
	}

	public static Color ColorFromStatus(PlayerCellStatus state) {
		if (state == PlayerCellStatus.PLAYER) {
			return Color.blue;
		} else if (state == PlayerCellStatus.PLAYER) {
			return Color.blue;
		} else if (state == PlayerCellStatus.ATTACK) {
			return Color.red;
		} else if (state == PlayerCellStatus.HEALTH) {
			return Color.green;
		} else if (state == PlayerCellStatus.REACH) {
			return Color.yellow;
		}
		return Color.white;
	}
}
