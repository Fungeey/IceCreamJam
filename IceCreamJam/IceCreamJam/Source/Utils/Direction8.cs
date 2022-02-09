using Microsoft.Xna.Framework;
using Nez;
using System;

namespace IceCreamJam {
	public enum Direction8 {
		East, SouthEast, South, SouthWest, West, NorthWest, North, NorthEast,
	}

	public static class Direction8Ext {
		public static Vector2 ToVector2(this Direction8 d) {
			switch(d) {
				case Direction8.East:
					return new Vector2(1, 0);
				case Direction8.SouthEast:
					return new Vector2(1, 1);
				case Direction8.South:
					return new Vector2(0, 1);
				case Direction8.SouthWest:
					return new Vector2(-1, 1);
				case Direction8.West:
					return new Vector2(-1, 0);
				case Direction8.NorthWest:
					return new Vector2(-1, -1);
				case Direction8.North:
					return new Vector2(0, -1);
				case Direction8.NorthEast:
					return new Vector2(1, -1);
				default:
					throw new ArgumentOutOfRangeException(nameof(d));
			}
		}

		public static Vector2 ToNormalizedVector2(this Direction8 d) {
			return d.ToVector2().Normalized();
		}

		public static Vector2 ToNormalizedVector2(this Direction8 d, float length) {
			return d.ToVector2().Normalized() * length;
		}

		public static bool IsVertical(this Direction8 d) {
			return d == Direction8.North || d == Direction8.South;
		}

		public static bool IsHorizontal(this Direction8 d) {
			return d == Direction8.West || d == Direction8.East;
		}

		public static bool IsOrthogonal(this Direction8 d) {
			return d.IsHorizontal() || d.IsVertical();
		}

		public static Direction8 FromVector2(Vector2 vec) {
			var rad = Mathf.Atan2(vec.Y, vec.X);
			var dir = (Utility.Mod((int)(rad * Mathf.Rad2Deg), 360) + 22.5f) / 45;
			return (Direction8)Utility.Mod((int)dir, 8);
		}


		public static Direction8 RotateClockwise(this Direction8 d, int increments) {
			return (Direction8)Utility.Mod((int)d + increments, 8);
		}

		public static int Difference(this Direction8 d, Direction8 o) {
			var num = Utility.Mod(o - d, 8);
			if (num > 4) num -= 8;
			return num;
		}
	}
}
