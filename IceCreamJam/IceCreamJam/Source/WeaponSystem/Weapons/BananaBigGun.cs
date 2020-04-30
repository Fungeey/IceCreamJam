
using IceCreamJam.WeaponSystem.Projectiles;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.WeaponSystem.Weapons {
    class BananaBigGun : Weapon {
        public BananaBigGun() {
            projectileType = typeof(BananaBig);
            Name = "BananaBigGun";
            ReloadTime = 0.5f;
            texturePath = ContentPaths.Banana_Base_Big;
            ammoCost = 5;
            iconIndex = 5;
        }

        public override void InitializeRenderer() {
            base.InitializeRenderer();
            renderer.LocalOffset = new Vector2(0, -5);
        }

        public override Projectile InstantiateProjectile(Vector2 pos) {
            var scene = weaponComponent.Entity.Scene;
            var dir = Vector2.Normalize(scene.Camera.MouseToWorldPoint() - weaponComponent.Entity.Position);

            var b = Pool<BananaBig>.Obtain();
            b.Initialize(dir, pos);

            return b;
        }
    }
}
