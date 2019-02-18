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

	public List<HexCoordinates> enemyStarter;
}

public class GameState {
	public StarterState levelStarter;
	public bool turnStarted = false;
	public int grownTilesThisTurn = 0;

	public void setup() {
		levelStarter = new StarterState();
		levelStarter.playerStarter = new List<HexCoordinates>();
		levelStarter.playerStarter.Add(new HexCoordinates(8, 8));
		levelStarter.playerStarter.Add(new HexCoordinates(9, 8));
		levelStarter.playerStarter.Add(new HexCoordinates(9, 9));
		levelStarter.playerStarter.Add(new HexCoordinates(10, 9));
		levelStarter.playerStarter.Add(new HexCoordinates(8, 9));
		levelStarter.playerStarter.Add(new HexCoordinates(9, 10));
		levelStarter.playerStarter.Add(new HexCoordinates(7, 7));
		levelStarter.playerStarter.Add(new HexCoordinates(7, 8));
		levelStarter.playerStarter.Add(new HexCoordinates(8, 7));
		levelStarter.playerStarter.Add(new HexCoordinates(10, 10));

		levelStarter.healthStarter = new HexCoordinates(8, 8);
		levelStarter.attackStarter = new HexCoordinates(9, 9);
		levelStarter.reachStarter = new HexCoordinates(9, 8);

		levelStarter.health = 21;
		levelStarter.attack = 9;
		levelStarter.reach = 3;

		levelStarter.enemyStarter = new List<HexCoordinates>();
		levelStarter.enemyStarter.Add(new HexCoordinates(12, 12));
	}

	public static Color ColorFromStatus(PlayerCellStatus state) {
		if (state == PlayerCellStatus.PLAYER) {
			return Color.blue;
		} else if (state == PlayerCellStatus.ATTACK) {
			return Color.blue;
		} else if (state == PlayerCellStatus.HEALTH) {
			return Color.blue;
		} else if (state == PlayerCellStatus.REACH) {
			return Color.blue;
		}
		return Color.white;
	}
}
