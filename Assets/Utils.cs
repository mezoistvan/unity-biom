using System;

public static class Utils {

	public static int AbsDistanceTimes100 (HexCoordinates a, HexCoordinates b) {
		return (int) (Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Z - b.Z), 2)) * 100);
	}
}
