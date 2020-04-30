
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.WeaponSystem.EnemyWeapons {
    class DoctorKnife : EnemyProjectile {
        public DoctorKnife() {
            Name = "DoctorKnife";
        }

        public override void Initialize(Vector2 direction, Vector2 position) {
            base.Initialize(direction, position);

            speed = 120;
            damage = 1;
            lifetime = 2;
            texturePath = ContentPaths.Doctor + "DocKnife.png";
        }

        public override void OnHit(CollisionResult? result) {
            base.OnHit(result);
            Pool<DoctorKnife>.Free(this);
        }
    }
}
