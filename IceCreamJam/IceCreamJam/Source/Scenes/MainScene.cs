﻿using IceCreamJam.Components;
using IceCreamJam.Entities;
using IceCreamJam.Entities.Civilians;
using IceCreamJam.Entities.Enemies;
using IceCreamJam.Rendering;
using IceCreamJam.RoadSystem;
using IceCreamJam.Source.Components;
using IceCreamJam.Systems;
using IceCreamJam.Tiled;
using IceCreamJam.UI;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Scenes {
    class MainScene : Scene {

        TilemapLoader loader;
        public RoadSystemComponent roadSystem;
        public Truck truck;
        public UIManager UICanvas;

        public override void Initialize() {
            loader = AddSceneComponent(new TilemapLoader());
            roadSystem = AddSceneComponent(new RoadSystemComponent());

            AddRenderer(new DefaultRenderer());
            RenderableComponentList.CompareUpdatableOrder = new PositionBasedRenderSorter();

            AddEntityProcessor(new HomingProjectileSystem(new Matcher().All(typeof(HomingTargetComponent))));
            AddEntityProcessor(new CivilianSystem(new Matcher().All(typeof(CivilianComponent))));

            AddRenderer(new RoadRenderer());
        }

        public override void OnStart() {
            loader.Load(ContentPaths.Test1);
            var map = loader.tiledMap.map;

            truck = AddEntity(new Truck() { Position = new Vector2(Screen.Width / 2 + 500, Screen.Height / 2 + 500) });
            UICanvas = AddEntity(new UIManager());

            for(int i = 1; i < 10; i++) 
                AddEntity(new Civilian() { Position = new Vector2(Screen.Width / 2 + i * 32, Screen.Height / 2) });

            for(int i = 0; i < 1; i++) {
                var d = Pool<Doctor>.Obtain();
                
                d.Initialize(new Vector2(Random.NextInt(map.WorldWidth), Random.NextInt(map.WorldHeight)));

                if (d.isNewEnemy)
                    AddEntity(d);
            }

            AddEntity(new Crosshair());
            
            Camera.MinimumZoom = 0.1f;
            Camera.ZoomOut(0.6f);
            Camera.AddComponent(new CameraFollowComponent(Camera, truck) {
                bounds = loader.mapBounds
            });
            Camera.Entity.UpdateOrder = truck.UpdateOrder + 1;
        }

		public override void Update() {
			base.Update();

			if(Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P)) {
                var ambulance = new Ambulance() { Position = loader.carSpawnPoints.RandomItem() };
                AddEntity(ambulance);
            }
		}
	}
}
