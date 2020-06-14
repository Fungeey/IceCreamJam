using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Collections.Generic;

namespace IceCreamJam {
    static class Utility {
        public static int Mod(int x, int m) {
            return (x % m + m) % m;
        }

        public static Vector2 Normalized(this Vector2 v) {
            float val = 1.0f / (float)System.Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
            return new Vector2(v.X * val, v.Y * val);
        }

        public static Vector2 RandomPointOnCircle() {
            var angle = Random.NextAngle();
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).Normalized();
        }

        public static Vector2 RandomPointOnCircle(float radius) => RandomPointOnCircle() * radius;

        public static List<T> SelectFromList<T>(List<T> list, params int[] indices) {
            var outList = new List<T>();
            foreach(int i in indices) {
                if(i >= 0 && i < list.Count)
                    outList.Add(list[i]);
			}
            return outList;
		}

        public static IList<T> Shuffle<T>(this IList<T> list) {
            ListExt.Shuffle(list);
            return list;
        }

        public static SpriteAnimation SpriteAnimationFromParams(float frameRate, params Sprite[] sprites) {
            return new SpriteAnimation(sprites, frameRate);
        }
    }
}
