namespace IceCreamJam {
    class ContentPaths {
        public const string Content = "Content/";
        
        public const string Shaders = Content + "Shaders/";
        public const string MaskEffect = Shaders + "UIMask.mgfxo";

        #region Sprites
        public const string Sprites = Content + "Sprites/";

        public const string BoxSprite = Sprites + "Box.png";
        public const string EmptySprite = Sprites + "Empty.png";

        public const string Truck = Sprites + "Truck/";
        #region NPC
        public const string NPC = Sprites + "NPC/";
        #endregion
        #region Enemies
        public const string Enemies = Sprites + "Enemies/";
        public const string Doctor = Enemies + "Doctor/";
        #endregion
        #region Weapons
        public const string Weapons = Sprites + "Weapons/";
        public const string TestProjectile = Weapons + "TestProjectile.png";
        public const string TestProjectile2 = Weapons + "TestProjectile2.png";

        public const string Scoop = Weapons + "Scoop/";
        public const string Scoop_Base = Scoop + "Scoop_Base.png";
        public const string Scoop_Cone = Scoop + "Scoop_Cone.png";
        public const string Scoop_Splat = Scoop + "Scoop_Splat.png";

        public const string Popsicle = Weapons + "Popsicle/";
        public const string Popsicle_Base = Popsicle + "Popsicle_Base.png";
        public const string Popsicle_Stick = Popsicle + "Popsicle_Stick.png";
        public const string Popsicle_Shatter = Popsicle + "Popsicle_Shatter.png";

        public const string Banana = Weapons + "Banana/";
        public const string Banana_Base_Big = Banana + "Banana_Base_Big.png";
        public const string Banana_Base_Small = Banana + "Banana_Base_Small.png";
        public const string Banana_Big = Banana + "Banana_Big.png";
        public const string Banana_Small = Banana + "Banana_Small.png";
        #endregion
        #region UI
        public const string UI = Sprites + "UI/";

        public const string Crosshair = UI + "Crosshair/";
        public const string Crosshair_Diagonal = Crosshair + "Crosshair_Diagonal.png";
        public const string Crosshair_Vertical = Crosshair + "Crosshair_Vertical.png";
        public const string Crosshair_Transition = Crosshair + "Crosshair_Transition.png";

        public const string HealthGUIEmpty = UI + "UI Empty.png";
        public const string HealthGUIFull = UI + "UI Full.png";
        public const string AmmoFrame = UI + "AmmoFrame.png";
        public const string WeaponIcons = UI + "WeaponIcons.png";

        public const string HealthBar = UI + "HealthBar/";
        public const string HealthBarFrame = HealthBar + "Frame.png";
        public const string HealthInternal = HealthBar + "Health_Internal.png";
        public const string DashInternal = HealthBar + "Dash_Internal.png";
        public const string SpeedInternal = HealthBar + "Speed_Internal.png";
        public const string SpeedInternalMaxed = HealthBar + "Speed_Internal_Maxed.png";
        #endregion
        #endregion

        #region Maps
        public const string TiledMaps = Content + "TiledMaps/";
        public const string Test1 = TiledMaps + "Test1.tmx";
        public const string RoadTest = TiledMaps + "RoadTest.tmx";
        public const string SmallRoadTest = TiledMaps + "SmallRoadTest.tmx";
        #endregion

        #region
        public const string Tilesets = Content + "TileSets/";
        public const string Buildings = Tilesets + "Buildings/";
        public const string BrownHouse = Buildings + "BrownHouse.png";
        public const string GreenHouse = Buildings + "GreenHouse.png";
        #endregion
    }
}
