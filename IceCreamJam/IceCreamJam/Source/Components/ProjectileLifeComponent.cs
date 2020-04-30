using IceCreamJam.WeaponSystem;
using Nez;

namespace IceCreamJam.Components {
    class ProjectileLifeComponent : Component, IUpdatable {
        public float lifetime;
        public float timeProgress;
        public bool isFinished = true;
        public float progress;
        
        public ProjectileLifeComponent(float lifetime) {
            this.lifetime = lifetime;
            Start();
        }

        public void Start() {
            isFinished = false;
            progress = 0;
            this.timeProgress = 0f;
        }

        public void Update() {
            if(isFinished)
                return;

            timeProgress += Time.DeltaTime;
            var remainingTime = lifetime - timeProgress;
            progress = remainingTime / lifetime;

            if(remainingTime < 0.01f) {
                (Entity as Projectile).OnHit(null);
                isFinished = true;
                progress = 1;
            }
        }
    }
}
