using IceCreamJam.Source.Content;
using IceCreamJam.Source.WeaponSystem.Projectiles;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections;

namespace IceCreamJam.Source.WeaponSystem.Weapons {
    class BananaSmallGun : Weapon {

        private const float bananaDelay = 0.1f;
        private Vector2 shootDir;

        public BananaSmallGun() {
            projectileType = typeof(BananaSmall);
            Name = "BananaSmallGun";
            ReloadTime = 0.5f;
            texturePath = ContentPaths.Banana_Base_Small;
            ammoCost = 3;
            iconIndex = 2;
        }

        public override void InitializeRenderer() {
            base.InitializeRenderer();
            this.renderer.LocalOffset = new Vector2(0, -5);
        }

        public override Projectile InstantiateProjectile(Vector2 pos) {
            var scene = weaponComponent.Entity.Scene;
            if(shootDir == Vector2.Zero)
                shootDir = Vector2.Normalize(scene.Camera.MouseToWorldPoint() - weaponComponent.Entity.Position);

            var b = Pool<BananaSmall>.Obtain(); 
            b.Initialize(shootDir, pos);

            return b;
        }

        public override void OnShoot() {
            base.OnShoot();
            Core.StartCoroutine(ShootSecondaryBananas());
        }

        IEnumerator ShootSecondaryBananas() {
            yield return Coroutine.WaitForSeconds(bananaDelay);
            ShootBanana();
            yield return Coroutine.WaitForSeconds(bananaDelay);
            ShootBanana();
            shootDir = Vector2.Zero;
        }

        private void ShootBanana() {
            var p = InstantiateProjectile(this.Position);
            if(p.IsNewProjectile)
                weaponComponent.Entity.Scene.AddEntity(p);
        }
    }
}
