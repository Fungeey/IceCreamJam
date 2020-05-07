using IceCreamJam.Components;
using IceCreamJam.Entities.Enemies;
using IceCreamJam.WeaponSystem.EnemyWeapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.WeaponSystem {
    abstract class Projectile : Entity, IPoolable {
        public float cost;
        public float damage;
        public string texturePath;
        public float speed;
        public float lifetime;

        public Vector2 direction;

        protected Mover moveComponent;
        public RenderableComponent renderer;
        public Collider collider;
        public ProjectileLifeComponent lifeComponent;

        public bool IsNewProjectile = true;

        public Projectile() {}

        public virtual void Initialize(Vector2 direction, Vector2 position) {
            this.direction = direction;
            this.Position = position;
            this.Rotation = Mathf.Atan2(direction.Y, direction.X);

            // Set this projectile to be re-used
            if(!IsNewProjectile) {
                this.SetEnabled(true);
                if(this.lifetime != 0)
                    lifeComponent.Start();
            }
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            SetupTextures();
            this.collider = AddComponent(new CircleCollider(4) {
                LocalOffset = new Vector2(5, 0),
                PhysicsLayer = (int)Constants.PhysicsLayers.PlayerProjectiles,
                CollidesWithLayers = (int)(Constants.PhysicsLayers.Buildings | Constants.PhysicsLayers.NPC | Constants.PhysicsLayers.EnemyProjectiles),
            });

            if(lifetime != 0f) {
                this.lifeComponent = AddComponent(new ProjectileLifeComponent(lifetime));
                lifeComponent.Start();
            }

            this.moveComponent = AddComponent(new Mover());
            this.IsNewProjectile = false;
        }

        public virtual void SetupTextures() {
            var texture = Scene.Content.LoadTexture(texturePath);
            this.renderer = AddComponent(new SpriteRenderer(texture) {
                RenderLayer = Constants.Layer_Bullets
            });
        }

        /// <summary>saa
        /// Calculate path (straight, boomerang, homing, aoe, etc)
        /// </summary>
        /// <returns>The vector to move the projectile by</returns>
        public virtual Vector2 CalculateVector() {
            return  direction * speed * Time.DeltaTime;
        }

        public override void Update() {
            base.Update();
            Move();
        }

        protected void Move() {
            var vector = CalculateVector();
            moveComponent.Move(vector, out var collisionResult);

            if(collisionResult.Collider != null)
                OnHit(collisionResult);
        }

        /// <summary>
        /// Override this to instantiate sub-projectiles
        /// </summary>
        public virtual void OnHit(CollisionResult? result) {
            if(result.HasValue && result.Value.Collider.Entity is Enemy)
                (result.Value.Collider.Entity as Enemy).Damage(damage);
            if(result.HasValue && result.Value.Collider.Entity is EnemyProjectile) {
                (result.Value.Collider.Entity as EnemyProjectile).OnHit(null);
                OnHit(null);
            }

            // Pool<T>.Free(this); 
            // ^^ Every projectile must have this!
        }

        public void Reset() {
            // All important fields should be reset in Initialize,
            // when the projectile is called from the pool and given a new 
            // position and direction.

            // Disable the entity to stop rendering and colliding
            this.SetEnabled(false);
        }
    }
}
