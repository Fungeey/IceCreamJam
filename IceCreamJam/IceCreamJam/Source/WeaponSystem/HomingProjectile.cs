using IceCreamJam.Systems;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.WeaponSystem {
    class HomingProjectile : Projectile {

        public float turnSpeed = 0.05f;
        public Entity target;
        public Vector2 targetHeading;
        public static HomingProjectileSystem homingSystem;

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            if(homingSystem == null)
                homingSystem = Scene.GetEntityProcessor<HomingProjectileSystem>();

            homingSystem.projectiles.Add(this);
        }

        public override Vector2 CalculateVector() {
            return targetHeading * speed * Time.DeltaTime;
        }

        public override void Update() {
            base.Update();

            if(target == null)
                return;

            // Get angle towards target
            var targetAngle = Mathf.AngleBetweenVectors(this.Position, target.Position);
            float newAngle = Mathf.ApproachAngleRadians(this.Rotation, targetAngle, turnSpeed);

            // Calculate a new heading based upon the new angle
            var newHeading = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
            newHeading.Normalize();
            targetHeading = newHeading;

            // Rotate projectile to face target
            this.Rotation = newAngle;
        }

        public override void OnHit(CollisionResult? result) {
            base.OnHit(result);
        }
    }
}
