using System;

namespace IceCreamJam {
    class Constants {


        #region Tiled
        public const string TiledLayerBackground = "Background";
        public const string TiledLayerBuildings = "Buildings";
        public const int TiledCellSize = 32;
        public const string TiledPropertyID = "TileID";
        public const string TiledIDBuilding = "Building";
        public const string TiledIDSidewalk = "Sidewalk";
        public const string TiledIDRoad = "Road";
        public const string TiledIDGrass = "Grass";
        public const string TiledIDIntersection = "Intersection";
        #endregion

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

        public const int Layer_FX = 8;
        public const int RenderLayer_Buildings = 4;

        public const float TwoPi = (float)Math.PI * 2;
        public const float HalfPi = (float)Math.PI / 2;
        public const float QuarterPi = (float)Math.PI / 4;
    }
}
