using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Entities.Enemies {
    abstract class Enemy : NPC, IPoolable {

        public RenderableComponent renderer;
        public static Truck truck;
        protected Mover mover;

        public float health, maxHealth;
        public bool isNewEnemy = true;

        public Enemy() {
            Name = "Enemy";
        }

        public virtual void Initialize(Vector2 position) {
            this.Position = position;
            health = maxHealth;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            AddComponent(new HomingTargetComponent());
            this.mover = AddComponent(new Mover());

            isNewEnemy = false;
        }

        public override void Update() {
            base.Update();

            if(truck == null)
                truck = (Truck)Scene.FindEntity("Truck");
        }

        public virtual void Damage(float damage) {
            this.health -= damage;

            if(this.health <= 0) {
                OnDeath();
            }
        }

        public virtual void OnDeath() {

        }

        public virtual void Reset() {
            // Disable the entity to stop rendering and colliding
            this.SetEnabled(false);
        }
    }
}
