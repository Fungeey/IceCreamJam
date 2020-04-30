using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.Source {
    static class Utility {
        public static int Mod(int x, int m) {
            return (x % m + m) % m;
        }

        public static Vector2 Normalized(this Vector2 v) {
            float val = 1.0f / (float)System.Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
            return new Vector2(v.X * val, v.Y * val);
        }

        public static SpriteAnimation SpriteAnimationFromParams(float frameRate, params Sprite[] sprites) {
            return new SpriteAnimation(sprites, frameRate);
        }
    }
}
