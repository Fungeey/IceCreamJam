using IceCreamJam.Source.Content;
using IceCreamJam.Source.Entities;
using IceCreamJam.Source.WeaponSystem.Projectiles;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;

namespace IceCreamJam.Source.WeaponSystem.Weapons {
    class PopsicleGun : Weapon {

        private AnimatedEntity shatterFX;
        private SpriteEntity loadedPopsicle;
        private ICoroutine reformCoroutine;
        private float reformTime; // Time before the popsicle reforms 

        public PopsicleGun() {
            projectileType = typeof(Popsicle);
            Name = "PopsicleGun";
            ReloadTime = 1f;
            texturePath = ContentPaths.Popsicle_Base;
            weaponMountOffset = new Vector2(0, -5);
            ammoCost = 3;
            iconIndex = 1;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            var popsicleTexture = Scene.Content.LoadTexture(ContentPaths.Popsicle + "Popsicle_Basic.png");
            loadedPopsicle = Scene.AddEntity(new SpriteEntity(popsicleTexture, Constants.Layer_WeaponOver, 0.5f));
            loadedPopsicle.ToggleVisible(defaultVisible);

            shatterFX = Scene.AddEntity(new AnimatedEntity());
            AddFXAnimation();
            shatterFX.ToggleVisible(false);
            shatterFX.animator.RenderLayer = Constants.Layer_WeaponOver;
            shatterFX.animator.LayerDepth = 0.4f;
            shatterFX.animator.OnAnimationCompletedEvent += (s) => ReloadPopsicle();
        }

        private void AddFXAnimation() {
            var texture = Scene.Content.LoadTexture(ContentPaths.Popsicle_Shatter);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 23);

            sprites.Reverse();
            shatterFX.animator.AddAnimation("Reform", Constants.GlobalFPS * 4, sprites.ToArray());

            // Calculate delay before reforming popsicle
            //reformTime = reloadTime - (sprites.Count / (Constants.GlobalFPS * 4));
            reformTime = (ReloadTime - 0.15f); // Jank approximation that looks good enough
        }

        public override Projectile InstantiateProjectile(Vector2 pos) {
            var scene = weaponComponent.Entity.Scene;
            var dir = Vector2.Normalize(scene.Camera.MouseToWorldPoint() - (Position));

            var p = Pool<Popsicle>.Obtain();
            p.Initialize(dir, Position + weaponMountOffset);
            return p;
        }

        public override void OnEquipped() {
            base.OnEquipped();
            loadedPopsicle.ToggleVisible(true);
        }

        public override void OnUnequipped() {
            base.OnUnequipped();
            shatterFX.ToggleVisible(false);
            loadedPopsicle.ToggleVisible(false);
        }

        //public override void OnShoot() {
        //    base.OnShoot();
        //
        //    //reformCoroutine = Core.StartCoroutine(ReloadTimer());
        //}

        //IEnumerator ReloadTimer() {
        //    loadedPopsicle.ToggleVisible(false);
        //    yield return Coroutine.WaitForSeconds(reformTime);
        //    // Reform popsicle
        //    shatterFX.ToggleVisible(true);
        //    shatterFX.animator.Play("Reform", Nez.Sprites.SpriteAnimator.LoopMode.ClampForever);
        //    loadedPopsicle.ToggleVisible(false);
        //}

        private void ReloadPopsicle() {
            shatterFX.ToggleVisible(false);
            loadedPopsicle.ToggleVisible(!InputManager.shoot.IsDown);
        }

        public override void Update() {
            base.Update();
            var dir = Vector2.Normalize(Scene.Camera.MouseToWorldPoint() - Position);
            shatterFX.Rotation = loadedPopsicle.Rotation = Mathf.Atan2(dir.Y, dir.X);
            shatterFX.Position = loadedPopsicle.Position = Position + weaponMountOffset;
        }
    }
}
