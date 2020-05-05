using IceCreamJam.Components;

using IceCreamJam.Entities;
using IceCreamJam.Entities.Enemies;
using IceCreamJam.Rendering;
using IceCreamJam.RoadSystem;
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
            Physics.RaycastsStartInColliders = false;

            loader = AddSceneComponent(new TilemapLoader());
            roadSystem = AddSceneComponent(new RoadSystemComponent());


            SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAll);
            AddRenderer(new DefaultRenderer());

            AddEntityProcessor(new HomingProjectileSystem(new Matcher().All(typeof(HomingTargetComponent))));
            AddEntityProcessor(new CivilianSystem(new Matcher().All(typeof(CivilianComponent))));

            AddRenderer(new RoadRenderer());
        }

        public override void OnStart() {
            truck = AddEntity(new Truck() { Position = new Vector2(25 * 32, 25 * 32) } );
            UICanvas = AddEntity(new UIManager());

            //for(int i = 0; i < 5; i++) 
            //    AddEntity(new Civilian(ContentPaths.NPC + $"NPC{i}.png") { Position = new Vector2(Screen.Width / 2 + i * 32, Screen.Height / 2) });

            for(int i = 0; i < 0; i++) {
                var d = Pool<Doctor>.Obtain();
                d.Initialize(new Vector2(Nez.Random.NextInt(Screen.Width), Nez.Random.NextInt(Screen.Height)));

                if(d.isNewEnemy)
                    AddEntity(d);
            }

            System.Threading.Thread.Sleep(1000);

            for(int i = 0; i < 0; i++) {
                //AddEntity(new TestVehicle() { Position = new Vector2(Nez.Random.NextInt(200 * 32), Nez.Random.NextInt(200 * 32)) });
                AddEntity(new TestVehicle() { Position = new Vector2(Nez.Random.NextInt(50 * 32), Nez.Random.NextInt(50 * 32)) });
            }

            AddEntity(new Crosshair());

            loader.Load(ContentPaths.SmallRoadTest);
            Camera.MinimumZoom = 0.1f;
            Camera.ZoomOut(0.75f);
            Camera.AddComponent(new FollowCamera(truck));
        }
    }
}
