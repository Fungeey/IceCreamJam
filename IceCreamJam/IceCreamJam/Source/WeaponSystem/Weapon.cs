
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections;

namespace IceCreamJam.WeaponSystem {
    abstract class Weapon : Entity {

        protected Type projectileType;
        protected float ReloadTime;
        private bool canShoot = true;
        protected string texturePath;
        protected Vector2 weaponMountOffset;
        public int ammoCost;
        public int iconIndex;

        public bool defaultVisible = false;
        public PlayerWeaponComponent weaponComponent;
        protected RenderableComponent renderer;

        public virtual void OnEquipped() => SetVisible(true);
        public virtual void OnUnequipped() => SetVisible(false);
        public void SetVisible(bool visible) => renderer?.SetEnabled(visible);

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            InitializeRenderer();
            SetVisible(defaultVisible);
        }

        public virtual void InitializeRenderer() {
            renderer = new SpriteRenderer(Scene.Content.LoadTexture(texturePath)) {
                RenderLayer = Constants.RenderLayer_Truck,
                LayerDepth = 0.8f
            };

            AddComponent(renderer);
        }

        public virtual void Shoot() {
            bool sufficientAmmo = weaponComponent.Ammo >= ammoCost;
            if(canShoot && sufficientAmmo) {
                canShoot = false;
                Core.StartCoroutine(ReloadTimer());

                // Instantiate a projectile using the weapon's projectile type
                var p = InstantiateProjectile(this.Position);

                // Don't add it to the scene if it's already been added
                if(p.IsNewProjectile)
                    weaponComponent.Entity.Scene.AddEntity(p); 

                OnShoot();
            }
        }

        public virtual void OnShoot() => weaponComponent.OnShoot();
        public abstract Projectile InstantiateProjectile(Vector2 pos);

        IEnumerator ReloadTimer() {
            yield return Coroutine.WaitForSeconds(ReloadTime);
            canShoot = true;
        }
    }
}
