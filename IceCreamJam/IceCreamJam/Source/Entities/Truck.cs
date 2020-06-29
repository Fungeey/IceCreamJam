using IceCreamJam.Components;
using IceCreamJam.Source.Components;
using IceCreamJam.WeaponSystem;
using IceCreamJam.WeaponSystem.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class Truck : Entity {
        public const float MaxHealth = 50;

        public override void OnAddedToScene() {
            Name = "Truck";

            AddComponent(new HealthComponent(MaxHealth));
            AddComponent(new AmmoComponent(50));
            AddComponent(new DirectionComponent());
            AddComponent(new PlayerInputComponent());

            AddComponent(new SpriteAnimator() { RenderLayer = Constants.RenderLayer_Truck, LayerDepth = 1 });
            AddComponent(new RenderSorterComponent());
            AddComponent(new SetAnimator());

            AddComponent(new PolygonCollider() { PhysicsLayer = (int)Constants.PhysicsLayers.Player, CollidesWithLayers = (int)Constants.PhysicsLayers.Buildings });
            AddComponent(new ColliderManager(ContentPaths.Content + "truckcollision.json"));

            //AddComponent(new ArcadeRigidbody() { ShouldUseGravity = false, Elasticity = 0 });
            AddComponent(new Mover());
            AddComponent(new PlayerMovementComponent());
            AddComponent(new PlayerDispenseComponent());

            AddComponent(new PlayerWeaponComponent(new ScoopGun(), new PopsicleGun(), new BananaBigGun(), new BananaSmallGun()));
            AddComponent(new PlayerStateMachine());
        }

        public Vector2 GetVectorToMouse() {
            return Scene.Camera.MouseToWorldPoint() - Position;
        }
    }
}
