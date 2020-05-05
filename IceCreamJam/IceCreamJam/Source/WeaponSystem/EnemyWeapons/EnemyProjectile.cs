using IceCreamJam.Components;
using IceCreamJam.Entities;
using IceCreamJam.Source.Components;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.WeaponSystem.EnemyWeapons {
    class EnemyProjectile : Projectile {
        public EnemyProjectile() {

        }

        public override void Initialize(Vector2 direction, Vector2 position) {
            base.Initialize(direction, position);
        }

        public override void OnAddedToScene() {
            SetupTextures();
            this.collider = AddComponent(new CircleCollider(4) {
                LocalOffset = new Vector2(5, 0),
                PhysicsLayer = (int)Constants.PhysicsLayers.EnemyProjectiles,
                CollidesWithLayers = (int)(Constants.PhysicsLayers.Buildings | Constants.PhysicsLayers.Player | Constants.PhysicsLayers.PlayerProjectiles),
            });

            if (lifetime != 0f) {
                this.lifeComponent = AddComponent(new ProjectileLifeComponent(lifetime));
                lifeComponent.Start();
            }

            this.moveComponent = AddComponent(new Mover());
            this.IsNewProjectile = false;
        }

        public override void OnHit(CollisionResult? result) {
            HealthComponent health;
            if (result.HasValue) {
                if ((health = result.Value.Collider.Entity.GetComponent<HealthComponent>()) != null)
                health.Damage(damage);
            }
        }
    }
}
