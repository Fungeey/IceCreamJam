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

        public const int RenderLayer_Bullets = 9;
        public const int RenderLayer_UI = -10;
        public const int RenderLayer_FX = 8;

        public const int RenderLayer_Civilian = 1;
        public const int RenderLayer_Enemy = 2;
        public const int RenderLayer_Truck = 3;
        public const int RenderLayer_Buildings = 4;
        public const int RenderLayer_Vehicles = 5;

        public const float TwoPi = (float)Math.PI * 2;
        public const float HalfPi = (float)Math.PI / 2;
        public const float QuarterPi = (float)Math.PI / 4;
    }
}
