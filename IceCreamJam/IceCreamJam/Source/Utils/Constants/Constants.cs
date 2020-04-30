using System;

namespace IceCreamJam {
    class Constants {

        public const string TiledLayerBuildings = "Buildings";
        public const string TiledPropertyID = "TileID";

        public const int GlobalFPS = 8;

        [Flags]
        public enum PhysicsLayers {
            None = 0,
            Buildings = 1,
            Player = 2,
            NPC = 4,
            PlayerProjectiles = 8,
            EnemyProjectiles = 16
        }

        public const int Layer_Map = 100;
        public const int Layer_WeaponBase = 10;
        public const int Layer_Bullets = 9;
        public const int Layer_WeaponOver = 8;
        public const int Layer_UI = -10;

        public const int Layer_Truck = 15;
        public const int Layer_NPC = 5;
        public const int Layer_Buildings = 6;

        public const int Layer_FX = 8;
    }
}
