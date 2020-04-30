using IceCreamJam.Components;
using IceCreamJam.WeaponSystem;
using IceCreamJam.WeaponSystem.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
	class Truck : Entity {

		public ArcadeRigidbody rb;
		private float health = 50;

		public override void OnAddedToScene() {
			this.Name = "Truck";
			
			var dir = AddComponent(new DirectionComponent());
			AddComponent(new PlayerInputComponent());
			AddComponent(new PlayerStateMachine());

			AddComponent(new SpriteAnimator() { RenderLayer = Constants.Layer_Truck, LayerDepth = 1 });
			AddComponent(new SetAnimator());
			AddComponent(new PlayerAnimationComponent());
			
			AddComponent(new PolygonCollider() { PhysicsLayer = (int)Constants.PhysicsLayers.Player, CollidesWithLayers = (int)Constants.PhysicsLayers.Buildings });
			var colliderManager = AddComponent(new ColliderManager(ContentPaths.Content + "truckcollision.json"));
			dir.OnDirectionChange += i => colliderManager.SetIndex((int)i);

			this.rb = AddComponent(new ArcadeRigidbody() { ShouldUseGravity = false, Elasticity = 0 });
			AddComponent(new PlayerMovementComponent());

			AddComponent(new PlayerWeaponComponent(new ScoopGun(), new PopsicleGun(), new BananaBigGun(), new BananaSmallGun()));
		}

		public override void DebugRender(Batcher batcher) {
			base.DebugRender(batcher);

			batcher.DrawCircle(this.Position, 50, Color.Green);
			batcher.DrawCircle(this.Position, 75, Color.Green);
			batcher.DrawCircle(this.Position, 150, Color.Green);
		}

		public void Damage(float damage) {
			this.health -= damage;
			if(health <= 0)
				Debug.Log("Dead!");
		}
	}
}
